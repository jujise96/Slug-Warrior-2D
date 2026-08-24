using UnityEngine.Audio;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.IO;

namespace Game.Audio
{
    /// <summary>
    /// Manages the game's audio. Only one ever exists in a scene. Access the Singleton Instance with <see cref="AudioManager.Instance"/>
    /// </summary>
    public sealed class AudioManager : MonoBehaviour
    {
        #region BackgroundAudioData Class Declaration
        /// <summary>
        /// This class is only accessable in <see cref="AudioManager"/> because
        /// it should be the only one that should have access to the data and settings states
        /// </summary>
        [System.Serializable]
        private class BackgroundAudioData
        {
            public BackgroundAudioSO Data { get; private set; }
            public BackgroundAudioState State { get; private set; }
            public AudioSource PlayingSource { get; private set; }
            public Coroutine UpdateStateCoroutine { get; set; } = null;

            public BackgroundAudioData(BackgroundAudioSO data)
            {
                this.Data = data;
                State = BackgroundAudioState.Unused;
            }

            public void SetState(BackgroundAudioState newState)
            {
                if (newState == BackgroundAudioState.Unused || newState == BackgroundAudioState.Queued)
                {
                    if (PlayingSource != null && PlayingSource.isPlaying)
                    {
                        if (Data.DisableFadeSettings!=null)
                        {
                            AudioManager.Instance.FadeAudioSourceVolume(PlayingSource, Data.DisableFadeSettings.Settings);
                        }
                        else PlayingSource.Stop();
                    }
                    PlayingSource = null;
                }
                else if (newState == BackgroundAudioState.Playing)
                {
                    AudioSource audioSource = AudioManager.Instance.GetAvailableAudioSourceFromType(Data.AudioSourceType);
                    if (audioSource == null)
                    {
                        UnityEngine.Debug.LogError($"Tried to set the current state of {typeof(BackgroundAudioData)} to {typeof(BackgroundAudioState)} {newState} " +
                            $"but there are 0 available {typeof(AudioSource)} for the {typeof(GlobalSourceType)} {Data.AudioSourceType}!");
                        return;
                    }
                    PlayingSource= audioSource;

                    audioSource.loop = Data.Loop;
                    audioSource.clip = Data.Track;

                    if (Data.EnableFadeSettings!=null)
                    {
                        audioSource.volume = 0;
                        audioSource.Play();
                        AudioManager.Instance.FadeAudioSourceVolume(PlayingSource, Data.EnableFadeSettings.Settings);
                    }
                    else
                    {
                        audioSource.volume = Data.Volume;
                        audioSource.Play();
                    }
                }
                State = newState;
            }
        }
        #endregion

        [Header("References")]
        [SerializeField] private AudioMixer audioMixer;
        [Tooltip("This source can be used for playing global sound effects, but it should rarely be used because " +
            "you should always try to find an object that can play the sound on its own")]
        [SerializeField] private AudioSource globalSFXSource;
        [Tooltip("The source for music. Most of the time you probably just want only 1 music source playing at any given time.")]
        [SerializeField] private AudioSource musicSource;
        [Tooltip("This system allows you to have multiple ambience sources which can all play different tracks. " +
            "This is most useful when you might have environmental sounds and want to have different backround ambience, " +
            "allowing you to create multiple tracks playing at the same time")]
        [SerializeField] private AudioSource[] ambienceSources;

        //Master constant MUST not change
        private const string MIXER_MASTER_NAME = "Master";

        //These used only for the methods that should be used in settings for changing the mixer volume.
        //They are for your convenience only since they are not used anywhere else
        private const string MIXER_MUSIC_NAME = "Music";
        private const string MIXER_SFX_NAME = "SFX";
        private const string MIXER_VOICE_NAME = "Voice";

        /// <summary>
        /// All the audio that can be played in the background by this manager (such as music, ambience, random background music/ambience, etc...)
        /// </summary>
        [Tooltip("All BackgroundAudioSO, music and ambience, must be in this array so that AudioManager can keep track of data and states. " +
            "Global SFX do not need BackgroundAudioSO!")]
        [SerializeField] private List<BackgroundAudioSO> backgroundAudio = new List<BackgroundAudioSO>();
#if UNITY_EDITOR
        public List<BackgroundAudioSO> BackgroundAudio {get=> backgroundAudio; set => backgroundAudio= value;}
#endif

