using UnityEngine;

namespace ThickerTrajectoryLines
{
    public class MyLabel : MyGUIElement
    {
        private GUIStyle _style;
        public GUIStyle Style { get => _style; private set => _style = value; }
        
        public string Text { get; private set; }
        public MyLabel(string text)
        {
            this.Text = text;
        }
        
        public GUIStyle EnsureStyle()
        {
            if (this.Style == null)
            {
                this.Style = MyGUI.MakeGUIStyle(GUI_STYLE_NAME.label);
                // this.Style.padding = MyGUI.RectOffset(0);
                // this.Style.margin = MyGUI.RectOffset(0);
                this.Style.normal.textColor = Color.white;
                
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
            GUILayout.Label(Text, style, options);
        }

        public MyLabel Set(string text)
        {
            this.Text = text;
            return this;
        }
        
        public void Destroy() { }
    }
}