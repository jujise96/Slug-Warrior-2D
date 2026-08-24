using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Audio
{
    /// <summary>
    /// How background audio should be played. Background audio includes Music and Ambience tracks since they both play in the background.
    /// <br> --> <see cref="BackgroundPlayType.Triggered"/> means the start/end of the background audio tracks should be up to you to decide when to play (like using Triggers or triggering it programmatically).</br>
    /// <br> --> <see cref="BackgroundPlayType.PlayRandomly"/> means the background audio should be triggered randomly and is up to the <see cref="AudioManager"/>.</br>
    /// <br>An example of <see cref="BackgroundPlayType.Triggered"/> can be seen in Hollow Knight and an example of <see cref="BackgroundPlayType.PlayRandomly"/> can be seen in Minecraft.</br>
    /// </summary>
    public enum BackgroundPlayType
    {
        Triggered,
        PlayRandomly,
    }
}