        /// <summary>
        /// The corresponding data for <see cref="backgroundAudio"/>
        /// </summary>
        private List<BackgroundAudioData> backgroundAudioData = new List<BackgroundAudioData>();

        #region Random Backgound Audio Fields/Properties
        /// <summary>
        /// All the currently possible random <see cref="BackgroundAudioSO"/> corresponding data that can be played when getting this list
        /// </summary>
        private List<BackgroundAudioData> queuedRandomBackgroundAudio = null;

        /// <summary>
        /// Checks if there are possible random <see cref="BackgroundAudioData"/> queued to be played in the background 
        /// (meaning that <see cref="StartRandomBackgroundAudio(List{BackgroundAudioSO})"/> has been called).
        /// </summary>
        public bool IsRandomBackgroundAudioQueued { get => queuedRandomBackgroundAudio != null && queuedRandomBackgroundAudio.Count > 0; }

        /// <summary>
        /// The currently playing random background audio when getting this data. Null if there is no random background audio playing
        /// </summary>
        private BackgroundAudioData currentPlayingRandomBackgroundAudio = null;

        /// <summary>
        /// The current interval until the next random background audio can be played (time until next random background audio or silence is choosen).
        /// -1 if there are no random background audio queued.
        /// </summary>
        private float randomBackgroundAudioInterval = -1;
        private float currentRandomBackgroundAudioTime = 0;

        /// <summary>
        /// The time in seconds that a check is done for random background audio updates
        /// </summary>
        private const float RANDOM_BACKGROUND_AUDIO_UPDATE_TIME = 1;
        private float currentUpdateTime = 0;
        #endregion


        [Tooltip("When a random background audio finishes playing, the minimum amount of time in seconds that can be randomly choosen " +
            "before picking the next random background audio to play")]
        [SerializeField] private float minPlayInterval;

        [Tooltip("When a random background audio finishes playing, the maximum amount of time in seconds that can be randomly choosen " +
            "before picking the next random background audio to play")]
        [SerializeField] private float maxPlayInterval;

        [Tooltip("If no audio is chosen to play randomly in the background, the minimum duration for silence")]
        [SerializeField] private float minSilenceDuration;
        [Tooltip("If no audio is chosen to play randomly in the background, the maximum duration for silence")]
        [SerializeField] private float maxSilenceDuration;



        [Header("Editor")]
        [ReadOnly] [Tooltip("The path used to get the location of all Background Audio to add them all to AudioManager")]
        [SerializeField] private string backgroundAudioPath = "Assets/AudioCollection/ScriptableObjects/BackgroundAudio";
        public string BackgroundAudioPath { get => backgroundAudioPath; }
        public static AudioManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            //Setup the background audio data
            if (backgroundAudio.Count > 0)
            {
                foreach (var audio in backgroundAudio) backgroundAudioData.Add(new BackgroundAudioData(audio));
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!IsRandomBackgroundAudioQueued) return;

            currentUpdateTime += Time.unscaledDeltaTime;
            currentRandomBackgroundAudioTime += Time.unscaledDeltaTime;

            //We do a cooldown time to not decrease performance since we are calling lots of operations in Update()
            if (currentUpdateTime>= RANDOM_BACKGROUND_AUDIO_UPDATE_TIME)
            {
                currentUpdateTime = 0;
                if (randomBackgroundAudioInterval>= currentRandomBackgroundAudioTime)
                {
                    currentRandomBackgroundAudioTime = 0;
                    SetNewRandomBackgroundAudio();
                }
            }
        }

