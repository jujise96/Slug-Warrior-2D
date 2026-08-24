using UnityEngine;
using UnityEngine.Audio;

namespace Game.Audio
{
    /// <summary>
    /// Stores data that can be used in fading an <see cref="AudioMixerGroup"/> and its volume.
    /// This can be useful when transitioning to fade all sounds in a group.
    /// </summary>
    [System.Serializable]
    public class MixerFadeSettings
    {

        [Tooltip("The MixerGroup that will have the volume faded.")]
        //[SerializeField] private string mixerGroup;
        [SerializeField] private AudioMixerGroup mixerGroupToFade;
        /// <summary>
        /// The name of the MixerGroup that will have the volume faded
        /// </summary>
        public AudioMixerGroup MixerGroupToFade { get => mixerGroupToFade; }

        [Tooltip("The duration in seconds to fade the MixerGroup from the current volume to the target volume")]
        [SerializeField] private float fadeDuration;
        /// <summary>
        /// The duration in seconds to fade the MixerGroup from the current volume to the target volume
        /// </summary>
        public float FadeDuration { get=> fadeDuration;}

        [Tooltip("The volume to change the MixerGroup volume to by the end of fading. Must be a value from 0-1")]
        [Range(0f, 1f)][SerializeField] private float targetEndVolume;
        /// <summary>
        /// The volume to change the MixerGroup volume to by the end of fading. Must be a value from 0-1
        /// </summary>
        public float TargetEndVolume { get => targetEndVolume;}

        [Tooltip("If true, will return to the default volume of the MixerGroup after is done fading and after the delay")] 
        [SerializeField] private bool returnToDefaultVolume;
        /// <summary>
        /// 
        /// </summary>
        public bool ReturnToDefaultVolume { get=> returnToDefaultVolume;}

        [Tooltip("If returnToDefaultVolume is true, after done fading, the time before the MixerGroup's volume is returned back to the value before fading. " +
            "If it is false, this value is ignored. Note: the mixer group volume is returned to the default volume instantly.")]
        [SerializeField] private float returnToDefaultDelay;
        public float ReturnToDefaultDelay { get=> returnToDefaultDelay;}  


        MixerFadeSettings(AudioMixerGroup mixerGroup, float fadeDuration, float targetEndVolume)
        {
            this.mixerGroupToFade = mixerGroup;
            this.fadeDuration = fadeDuration;
            this.targetEndVolume = Mathf.Clamp(targetEndVolume, 0f, 1f);
            this.returnToDefaultVolume = false;
        }

        MixerFadeSettings(AudioMixerGroup mixerGroup, float fadeDuration, float targetEndVolume, float returnToDefaultDuration)
        {
            this.mixerGroupToFade = mixerGroup;
            this.fadeDuration = fadeDuration;
            this.targetEndVolume = Mathf.Clamp(targetEndVolume, 0f, 1f);
            this.returnToDefaultVolume = true;
            this.returnToDefaultDelay = returnToDefaultDuration;
        }
    }
}

