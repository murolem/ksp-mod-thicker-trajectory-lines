using UnityEngine;

namespace ThickerTrajectoryLines
{
    public class MySection : MyGUIElement
    {
        public GUIStyle Style { get; private set; }
        public string Text { get; private set; }

        private MyHr myHr;
        private MyLabel myLabel;
        public MySection(string label)
        {
            this.myHr = new MyHr();
            this.myLabel = new MyLabel(label);
        }

        public GUIStyle EnsureStyle()
        {
            if (this.Style == null)
            {
                this.Style = MyGUI.MakeGUIStyle(GUI_STYLE_NAME.box);
                
                var labelStyle = this.myLabel.EnsureStyle();
                labelStyle.fontStyle = FontStyle.Bold;
                labelStyle.normal.textColor = MyGUI.defaultSkin.label.normal.textColor;
                
                var hrStyle = this.myHr.EnsureStyle();
                
                void OnScaleChanged(float newScale)
                {
                    labelStyle.fontSize = Mathf.RoundToInt(MyGUI.FontSize * 1.15f);
                    hrStyle.margin.bottom = Mathf.RoundToInt(8f * newScale);
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
            
            GUILayout.BeginHorizontal();
            
            GUILayout.Space(-10f);
            
            GUILayout.BeginVertical();
            this.myLabel.Draw(null, options);
            this.myHr.Draw(null, options);
            GUILayout.EndVertical();
            
            GUILayout.Space(-10f);
            
            GUILayout.EndHorizontal();
        }
        
        public MySection Set(string text)
        {
            this.Text = text;
            return this;
        }
        
        public void Destroy() { }
    }
}