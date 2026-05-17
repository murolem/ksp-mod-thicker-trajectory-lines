using System;
using UnityEngine;

namespace ThickerTrajectoryLines
{
    public class MyCheckbox : MyGUIElement
    {
        private string label;
        private bool toggle;
        
        public Action<bool> ValueChanged;
        
        public MyCheckbox(string label, bool toggle)
        {
            this.label = label;
            this.toggle = toggle;
        }

        public void Draw(GUIStyle guiStyle)
        {
            var oldValue = toggle;
            var style = new GUIStyle(GUI.skin.toggle);
            style.fontSize = guiStyle.fontSize;
            // todo scale
            var newValue = GUILayout.Toggle(toggle, label, style); 
            toggle = newValue;
            
            if (newValue != oldValue)
            {
                ValueChanged?.Invoke(newValue);
            }
        }
    }
}