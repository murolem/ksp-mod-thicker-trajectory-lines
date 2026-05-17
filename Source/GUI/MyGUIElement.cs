using JetBrains.Annotations;
using UnityEngine;

namespace ThickerTrajectoryLines
{
    /// <summary>
    /// Wrapper around basic controls.
    /// </summary>
    public interface MyGUIElement
    {
        /// <summary>
        /// Current style. Initially `null` since it can only be set while in OnGUI event.
        /// </summary>
        GUIStyle Style { get; }
        
        /// <summary>
        /// Ensures that `Style` (1) has a value and (2) the value is appropriate for that type of control.
        ///
        /// By default, uses the default KSP style for that element and respect the `Scaling` in `MyGUI`.
        ///
        /// Can only be called while in OnGUI event.
        ///
        /// Calling this again will return previously created style.
        /// </summary>
        /// <returns>Resulting style.</returns>
        GUIStyle EnsureStyle();
        
        /// <summary>
        /// Render the control. Can only be called while in OnGUI event.
        ///
        /// Some controls can return values, but they are not returned by this method.
        /// Instead, they can be accessible via relevant for controls fields (eg `Toggle`, `Pressed`, etc). 
        /// </summary>
        /// <param name="styleOverride">If not `null`, this style will be used instead of `Style`. If `null`, uses `Style` by calling `EnsureStyle`.</param>
        /// <param name="options">GUI options to pass along to the control.</param>
        void Draw(GUIStyle styleOverride = null, params GUILayoutOption[] options);
    }
}