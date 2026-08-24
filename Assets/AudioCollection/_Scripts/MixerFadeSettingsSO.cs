using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Audio
{
    /// <summary>
    /// Stores <see cref="MixerFadeSettings"/> data as a Scriptable Object to allow for common presets
    /// </summary>
    [CreateAssetMenu(fileName = "MixerFadeSettingsSO", menuName = "ScriptableObjects/Mixer Fade Settings")]
    public class MixerFadeSettingsSO : ScriptableObject
    {
        [field: SerializeField] public MixerFadeSettings Settings { get; private set; }
    }
}
