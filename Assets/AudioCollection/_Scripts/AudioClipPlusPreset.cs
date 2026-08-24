using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Audio
{
    /// <summary>
    /// Achieves the same functionality as <see cref="AudioClipPlus"/> but allows for setting the non-<see cref="AudioClip"/> settings in a preset.
    /// This allows for a better-looking Inspector, more organization and flexibility.
    /// </summary>
    public class AudioClipPlusPreset : AudioClipPlus
    {
        #region Hidden Inherited Members
        //We hide the Serialized members so that we can instead use the data from the preset
        private new float minVolume;

        private new float maxVolume;
        private new float minPitch;
        private new float maxPitch;
        #endregion

        [field: SerializeField] public AudioClipPlusPresetSO Preset { get; private set; }

        public override float MinVolume { get => Preset.MinVolume; }
        public override float MaxVolume { get => Preset.MaxVolume; }
        public override float MinPitch { get => Preset.MinPitch; }
        public override float MaxPitch { get => Preset.MaxPitch; }
    }
}
