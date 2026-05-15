using UnityEngine;

namespace ThickerTrajectoryLines
{
    // haha myspace
    public class MySpace : MyGUIElement
    {
        private int pixels;
        public MySpace(int pixels)
        {
            this.pixels = pixels;
        }
        
        public void Draw(GUIStyle guiStyle)
        {
            GUILayout.Space(this.pixels * GameSettings.UI_SCALE);
        }
    }
}