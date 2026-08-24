using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Audio
{
    /// <summary>
    /// The state of Background Audio.
    /// <br>--> <see cref="BackgroundAudioState.Unused"/> means the Background Audio is not being played or not being used.</br>
    /// <br>--> <see cref="BackgroundAudioState.Queued"/> means the Background Audio is being ready to be played, but is NOT playing. 
    /// This is ONLY used for Background Audio played at random intervals.</br>
    /// <br>--> <see cref="BackgroundAudioState.Playing"/> means the Background Audio is actively being played.</br>
    /// </summary>
    public enum BackgroundAudioState
    { 
        Unused,
        Queued,
        Playing,
    }
}
