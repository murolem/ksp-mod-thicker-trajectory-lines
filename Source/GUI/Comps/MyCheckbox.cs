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
            var newValue = GUILayout.Toggle(toggle, label, guiStyle); 
            toggle = newValue;
            
            if (newValue != oldValue)
            {
                ValueChanged?.Invoke(newValue);
            }
        }
    }
}