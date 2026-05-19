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
        /// Element style. Some elements (like the slider) can have some extra styles in their class (like the thumb style, on the slider).
        ///
        /// Initially `null` (since it can only be set while in OnGUI event), set to some style while the element is being drawn.
        /// Can be set and returned early by calling `EnsureStyle` while in OnGUI.
        /// </summary>
        GUIStyle Style { get; }
        
        /// <summary>
        /// Ensures that `Style` (1) has a value and (2) the value is appropriate for that type of control.
        ///
        /// By default, uses the default KSP style for that element with respect to the `Scaling` in `MyGUI`.
        ///
        /// Can only be called while in OnGUI event.
        ///
        /// Calling this again will return previously created style.
        /// </summary>
        /// <returns>Resulting style.</returns>
        GUIStyle EnsureStyle();
        
        /// <summary>
        /// Renders the control. Can only be called while in OnGUI.
        ///
        /// Some controls can return values, but they are not returned by this method.
        /// Instead, they can be accessible via relevant for controls fields (eg `Toggle`, `Pressed`, etc). 
        /// </summary>
        /// <param name="styleOverride">If not `null`, the given style will be used instead of the element's `Style`.
        /// If `null`, uses the element's `Style` by calling `EnsureStyle`.</param>
        /// <param name="options">GUI options to pass along to the control.</param>
        void Draw(GUIStyle styleOverride = null, params GUILayoutOption[] options);

        /// <summary>
        /// Meant for cleanup after component is no longer needed and will no longer be used.
        ///
        /// Each component is specifically expected to:
        /// - Remove any event subscribers.
        /// </summary>
        void Destroy();
    }
}