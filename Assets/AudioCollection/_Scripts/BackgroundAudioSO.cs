using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Game.Audio
{
    /// <summary>
    /// Stores information about background audio that can be used when playing the sounds as well as identifying them
    /// </summary>
    [CreateAssetMenu(fileName = "BackgroundAudioSO", menuName = "ScriptableObjects/Background Audio")]
    public class BackgroundAudioSO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public AudioClip Track { get; private set; }
        [Range(0f, 1f)][SerializeField] private float volume = 1f;
        public float Volume { get => volume; }

        [Tooltip("The global source type for this BackgroundAudioSO. " +
            "GlobalSoundEffect should never really be an option because global sound effects DO NOT need BackgroundAudioSO")]
        [SerializeField] private GlobalSourceType audioSourceType = GlobalSourceType.Music;
        public GlobalSourceType AudioSourceType { get => audioSourceType; }

        [Tooltip("If true, will loop the track when played. This only applies to background audio that have isRandomlyPlayed set to false! " +
            "If that is true, this boolean will ALWAYS return false when retrieved")][SerializeField] private bool loop;

        /// <summary>
        /// If true, will loop the track when played. This only applies to <see cref="BackgroundAudioSO"/> that have <see cref="IsRandomlyPlayed"/> set to false!
        /// If <see cref="IsRandomlyPlayed"/> is true, this property will ALWAYS return false when retrieved.
        /// </summary>
        public bool Loop
        {
            get
            {
                if (IsRandomlyPlayed || audioSourceType==GlobalSourceType.GlobalSoundEffects) return false;
                else return loop;
            }
        }

        [Space(10)]
        [Header("Random Background Audio")]
        [Tooltip("If true, will play the track randomly in the background based on the settings below. If false, the settings below can be ignored")]
        [SerializeField] private bool isRandomlyPlayed;
        public bool IsRandomlyPlayed { get => isRandomlyPlayed; }

        [Range(0, 100)][SerializeField] private int playChance;
        public int PlayChance { get => playChance; }

        [Space(10)]
        [Header("Fading")]
        [Tooltip("Fade settings when this Track begins playing. " +
            "Leave empty if the track should immediately start playing with no fading.")]
        [SerializeField] private SourceFadeSettingsSO enableFadeSettings;
        public SourceFadeSettingsSO EnableFadeSettings { get => enableFadeSettings;}
        [Tooltip("Fade settings when this Track ends playing." +
            "Leave empty if the track should immediately stop playing with no fading.")]
        [SerializeField] private SourceFadeSettingsSO disableFadeSettings;
        public SourceFadeSettingsSO DisableFadeSettings { get => disableFadeSettings; }

        private void OnValidate()
        {
            if (enableFadeSettings!=null && volume!=enableFadeSettings.Settings.TargetEndVolume)
            {
                UnityEngine.Debug.LogWarning($"The {typeof(BackgroundAudioSO)} named '{name}' has enable fade settings with a target end volume, " +
                    $"{enableFadeSettings.Settings.TargetEndVolume}, that does not equal the default track volume, {volume}! " +
                    $"It is recommended that you keep the volumes the same or you do not include enable fade settings in order to prevent akward fluctuations in the volume!");
            }

            if (disableFadeSettings!=null && !Mathf.Approximately(disableFadeSettings.Settings.TargetEndVolume, 0f))
            {
                UnityEngine.Debug.LogWarning($"The {typeof(BackgroundAudioSO)} named '{name}' has disable fade settings with a target end volume of {disableFadeSettings.Settings.TargetEndVolume}, " +
                    $"but it is recommended to have a target end volume of 0. If it is not 0, the {typeof(AudioSource)} may continue to play at a low volume and " +
                    $"may get cut off when a new {typeof(AudioClip)} is played! If you have a target volume that is not 0, it is better to than not include any disable fade settings at all.");
            }

            if (audioSourceType == GlobalSourceType.GlobalSoundEffects)
            {
                UnityEngine.Debug.LogWarning($"The {typeof(GlobalSourceType)} for {name} is {audioSourceType}, which implies this {typeof(BackgroundAudioSO)} is a global sound effect, " +
                    $"but global sound effects do NOT need their own Scriptable Objects!");
            }
        }
    }
}
