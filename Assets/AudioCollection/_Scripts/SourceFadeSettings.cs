using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Audio
{
    /// <summary>
    /// Stores data that can be used in fading <see cref="AudioSource"/> and their <see cref="AudioClip"/> volume. 
    /// This can be useful when playing and stopping <see cref="AudioClip"/> and having gradual audio transitions.
    /// </summary>
    [System.Serializable]
    public class SourceFadeSettings
    {
        [Tooltip("The duration in seconds to fade the MixerGroup from the current volume to the target volume")]
        [SerializeField] private float fadeDuration;
        /// <summary>
        /// The duration in seconds to fade the MixerGroup from the current volume to the target volume
        /// </summary>
        public float FadeDuration { get => fadeDuration; }

        [Tooltip("The volume to change the MixerGroup volume to by the end of fading. Must be a value from 0-1")]
        [Range(0f, 1f)][SerializeField] private float targetEndVolume;
        /// <summary>
        /// The volume to change the MixerGroup volume to by the end of fading. Must be a value from 0-1
        /// </summary>
        public float TargetEndVolume { get => targetEndVolume; }

        public SourceFadeSettings(float fadeDuration, float targetEndVolume)
        {
            this.fadeDuration = fadeDuration;
            this.targetEndVolume = Mathf.Clamp(targetEndVolume, 0f, 1f);
        }
    }
}
