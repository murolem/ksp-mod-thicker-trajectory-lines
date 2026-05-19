using System;
using System.Collections.Generic;
using UnityEngine;

namespace ThickerTrajectoryLines
{
    public class MySlider : MyGUIElement
    {
        public GUIStyle Style { get; private set; }
        public GUIStyle ThumbStyle { get; private set; }
        public float Value { get; private set; }
        public event Action<float> ValueChanged;
        
        private string label;
        private float min;
        private float max;
        private float step;
        
        private MyLabel myNameLabel;
        private MyLabel myValueLable;
        private MyLabel myMinLabel;
        private MyLabel myMaxLabel;
        public MySlider(string label, float value, float min, float max, float step = Single.Epsilon)
        {
            this.label = label;
            this.Value = value;
            this.min = min;
            this.max = max;
            this.step = step;
            
            this.myNameLabel = new MyLabel(label);
            this.myValueLable = new MyLabel("value: " + value.ToString("R"));
            this.myMinLabel = new MyLabel("min: " + min.ToString("R"));
            this.myMaxLabel = new MyLabel("max: " + max.ToString("R"));
        }
        
        public GUIStyle EnsureStyle()
        {
            if (this.Style == null || this.ThumbStyle == null)
            {
                this.Style = MyGUI.MakeGUIStyle(GUI_STYLE_NAME.horizontalSlider);
                this.ThumbStyle = MyGUI.MakeGUIStyle(GUI_STYLE_NAME.horizontalSliderThumb);
            }
            
            return this.Style;
        }

        public void Draw(GUIStyle styleOverride = null, params GUILayoutOption[] options)
        {
            this.Draw(styleOverride, null, options);
        }

        public void Draw(GUIStyle styleOverride = null, GUIStyle thumbStyleOverride = null, params GUILayoutOption[] options)
        {
            EnsureStyle();
            
            var sliderStyle = styleOverride ?? this.Style;
            // handle margin separatly
            var sliderMargin = MyGUI.CloneRectOffset(sliderStyle.margin);
            // only leave the horizontal margin since it can be safely applied to the slider without shifting labels around
            sliderStyle.margin = MyGUI.RectOffset(0f, sliderMargin.left);
            
            var numLabelStyle = myValueLable.EnsureStyle();
            numLabelStyle.fontSize = Mathf.RoundToInt(MyGUI.FontSize * 0.9f);;
            numLabelStyle.normal.textColor = Color.HSVToRGB(0f, 0f, .85f);
            
            GUILayout.Space(sliderMargin.top);
            
            // text and value labels
            GUILayout.BeginHorizontal();
            GUILayout.Space(sliderMargin.left);
            this.myNameLabel.Draw();
            GUILayout.FlexibleSpace();
            this.myValueLable.Draw(numLabelStyle);
            GUILayout.Space(sliderMargin.right);
            GUILayout.EndHorizontal();
            
            // slider
            
            var sliderThumbStyle = thumbStyleOverride ?? this.ThumbStyle;;
        
            var sliderHeight = 18f * MyGUI.Scale;
            
            var optsAugmented = new GUILayoutOption[options.Length + 1];
            optsAugmented[0] = GUILayout.Height(sliderHeight);
            Array.Copy(options, 0, optsAugmented, 1, options.Length);
            
            sliderStyle.fixedHeight = sliderHeight;
            sliderThumbStyle.fixedHeight = sliderHeight;
            sliderThumbStyle.fixedWidth = sliderHeight;
            
            var oldValue = Value;
            var newValue = GUILayout.HorizontalSlider(Value, min, max, sliderStyle, sliderThumbStyle, optsAugmented);
            newValue = MathfUtils.RoundStep(newValue, step);
            if (newValue != oldValue)
            {
                this.Value = newValue;
                // round smallest values where precious errors happen so that we dont display them,
                // while still displaying small enough values.
                myValueLable.Set("value: " + newValue.ToString("G7"));
                ValueChanged?.Invoke(newValue);
            }
            
            // min and max labels
            GUILayout.BeginHorizontal();
            GUILayout.Space(sliderMargin.left);
            this.myMinLabel.Draw(numLabelStyle);
            GUILayout.FlexibleSpace();
            this.myMaxLabel.Draw(numLabelStyle);
            GUILayout.Space(sliderMargin.right);
            GUILayout.EndHorizontal();
            
            GUILayout.Space(sliderMargin.bottom);
        }

        public MySlider OnValueChanged(Action<float> listener)
        {
            ValueChanged += listener;
            listener(Value); // for hot reload
            return this;
        }
        
        public void Destroy()
        {
            ValueChanged = null;
        }
    }
}