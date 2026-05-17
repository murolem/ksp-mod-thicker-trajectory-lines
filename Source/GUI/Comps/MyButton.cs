using UnityEngine;

namespace ThickerTrajectoryLines
{
    public class MyButton : MyGUIElement
    {
        private string text;
        public MyButton(string text)
        {
            this.text = text;
        }
        
        public void Draw(GUIStyle guiStyle)
        {
            var style = new GUIStyle(GUI.skin.button);
            style.fontSize = guiStyle.fontSize;
            // todo scale
            GUILayout.Button(this.text, style);
        }
    }
}