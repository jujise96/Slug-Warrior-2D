using UnityEngine;

namespace Game
{
    /// <summary>
    /// Custom Attribute that displays the variable in the inspector but it cannot be changed
    /// </summary>
    public class ReadOnlyAttribute : PropertyAttribute { }
}
