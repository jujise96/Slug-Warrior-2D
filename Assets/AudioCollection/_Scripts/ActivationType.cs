using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// The activation type of any object. This can be applied to any type of object and is up to for you to decide what "Enable" and "Disable" might mean! 
    /// However, just be sure that you also allow it to be both Enabled and Disabled at the same time! (If not, you can simply do a check during OnValidate()!)
    /// </summary>
    [System.Serializable]
    public enum ActivationType
    {
        Enable,
        Disable,
        EnableAndDisable,
    }
}