        #region Utility
        /// <summary>
        /// Converts a volume (int) from 0-1 to a dB (float) from -80 to 20 normally, 
        /// but if <paramref name="is100Scale"/> is TRUE, will convert it from a 0-100 value
        /// </summary>
        /// <param name="startVolume"></param>
        /// <returns></returns>
        private float ConvertVolumeToDecibels(float startVolume, bool is100Scale= false)
        {
            //we set the volume from 0-1 and then switch it to dB which Mixer uses (-80dB to 20dB)
            float newVolume = 0;

            //Lower extreme is not 0 because logs have a vertical asymptote at 0 and therefore can not have an input of 0
            if (is100Scale) newVolume= Mathf.Clamp(startVolume, 0.0001f, 100f);
            else newVolume= Mathf.Clamp(startVolume, 0.0001f, 1f);

            if (is100Scale) newVolume /= 100;
            newVolume = Mathf.Log10(newVolume) * 20;
            return newVolume;
        }

        /// <summary>
        /// Converts a dB (float) from -80 to 20 to a volume (int) from 0-1 normally, 
        /// but if <paramref name="multiplyBy100"/> is TRUE, will return a 0-100 value
        /// </summary>
        /// <param name="decibelVolume"></param>
        /// <returns></returns>
        private float ConvertDecibelsToVolume(float decibelVolume, bool multiplyBy100= false)
        {
            int newVolume = (int)Mathf.Clamp(decibelVolume, -80f, 20f);
            newVolume = (int)Mathf.Pow(10, newVolume / 20);
            if (multiplyBy100) newVolume *= 100;
            return newVolume;
        }

        public void SetMasterVolume(float volume) => audioMixer.SetFloat(GetMixerVolumeParamFromGroupName(MIXER_MASTER_NAME), ConvertVolumeToDecibels(volume, true));
        public void SetMusicVolume(float volume) => audioMixer.SetFloat(GetMixerVolumeParamFromGroupName(MIXER_MUSIC_NAME), ConvertVolumeToDecibels(volume, true));
        public void SetSFXVolume(float volume) => audioMixer.SetFloat(GetMixerVolumeParamFromGroupName(MIXER_SFX_NAME), ConvertVolumeToDecibels(volume, true));
        public void SetVoiceVolume(float volume) => audioMixer.SetFloat(GetMixerVolumeParamFromGroupName(MIXER_VOICE_NAME), ConvertVolumeToDecibels(volume, true));

        /// <summary>
        /// Will get current not already playing <see cref="AudioSource"/> based on the <see cref="GlobalSourceType"/>. 
        /// If <paramref name="throwIfPlaying"/> is false, it will NOT throw errors if an <see cref="AudioSource"/> found is playing another <see cref="AudioClip"/>.
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="throwIfPlaying"></param>
        /// <returns></returns>
        public AudioSource GetAvailableAudioSourceFromType(GlobalSourceType sourceType, bool throwIfPlaying= true)
        {
            switch (sourceType)
            {
                case GlobalSourceType.GlobalSoundEffects:
                    if(globalSFXSource.isPlaying)
                    {
                        if (throwIfPlaying) UnityEngine.Debug.LogError($"Tried to get the {typeof(AudioSource)} from {typeof(GlobalSourceType)} {sourceType} " +
                            $"but the current {sourceType} {typeof(AudioSource)} is currently playing! You cannot retrieve {typeof(AudioSource)} when they are still playing!");
                        return null;
                    }
                    return globalSFXSource;
                case GlobalSourceType.Music:
                    if (musicSource.isPlaying)
                    {
                        if (throwIfPlaying) UnityEngine.Debug.LogError($"Tried to get the {typeof(AudioSource)} from {typeof(GlobalSourceType)} {sourceType} " +
                            $"but the current {sourceType} {typeof(AudioSource)} is currently playing! You cannot retrieve {typeof(AudioSource)} when they are still playing!");
                        return null;
                    }
                    return musicSource;
                case GlobalSourceType.Ambience:
                    {
                        AudioSource[] sources = ambienceSources.Where(source => !source.isPlaying).ToArray();
                        if (sources.Length == 0)
                        {
                            if (throwIfPlaying) UnityEngine.Debug.LogError($"Tried to get the {typeof(AudioSource)} from {typeof(GlobalSourceType)} {sourceType} " +
                                $"but there are 0 {typeof(AudioSource)} that are currently not playing a sound!");
                            return null;
                        }
                        else return sources.First();
                    }
                default:
                    UnityEngine.Debug.LogError($"Tried to get the {typeof(AudioSource)} from {typeof(GlobalSourceType)} {sourceType} " +
                        $"but it does not have a corresponding {typeof(AudioSource)} defined in GetAudioSourceFromType()");
                    return null;
            }
        }

