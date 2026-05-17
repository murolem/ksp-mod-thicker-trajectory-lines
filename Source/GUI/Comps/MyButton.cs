using System;
using UnityEngine;

namespace ThickerTrajectoryLines
{
    public class MyButton : MyGUIElement
    {
        public GUIStyle Style { get; private set; }
        public string Text { get; private set; }
        
        /** Whether the button is currently pressed. */
        public bool Pressed { get; private set; }

        public Action OnPressed;
        
        public MyButton(string text)
        {
            this.Text = text;
        }

        public GUIStyle EnsureStyle()
        {
            if (this.Style == null)
            {
                this.Style = MyGUI.MakeGUIStyle(GUI_STYLE_NAME.button);

                void OnScaleChanged(float newScale)
                {
                    this.Style.fontSize = MyGUI.FontSize;
                }
                OnScaleChanged(MyGUI.Scale);
                MyGUI.ScaleChanged += OnScaleChanged;
            }
            return this.Style;
        }
        
        public void Draw(GUIStyle styleOverride = null, params GUILayoutOption[] options)
        {
            EnsureStyle();

            var style = styleOverride ?? this.Style;
            
            var wasPressedBefore = Pressed;
            Pressed = GUILayout.Button(this.Text, style, options);
            if (Pressed && !wasPressedBefore)
            {
                OnPressed?.Invoke();
            }
        }
        
        public MyButton Set(string text)
        {
            this.Text = text;
            return this;
        }
    }
}