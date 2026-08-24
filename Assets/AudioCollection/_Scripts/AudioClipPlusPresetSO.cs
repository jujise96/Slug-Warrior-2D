using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Audio
{
    /// <summary>
    /// This Scriptable Object holds all the data from <see cref="AudioClipPlus"/> except from the <see cref="AudioClip"/>.
    /// This allows for <see cref="AudioClip"/> extra settings presets for more flexibility and organization.
    /// </summary>
    [CreateAssetMenu(fileName = "AudioClipPlusPresetSO", menuName = "ScriptableObjects/AudioClip+ Preset")]
    public class AudioClipPlusPresetSO : ScriptableObject
    {
        [Range(0f, 1f)][SerializeField] private float minVolume = 1f;
        public float MinVolume { get => minVolume; }
        [Range(0f, 1f)][SerializeField] private float maxVolume = 1f;
        public float MaxVolume { get => maxVolume; }


        [Range(0f, 2f)][SerializeField] private float minPitch = 0.95f;
        public float MinPitch { get => minPitch; }
        [Range(0f, 2f)][SerializeField] private float maxPitch = 1.0f;
        public float MaxPitch { get => maxPitch; }
    }
}
