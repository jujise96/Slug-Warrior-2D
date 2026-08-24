using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Audio
{
    /// <summary>
    /// Stores <see cref="SourceFadeSettings"/> data as a Scriptable Object to allow for common presets
    /// </summary>
    [CreateAssetMenu(fileName = "SourceFadeSettingsSO", menuName = "ScriptableObjects/AudioSource Fade Settings")]
    public class SourceFadeSettingsSO : ScriptableObject
    {
        [field: SerializeField] public SourceFadeSettings Settings { get; private set; }
    }
}