        /// <summary>
        /// Will get the <see cref="AudioMixerGroup"/> volume paramter from the <see cref="AudioMixerGroup"/> name. 
        /// The volume parameter must follow the correct syntax: <<see cref="AudioMixerGroup"/> Name>"Volume".
        /// </summary>
        /// <param name="mixerGroupName"></param>
        /// <returns></returns>
        private string GetMixerVolumeParamFromGroupName(string mixerGroupName)
        {
            if (!mixerGroupName.Equals(MIXER_MASTER_NAME))
            {
                bool wasFound = false;
                foreach (var group in audioMixer.FindMatchingGroups(MIXER_MASTER_NAME))
                {
                    if (mixerGroupName.Equals(group.name))
                    {
                        wasFound = true;
                        break;
                    }
                }

                if (!wasFound)
                {
                    UnityEngine.Debug.LogError($"Tried to get an {typeof(AudioMixer)} volume exposed parameter, but the mixer group {mixerGroupName} does not exist!");
                    return "";
                }
            }

            return mixerGroupName + "Volume";
        }

        /// <summary>
        /// Will update the state for <see cref="BackgroundAudioData"/> after <paramref name="time"/> seconds.
        /// This is only really useful for updating the state of a <see cref="BackgroundAudioSO"/> where <see cref="BackgroundAudioSO.Loop"/> is false.
        /// Since non-looping <see cref="BackgroundAudioSO"/> end state is determined by their play time and not from a method like looping <see cref="BackgroundAudioSO"/>,
        /// A timer after they start playing can allow the state to be updated.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="updatedState"></param>
        /// <param name="time"></param>
        private void UpdateStateAfterTime(BackgroundAudioData data, BackgroundAudioState updatedState, float time)
        {
            data.UpdateStateCoroutine = StartCoroutine(PlayTime());
            IEnumerator PlayTime()
            {
                yield return new WaitForSecondsRealtime(data.Data.Track.length);
                data.SetState(updatedState);
                data.UpdateStateCoroutine = null;
            }
        }

        /// <summary>
        /// Will stop any updating state coroutines on <paramref name="data"/>
        /// </summary>
        /// <param name="data"></param>
        private void StopUpdatedStateAfterTime(BackgroundAudioData data)
        {
            if (data.UpdateStateCoroutine != null) StopCoroutine(data.UpdateStateCoroutine);
        }

        /// <summary>
        /// Will get the current state of <paramref name="backgroundAudio"/> based on its corresponding data and will return a <see cref="BackgroundAudioState"/>.
        /// </summary>
        /// <param name="backgroundAudio"></param>
        /// <returns></returns>
        public BackgroundAudioState? GetCurrentStateOfBackgroundAudio(BackgroundAudioSO backgroundAudio)
        {
            if (backgroundAudioData.Count == 0)
            {
                UnityEngine.Debug.LogError($"Tried to get the current state of {backgroundAudio.Name}, but the corresponding data for all background audio has not been set up yet! " +
                    $"Make sure to call GetCurrentStateOfBackgroundAudio() only after Start() on {typeof(AudioManager)}!");
                return null;
            }

            BackgroundAudioData data= GetBackgroundAudioData(backgroundAudio);
            if (data == null)
            {
                UnityEngine.Debug.LogError($"Tried to get the data of {backgroundAudio.Name}, but the corresponding data for it was not found!");
                return null;
            }

            return data.State;
        }
        #endregion

        #region Triggering Audio

