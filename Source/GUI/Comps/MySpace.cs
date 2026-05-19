using UnityEngine;

namespace ThickerTrajectoryLines
{
    // haha myspace
    public class MySpace : MyGUIElement
    {
        public GUIStyle Style { get; private set; }
        
        private int pixels;
        public MySpace(int pixels)
        {
            this.pixels = pixels;
        }
        
        // not used here
        public GUIStyle EnsureStyle()
        {
            this.Style = this.Style ?? MyGUI.MakeGUIStyle(GUI_STYLE_NAME.box);
            return this.Style;
        }
        
        // options not used here
        public void Draw(GUIStyle styleOverride = null, params GUILayoutOption[] options)
        {
            GUILayout.Space(this.pixels * 2);
        }
        
        public void Destroy() { }
    }
}