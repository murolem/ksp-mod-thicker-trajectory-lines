using UnityEngine;

namespace ThickerTrajectoryLines
{
    public class MyLabel : MyGUIElement
    {
        private string text;
        public MyLabel(string text)
        {
            this.text = text;
        }

        public void Draw(GUIStyle guiStyle)
        {
            GUILayout.Label(text, guiStyle);
        }

        public MyLabel Set(string text)
        {
            this.text = text;
            return this;
        }
    }
}