        /// <summary>
        /// Every <see cref="BackgroundAudioSO"/> in <paramref name="randomBackgroundAudio"/> will be added to the queue of possible random background audo that can be played randomly. 
        /// It will also begin the random interval and playing one of the tracks or silence if it is choosen.
        /// </summary>
        /// <param name="randomBackgroundAudio"></param>
        public void StartRandomBackgroundAudio(List<BackgroundAudioSO> randomBackgroundAudio)
        {
            if (backgroundAudioData.Count==0)
            {
                UnityEngine.Debug.LogError("Tried to start random background audio, but the corresponding data for all background audio has not been set up yet! " +
                    $"Make sure to call StartRandomBackgroundAudio() only after Start() on {typeof(AudioManager)}!");
                return;
            }
            int totalChance = 0;

            //Find the corresponding data and do some error checks to prevent unwanted behaviors
            List<BackgroundAudioData> correspondingData = new List<BackgroundAudioData>();
            foreach (var randomAudio in randomBackgroundAudio)
            {
                BackgroundAudioData foundData = GetBackgroundAudioData(randomAudio);
                if (foundData== default || foundData==null)
                {
                    UnityEngine.Debug.LogError($"Tried to find corresponding data for the {typeof(BackgroundAudioSO)} named '{randomAudio.name}' but it does not exist! " +
                        $"Make sure to only call StartRandomBackgroundAudio() when all data is set!");
                    correspondingData.Clear();
                    return;
                }
                if (!foundData.Data.IsRandomlyPlayed)
                {
                    UnityEngine.Debug.LogError($"Tried to start random background audio, but {typeof(BackgroundAudioSO)} named '{randomAudio.name}' cannot be played randomly! " +
                        $"Make sure isRandomlyPlayed is set to true!");
                    correspondingData.Clear();
                    return;
                }
                correspondingData.Add(foundData);
                totalChance += randomAudio.PlayChance;
            }
            if (totalChance>100)
            {
                UnityEngine.Debug.LogError($"Tried to start random background audio, but the total chance of all background audio in the argument, {totalChance} is greater than 100!");
                correspondingData.Clear();
                return;
            }

            queuedRandomBackgroundAudio= correspondingData;
            SetNewRandomBackgroundAudio();
        }

        /// <summary>
        /// Will stop all <see cref="BackgroundAudioSO"/> in <paramref name="randomBackgroundAudio"/> if they are playing and will also remove them from the queue of the random background audio.
        /// If there are still some remaining random background audio will continue to play. If all of the queued <see cref="BackgroundAudioSO"/> are removed, random background audio will stop.
        /// </summary>
        /// <param name="randomBackgroundAudio"></param>
        public void StopRandomBackgroundAudio(List<BackgroundAudioSO> randomBackgroundAudio)
        {
            if (backgroundAudioData.Count == 0)
            {
                UnityEngine.Debug.LogError("Tried to stop random background audio, but the corresponding data for all background audio has not been set up yet! " +
                    $"Make sure to call StopRandomBackgroundAudio() only after Start() on {typeof(AudioManager)}!");
                return;
            }

            //Find the corresponding data and do some error checks to prevent unwanted behaviors
            List<BackgroundAudioData> correspondingData = new List<BackgroundAudioData>();
            foreach (var randomAudio in randomBackgroundAudio)
            {
                BackgroundAudioData foundData = GetBackgroundAudioData(randomAudio);
                if (foundData == default || foundData == null)
                {
                    UnityEngine.Debug.LogError($"Tried to find corresponding data for the {typeof(BackgroundAudioSO)} named '{randomAudio.Name}' but it does not exist! " +
                        $"Make sure to only call StopRandomBackgroundAudio() when all data is set!");
                    correspondingData.Clear();
                    return;
                }
                if (!foundData.Data.IsRandomlyPlayed)
                {
                    UnityEngine.Debug.LogError($"Tried to stop random background audio, but {typeof(BackgroundAudioSO)} named '{randomAudio.Name}' cannot be played randomly! " +
                        $"Make sure isRandomlyPlayed is set to true!");
                    correspondingData.Clear();
                    return;
                }
                if (foundData.State== BackgroundAudioState.Unused)
                {
                    UnityEngine.Debug.LogError($"Tried to stop playing {typeof(BackgroundAudioSO)} named '{randomAudio.Name}' but it is not currently not used as random background audio! " +
                        $"Make sure to specify the correct {typeof(BackgroundAudioSO)} in your argument!");
                    correspondingData.Clear();
                    return;
                }
                correspondingData.Add(foundData);
            }

            foreach (var data in correspondingData)
            {
                //If we stop random background audio while still playing, the coroutine continues to run, so we stop it
                if (data.UpdateStateCoroutine != null) StopCoroutine(data.UpdateStateCoroutine);
                data.SetState(BackgroundAudioState.Unused);
            }
            queuedRandomBackgroundAudio = null;
            currentPlayingRandomBackgroundAudio = null;
        }

