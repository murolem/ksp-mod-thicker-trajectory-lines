using System;
using UnityEngine;

namespace ThickerTrajectoryLines
{
    public class MyHr : MyGUIElement
    {
        private GUIStyle _style;
        public GUIStyle Style { get => _style; private set => _style = value; }
        
        private static Texture2D tex = MyGUI.MakeTex(2, 2, MyGUI.HSVColor(180f, 70f, 90f));

        public GUIStyle EnsureStyle()
        {
            if (this.Style == null)
            {
                this.Style = MyGUI.MakeGUIStyle(GUI_STYLE_NAME.box);
                this.Style.padding = MyGUI.RectOffset(0f);
                this.Style.margin = MyGUI.RectOffset(0f);
                this.Style.normal.background = tex;
                
                void OnScaleChanged(float newScale)
                {
                    var height = 2f * newScale;
                    this.Style.border.top = Mathf.RoundToInt(height);
                    this.Style.border.bottom = Mathf.RoundToInt(height);
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
            var height = style.border.top;
            GUILayout.Box("", style, GUILayout.ExpandWidth(true), GUILayout.Height(height));
        }
        
        public void Destroy() { }
    }
}