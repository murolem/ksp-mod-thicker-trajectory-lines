using System;
using UnityEngine;

namespace ThickerTrajectoryLines
{
    public class MyButton : MyGUIElement
    {
        public GUIStyle Style { get; private set; }
        public string Text { get; private set; }
        
        /** Whether the button is currently pressed. */
        public bool IsPressed { get; private set; }

        public Action Pressed;

        private GUILayoutOption[] options;
        
        public MyButton(string text, params GUILayoutOption[] options)
        {
            this.Text = text;
            this.options = options;
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
                OnScaleChanged(SettingsGUI.Instance.Scale);
                SettingsGUI.Instance.ScaleChanged += OnScaleChanged;
            }
            return this.Style;
        }
        
        public void Draw(GUIStyle styleOverride = null, params GUILayoutOption[] options)
        {
            EnsureStyle();

            var style = styleOverride ?? this.Style;

            var optionsCombined = new GUILayoutOption[this.options.Length + options.Length];
            this.options.CopyTo(optionsCombined, 0);
            options.CopyTo(optionsCombined, this.options.Length);
            
            var wasPressedBefore = IsPressed;
            IsPressed = GUILayout.Button(this.Text, style, optionsCombined);
            if (IsPressed && !wasPressedBefore)
            {
                Pressed?.Invoke();
            }
        }
        
        public MyButton Set(string text)
        {
            this.Text = text;
            return this;
        }
        
        public MyButton OnPressed(Action listener)
        {
            Pressed += listener;
            return this;
        }

        public void Destroy()
        {
            Pressed = null;
        }
    }
}