        private void SetNewRandomBackgroundAudio()
        {
            //Assign ranges to the data based on the play chance so we know what numbers correspond to what background audio
            Dictionary<BackgroundAudioData, Vector2> dataRanges = new Dictionary<BackgroundAudioData, Vector2>();
            int startRange = 1;
            foreach (var data in queuedRandomBackgroundAudio)
            {
                data.SetState(BackgroundAudioState.Queued);
                dataRanges.Add(data, new Vector2(Mathf.Clamp(startRange, 1, 100), Mathf.Clamp(startRange + data.Data.PlayChance - 1, 1, 100)));
            }

            //Choose a random value and find the data the corresponds to it
            int choosenValue = UnityEngine.Random.Range(1, 101);
            BackgroundAudioData dataToPlay = null;
            foreach (var range in dataRanges)
            {
                if (choosenValue >= range.Value.x && choosenValue <= range.Value.y)
                {
                    dataToPlay = range.Key;
                    break;
                }
            }

            //Set the current playing audio and the time until it can be rechoosen
            float randomInterval = UnityEngine.Random.Range(minPlayInterval, maxPlayInterval);
            if (dataToPlay != null)
            {
                currentPlayingRandomBackgroundAudio = dataToPlay;
                randomBackgroundAudioInterval = dataToPlay.Data.Track.length + randomInterval;

                dataToPlay.SetState(BackgroundAudioState.Playing);

                //If we don't loop (should be all randomly played audio), we need to automatically set to a queued state after the time is up without needing the user to call a method
                if (!dataToPlay.Data.Loop) UpdateStateAfterTime(dataToPlay, BackgroundAudioState.Queued, dataToPlay.Data.Track.length);
            }
            else
            {
                currentPlayingRandomBackgroundAudio = null;
                randomBackgroundAudioInterval = UnityEngine.Random.Range(minSilenceDuration, maxSilenceDuration) + randomInterval;
            }
        }

        private BackgroundAudioData GetBackgroundAudioData(BackgroundAudioSO backgroundAudio) => backgroundAudioData.Where(data => data.Data==backgroundAudio).FirstOrDefault();

        /// <summary>
        /// Will begin playing the <see cref="BackgroundAudioSO.Track"/> if <see cref="BackgroundAudioSO.IsRandomlyPlayed"/> is false.
        /// <br>If <see cref="BackgroundAudioSO.Loop"/> is false, will automatically update the <see cref="BackgroundAudioData"/> to a stopped state 
        /// without needing to call <see cref="DisableBackgroundAudio(BackgroundAudioSO)"/>.</br>
        /// <br>If you want to play background audio at random intervals use <see cref="StartRandomBackgroundAudio(List{BackgroundAudioSO})"/> instead.</br>
        /// </summary>
        /// <param name="backgroundAudio"></param>
        public void EnableBackgroundAudio(BackgroundAudioSO backgroundAudio)
        {
            if (backgroundAudioData.Count == 0)
            {
                UnityEngine.Debug.LogError($"Tried to start background audio, but the corresponding data for all background audio has not been set up yet! " +
                    $"Make sure to call EnableBackgroundAudio() only after Start() on {typeof(AudioManager)}!");
                return;
            }

            if (backgroundAudio.IsRandomlyPlayed)
            {
                UnityEngine.Debug.LogError($"Tried to enable background audio in EnableBackgroundAudio() for {typeof(BackgroundAudioSO)} named '{backgroundAudio.Name}' " +
                    $"but it is randomly played and EnableBackgroundAudio() only plays not random background audio! Use StartRandomBackgroundAudio() instead for random background audio!");
                return;
            }
            BackgroundAudioData data = GetBackgroundAudioData(backgroundAudio);
            if (data==null || data==default)
            {
                UnityEngine.Debug.LogError($"Tried to find corresponding data for the {typeof(BackgroundAudioSO)} named '{backgroundAudio.Name}' but it does not exist! " +
                        $"Make sure to only call EnableBackgroundAudio() when all data is set!");
                return;
            }
            data.SetState(BackgroundAudioState.Playing);
            if (!data.Data.Loop) UpdateStateAfterTime(data, BackgroundAudioState.Unused, data.Data.Track.length);
        }

