using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Audio
{
    /// <summary>
    /// Allows for more customization for each <see cref="UnityEngine.AudioClip"/> in the Inspector.
    /// This allows for setting different settings for an <see cref="UnityEngine.AudioClip"/>, 
    /// which can be useful for playing different <see cref="UnityEngine.AudioClip"/> with different settings in the same <see cref="AudioSource"/>.
    /// </summary>
    public class AudioClipPlus
    {
        [field: SerializeField] public AudioClip AudioClip { get; set; }


        [Range(0f, 1f)][SerializeField] protected float minVolume;
        public virtual float MinVolume { get => minVolume; }
        [Range(0f, 1f)][SerializeField] protected float maxVolume;
        public virtual float MaxVolume { get => maxVolume; }


        [Range(0f, 2f)][SerializeField] protected float minPitch;
        public virtual float MinPitch { get => minPitch; }
        [Range(0f, 2f)][SerializeField] protected float maxPitch;
        public virtual float MaxPitch { get => maxPitch; }

        /// <summary>
        /// Will set this <see cref="AudioClipPlus"/> settings to the <paramref name="audioSource"/>. 
        /// The volume on the <see cref="AudioSource"/> will be a random value from <see cref="MinVolume"/> to <see cref="MaxVolume"/> inclusive.
        /// The pitch on the <see cref="AudioSource"/> will be set as a random value from <see cref="MinPitch"/> to <see cref="MaxPitch"/>.
        /// Note: looping will always be set to false when this is called because in most circumstances sounds should not loop.
        /// The <see cref="UnityEngine.AudioClip"/>, <see cref="AudioClip"/>, will be also be set as the clip on the <see cref="AudioSource"/>.
        /// </summary>
        /// <param name="audioSource"></param>
        public void SetSettingsToAudioSource(AudioSource audioSource)
        {
            audioSource.volume = UnityEngine.Random.Range(MinVolume, MaxVolume);
            audioSource.pitch = UnityEngine.Random.Range(MinPitch, MaxPitch);
            audioSource.loop = false;
            audioSource.clip = AudioClip;
        }

        /// <summary>
        /// Will overwrite the settings on the <paramref name="audioSource"/> to the settings on this <see cref="AudioClipPlus"/> using <see cref="SetSettingsToAudioSource(AudioSource)"/>.
        /// It will then play the sound on the <see cref="AudioSource"/> using <see cref="AudioSource.Play"/>. This should be used most of the time rather than <see cref="PlaySound(AudioSource)"/>.
        /// It is recommended that when playing <see cref="UnityEngine.AudioClip"/> from <see cref="AudioClipPlus"/> it is done in an <see cref="AudioSource"/> meant to be used by 
        /// <see cref="AudioClipPlus"/> only!
        /// </summary>
        /// <param name="audioSource"></param>
        public void PlaySoundOverwrite(AudioSource audioSource)
        {
            SetSettingsToAudioSource(audioSource);
            audioSource.Play();
        }

        /// <summary>
        /// Will simply play the <see cref="UnityEngine.AudioClip"/>, <see cref="AudioClip"/>, using <see cref="AudioSource.PlayOneShot(AudioClip)"/>.
        /// Most of the time, you want to use <see cref="PlaySoundOverwrite(AudioSource)"/> instead because it also changes the settings on the <see cref="AudioSource"/> to match the settings in this class.
        /// This method will only play the <see cref="UnityEngine.AudioClip"/> and does not apply the volume or the pitch changes. 
        /// This should only be used in rare circumstances where you want to play a sound with the same settings as the <paramref name="audioSource"/>.
        /// </summary>
        /// <param name="audioSource"></param>
        public void PlaySound(AudioSource audioSource) => audioSource.PlayOneShot(AudioClip);
    }
}
