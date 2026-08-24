using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Audio
{
    /// <summary>
    /// Can trigger audio when a target enters the collider in 3D
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class AudioTrigger3D : Trigger3D
    {
        [System.Serializable]
        private class TriggeredAudioData
        {
            [field: SerializeField] public BackgroundAudioSO BackgroundAudio { get; private set; }
            [Tooltip("What occurs when entering the collider for the BackgroundAudio. " +
                "If Enable, then it will only be able to enable the BackgroundAudio if it isn't already playing. " +
                "If Disable, then it will only be able to disable the BackgroundAudio if it is not playing. " +
                "If both EnableAndDisable, then it will enable the BackgroundAudio when a player enters it from either side and then based on the side it entered, " +
                "if they enter from the other side, it will disable the BackgroundAudio")]
            [SerializeField] private ActivationType activationType;
            public ActivationType ActivationType { get => activationType; }
        }

        private enum TriggeredSide
        {
            None,
            Left,
            Right,
        }
        private TriggeredSide triggerEnableSide = TriggeredSide.None;

        [SerializeField] private new Collider collider;
        [SerializeField] private UnityEvent OnTargetEnter;
        [SerializeField] private UnityEvent OnTargetExit;


        [Tooltip("All the background audio that should be triggering when entering the collider")]
        [SerializeField] private TriggeredAudioData[] triggeredBackgroundAudio;

        //Since random background audio requires a list as an argument, we can separate them from the start rather than potentially doing that operation multiple times
        private List<BackgroundAudioSO> enableRandomBackgroundAudio = new List<BackgroundAudioSO>();
        private List<BackgroundAudioSO> disableRandomBackgroundAudio = new List<BackgroundAudioSO>();
        private List<BackgroundAudioSO> bothRandomBackgroundAudio = new List<BackgroundAudioSO>();

        private List<BackgroundAudioSO> enableDefaultBackgroundAudio = new List<BackgroundAudioSO>();
        private List<BackgroundAudioSO> disableDefaultBackgroundAudio = new List<BackgroundAudioSO>();
        private List<BackgroundAudioSO> bothDefaultBackgroundAudio = new List<BackgroundAudioSO>();


        // Start is called before the first frame update
        void Start()
        {
            if (collider == null)
            {
                UnityEngine.Debug.LogError($"The {typeof(AudioTrigger3D)} must have a {typeof(Collider)} reference for field 'Collider' or it may not work properly!");
                return;
            }

            //Separate the audio based on the types, so we don't have to run lots of loops during triggering and we can just run these once on Start()
            if (triggeredBackgroundAudio.Length > 0)
            {
                foreach (var backgroundAudio in triggeredBackgroundAudio)
                {
                    if (backgroundAudio.BackgroundAudio == null)
                    {
                        UnityEngine.Debug.LogError($"The {typeof(AudioTrigger3D)} {gameObject.name} has a triggered background audio that has a null {typeof(BackgroundAudioSO)}");
                        return;
                    }

                    //Random Background Audio
                    if (backgroundAudio.BackgroundAudio.IsRandomlyPlayed)
                    {
                        if (backgroundAudio.ActivationType == ActivationType.Enable) enableRandomBackgroundAudio.Add(backgroundAudio.BackgroundAudio);
                        else if (backgroundAudio.ActivationType == ActivationType.Disable) disableRandomBackgroundAudio.Add(backgroundAudio.BackgroundAudio);
                        else if (backgroundAudio.ActivationType == ActivationType.EnableAndDisable) bothRandomBackgroundAudio.Add(backgroundAudio.BackgroundAudio);
                    }

                    //Default Background Audio
                    else if (!backgroundAudio.BackgroundAudio.IsRandomlyPlayed)
                    {
                        if (backgroundAudio.ActivationType == ActivationType.Enable) enableDefaultBackgroundAudio.Add(backgroundAudio.BackgroundAudio);
                        else if (backgroundAudio.ActivationType == ActivationType.Disable) disableDefaultBackgroundAudio.Add(backgroundAudio.BackgroundAudio);
                        else if (backgroundAudio.ActivationType == ActivationType.EnableAndDisable) bothDefaultBackgroundAudio.Add(backgroundAudio.BackgroundAudio);
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        //-------------------------------------------------
        //KNOWN ISSUE: LAG SPIKE OCCURS WHEN LOADING SOUNDS
        //-------------------------------------------------

        private void OnTriggerEnter(Collider collider)
        {
            base.OnEnter(collider);
            OnTargetEnter?.Invoke();

            #region Enabling/Disabling Random Audio
            //Random Background Audio
            if (enableRandomBackgroundAudio.Count > 0) EnableRandomBackgroundAudio(enableRandomBackgroundAudio);
            if (disableRandomBackgroundAudio.Count > 0) DisableDefaultBackgroundAudio(disableRandomBackgroundAudio);

            //Handle if it is both sides
            if (bothRandomBackgroundAudio.Count > 0)
            {
                TriggeredSide currentTriggeredSide = IsColliderRightOfTrigger(collider) ? TriggeredSide.Right : TriggeredSide.Left;
                if (triggerEnableSide == TriggeredSide.None)
                {
                    EnableRandomBackgroundAudio(bothRandomBackgroundAudio);
                    triggerEnableSide = currentTriggeredSide;
                }
                else if (currentTriggeredSide == triggerEnableSide) EnableRandomBackgroundAudio(bothRandomBackgroundAudio);
                else if (currentTriggeredSide != triggerEnableSide) DisableDefaultBackgroundAudio(bothRandomBackgroundAudio);
            }
            #endregion

            #region Enabling/Disabling Default Audio
            //Default Background Audio
            if (enableDefaultBackgroundAudio.Count > 0) EnableDefaultBackgroundAudio(enableDefaultBackgroundAudio);
            if (disableDefaultBackgroundAudio.Count > 0) DisableDefaultBackgroundAudio(disableDefaultBackgroundAudio);

            //Handle if it is both sides
            if (bothDefaultBackgroundAudio.Count > 0)
            {
                TriggeredSide currentTriggeredSide = IsColliderRightOfTrigger(collider) ? TriggeredSide.Right : TriggeredSide.Left;
                if (triggerEnableSide == TriggeredSide.None)
                {
                    EnableDefaultBackgroundAudio(bothDefaultBackgroundAudio);
                    triggerEnableSide = currentTriggeredSide;
                }
                else if (currentTriggeredSide == triggerEnableSide) EnableDefaultBackgroundAudio(bothDefaultBackgroundAudio);
                else if (currentTriggeredSide != triggerEnableSide) DisableDefaultBackgroundAudio(bothDefaultBackgroundAudio);

                //UnityEngine.Debug.Log($"Current triggered side: {currentTriggeredSide} enable side: {triggerEnableSide}");
            }
            #endregion
        }

        private void OnTriggerExit(Collider collider)
        {
            base.OnExit(collider);
            OnTargetExit?.Invoke();
        }

        /// <summary>
        /// Returns true if <paramref name="collider"/> is to the right of this <see cref="collider"/>
        /// </summary>
        /// <param name="collider"></param>
        /// <returns></returns>
        private bool IsColliderRightOfTrigger(Collider collider) => Mathf.Sign(collider.transform.position.x - this.collider.bounds.center.x) == 1;

        #region Enabling/Disabling Random Audio Methods
        private void EnableRandomBackgroundAudio(List<BackgroundAudioSO> list)
        {
            List<BackgroundAudioSO> notPlayingList = new List<BackgroundAudioSO>();
            foreach (var audio in list)
            {
                if (AudioManager.Instance.GetCurrentStateOfBackgroundAudio(audio) != BackgroundAudioState.Playing)
                    notPlayingList.Add(audio);
            }

            if (notPlayingList.Count > 0) AudioManager.Instance.StartRandomBackgroundAudio(notPlayingList);
        }

        private void DisableRandomBackgroundAudio(List<BackgroundAudioSO> list)
        {
            List<BackgroundAudioSO> playingList = new List<BackgroundAudioSO>();
            foreach (var audio in list)
            {
                if (AudioManager.Instance.GetCurrentStateOfBackgroundAudio(audio) == BackgroundAudioState.Playing)
                    playingList.Add(audio);
            }
            if (playingList.Count > 0) AudioManager.Instance.StopRandomBackgroundAudio(playingList);
        }
        #endregion

        #region Enabling/Disabling Default Audio Methods
        private void EnableDefaultBackgroundAudio(List<BackgroundAudioSO> list)
        {
            //If we have to enable it, it must not already be playing for us to do it
            foreach (var audio in list)
            {
                if (AudioManager.Instance.GetCurrentStateOfBackgroundAudio(audio) != BackgroundAudioState.Playing)
                    AudioManager.Instance.EnableBackgroundAudio(audio);
            }
        }

        private void DisableDefaultBackgroundAudio(List<BackgroundAudioSO> list)
        {
            //If we have to disable it, we must be playing it to disable
            foreach (var audio in list)
            {
                if (AudioManager.Instance.GetCurrentStateOfBackgroundAudio(audio) == BackgroundAudioState.Playing)
                    AudioManager.Instance.DisableBackgroundAudio(audio);
            }
        }
        #endregion


    }
}