        /// <summary>
        /// Will stop playing the <see cref="BackgroundAudioSO.Track"/> if <see cref="BackgroundAudioSO.IsRandomlyPlayed"/> is false.
        /// </summary>
        /// <param name="backgroundAudio"></param>
        public void DisableBackgroundAudio(BackgroundAudioSO backgroundAudio)
        {
            if (backgroundAudioData.Count == 0)
            {
                UnityEngine.Debug.LogError($"Tried to stop background audio, but the corresponding data for all background audio has not been set up yet! " +
                    $"Make sure to call DisableBackgroundAudio() only after Start() on {typeof(AudioManager)}!");
                return;
            }
            if (backgroundAudio.IsRandomlyPlayed)
            {
                UnityEngine.Debug.LogError($"Tried to disable background audio in DisableBackgroundAudio() for {typeof(BackgroundAudioSO)} named '{backgroundAudio.Name}' " +
                    $"but it is randomly played and DisableBackgroundAudio() only plays not random background audio! Use StopRandomBackgroundAudio() instead for random background audio!");
                return;
            }
            BackgroundAudioData data = GetBackgroundAudioData(backgroundAudio);
            if (data == null || data == default)
            {
                UnityEngine.Debug.LogError($"Tried to find corresponding data for the {typeof(BackgroundAudioSO)} named '{backgroundAudio.Name}' but it does not exist! " +
                        $"Make sure to only call DisableBackgroundAudio() when all data is set!");
                return;
            }
            if (data.State!= BackgroundAudioState.Playing)
            {
                UnityEngine.Debug.LogError($"Tried to disable background audio for the {typeof(BackgroundAudioSO)} named '{backgroundAudio.Name}', but it is not currently playing!");
                return;
            }

            //If we call this before the audio is over, we stop the coroutine that sets the updated state
            StopUpdatedStateAfterTime(data);

            data.SetState(BackgroundAudioState.Unused);
        }

        /// <summary>
        /// Will play an <see cref="AudioClip"/> from the global SFX <see cref="AudioSource"/> if it is available (meaning there are no other sounds using it).
        /// <br><see cref="AudioSource.volume"/> will be set to 1, <see cref="AudioSource.pitch"/> will be set to 1, and <see cref="AudioSource.loop"/> will be set to false.</br>
        /// <br>If you want more control over these settings use <see cref="PlayGlobalClip(AudioClipPlus)"/></br>
        /// </summary>
        /// <param name="clip"></param>
        public void PlayGlobalClip(AudioClip clip)
        {
            AudioSource source = GetAvailableAudioSourceFromType(GlobalSourceType.GlobalSoundEffects);
            if (source == null) return;

            source.volume = 1;
            source.pitch = 1;
            source.clip= clip;
            source.loop = false;
            source.Play();
        }

        /// <summary>
        /// Will play an <see cref="AudioClip"/> from the global SFX <see cref="AudioSource"/> if it is available (meaning there are no other sounds using it).
        /// <br>It will set the <see cref="AudioSource"/> for SFX to the settings on the <see cref="AudioClipPlus"/> using <see cref="AudioClipPlus.PlaySoundOverwrite(AudioSource)"/></br>
        /// </summary>
        /// <param name="clip"></param>
        public void PlayGlobalClip(AudioClipPlus clip)
        {
            AudioSource source = GetAvailableAudioSourceFromType(GlobalSourceType.GlobalSoundEffects);
            if (source == null) return;
            
            clip.PlaySoundOverwrite(source);
        }
        #endregion

