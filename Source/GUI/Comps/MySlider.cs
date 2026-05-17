using System;
using System.Collections.Generic;
using UnityEngine;

namespace ThickerTrajectoryLines
{
    public class MySlider : MyGUIElement
    {
        public Action<float> ValueChanged;
        
        private string label;
        private float value;
        private float min;
        private float max;
        private float step;
        
        private MyLabel textLabelEl;
        private MyLabel valueLabelEl;
        private MyLabel minLabelEl;
        private MyLabel maxLabelEl;
        
        public MySlider(string label, float value, float min, float max, float step = Single.Epsilon)
        {
            this.label = label;
            this.value = value;
            this.min = min;
            this.max = max;
            this.step = step;
            
            this.textLabelEl = new MyLabel(label);
            this.valueLabelEl = new MyLabel(value.ToString("R"));
            this.minLabelEl = new MyLabel(min.ToString("R"));
            this.maxLabelEl = new MyLabel(max.ToString("R"));
        }

        public void Draw(GUIStyle guiStyle)
        {
            // text and value labels
            GUILayout.BeginHorizontal();
            this.textLabelEl.Draw(guiStyle);
            GUILayout.FlexibleSpace();
            this.valueLabelEl.Draw(guiStyle);
            GUILayout.EndHorizontal();
            
            // slider
            var oldValue = value;
            // todo scale
            var newValue = GUILayout.HorizontalSlider(value, min, max);
            newValue = MathfUtils.RoundStep(newValue, step);
            if (newValue != oldValue)
            {
                this.value = newValue;
                // round smallest values where precious errors happen so that we dont display them,
                // while still displaying small enough values.
                valueLabelEl.Set(newValue.ToString("G7"));
                ValueChanged?.Invoke(newValue);
            }
            
            // min and max labels
            GUILayout.BeginHorizontal();
            this.minLabelEl.Draw(guiStyle);
            GUILayout.FlexibleSpace();
            this.maxLabelEl.Draw(guiStyle);
            GUILayout.EndHorizontal();
        }
    }
}