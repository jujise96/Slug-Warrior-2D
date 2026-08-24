using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Audio
{
    /// <summary>
    /// Should correspond to what type of <see cref="AudioSource"/> a sound should be played from in the <see cref="AudioManager"/>. 
    /// Add more this list if you add more types! Make sure to also link these up with the correct prefab!
    /// </summary>
    public enum GlobalSourceType
    {
        GlobalSoundEffects,
        Music,
        Ambience,
    }
}