        #region Fading Methods
        /// <summary>
        /// Will fade the <see cref="AudioMixerGroup"/> volume exposed parameter based on the <paramref name="fadeSettings"/>. 
        /// See <see cref="MixerFadeSettings"/> for more info on the values.
        /// </summary>
        /// <param name="fadeSettings"></param>
        /// <returns></returns>
        private IEnumerator FadeMixerGroupVolumeCoroutine(MixerFadeSettings fadeSettings)
        {
            //string volumeParam = GetMixerVolumeParamFromGroupName(fadeSettings.MixerGroupName);
            string volumeParam = GetMixerVolumeParamFromGroupName(fadeSettings.MixerGroupToFade.name);

            float currentTime = 0;
            audioMixer.GetFloat(volumeParam, out float startVolume);

            startVolume = ConvertDecibelsToVolume(startVolume);

            //note: second parameter is 0.0001 because if its 0, then audioMixer breaks because it works logarithmically.
            float targetValue = Mathf.Clamp(fadeSettings.TargetEndVolume, 0.0001f, 1f);

            while (currentTime < fadeSettings.FadeDuration)
            {
                currentTime += Time.unscaledDeltaTime;
                float newVolume = Mathf.Lerp(startVolume, targetValue, currentTime / fadeSettings.FadeDuration);
                audioMixer.SetFloat(volumeParam, ConvertVolumeToDecibels(newVolume));
                yield return null;
            }

            //If we return back to default, we start a separate coroutine before exiting
            if (fadeSettings.ReturnToDefaultVolume!= false)
            {
                StartCoroutine(ReturnToDefaultDelay());
                IEnumerator ReturnToDefaultDelay()
                {
                    yield return new WaitForSecondsRealtime(fadeSettings.ReturnToDefaultDelay);
                    audioMixer.SetFloat(volumeParam, ConvertVolumeToDecibels(startVolume));
                }
            }
            yield break;
        }

        /// <summary>
        /// A version of <see cref="FadeMixerGroupVolumeCoroutine(MixerFadeSettings)"/> that does not require a Coroutine to start.
        /// Will fade the <see cref="AudioMixerGroup"/> volume exposed parameter based on the <paramref name="fadeSettings"/>. 
        /// See <see cref="MixerFadeSettings"/> for more info on the values.
        /// </summary>
        /// <param name="fadeSettings"></param>
        public void FadeMixerGroupVolume(MixerFadeSettings fadeSettings) => FadeMixerGroupVolumeCoroutine(fadeSettings);


        /// <summary>
        /// Will fade the volume of a <see cref="AudioSource"/> using <paramref name="fadeSettings"/>.
        /// If the end volume is 0, the <see cref="AudioSource"/> will stop playing.
        /// See <see cref="SourceFadeSettings"/> for more info on the values.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="fadeSettings"></param>
        /// <returns></returns>
        private IEnumerator FadeAudioSourceVolumeCoroutine(AudioSource source, SourceFadeSettings fadeSettings)
        {
            float currentTime = 0;
            float startVolume = source.volume;
            float endVolume = Mathf.Clamp(fadeSettings.TargetEndVolume, 0f, 1f);

            while(currentTime < fadeSettings.FadeDuration)
            {
                currentTime += Time.unscaledDeltaTime;
                source.volume = Mathf.Lerp(startVolume, endVolume, currentTime/ fadeSettings.FadeDuration);
                yield return new WaitForEndOfFrame();
                if (Mathf.Approximately(source.volume, 0))
                {
                    source.Stop();
                    yield break;
                }
            }
            yield break;
        }

        /// <summary>
        /// A version of <see cref="FadeAudioSourceVolumeCoroutine(AudioSource, SourceFadeSettings)"/> that does not require a Coroutine to start.
        /// Will fade the volume of a <see cref="AudioSource"/> using <paramref name="fadeSettings"/>.
        /// If the end volume is 0, the <see cref="AudioSource"/> will stop playing.
        /// See <see cref="SourceFadeSettings"/> for more info on the values.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="fadeSettings"></param>
        public void FadeAudioSourceVolume(AudioSource source, SourceFadeSettings fadeSettings) => StartCoroutine(FadeAudioSourceVolumeCoroutine(source, fadeSettings));
        #endregion
    }
}

