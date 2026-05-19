using System;
using UnityEngine;

namespace ThickerTrajectoryLines
{
    public class MyToggle : MyGUIElement
    {
        public GUIStyle Style { get; private set; }
        
        private string label;
        private MyLabel myLabel;
        
        public bool Toggle;
        public event Action<bool> ValueChanged;
        
        public MyToggle(string label, bool toggle)
        {
            this.label = label;
            this.Toggle = toggle;

            this.myLabel = new MyLabel(label);
        }
        
        public GUIStyle EnsureStyle()
        {
            if (this.Style == null)
            {
                this.Style = MyGUI.MakeGUIStyle(GUI_STYLE_NAME.toggle);
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
            
            // we gonna render the box and the label separately
            // since the fuckass unity doesnt support size for the box itself 

            var toggleStyle = styleOverride ?? this.Style;;
            
            var margin = toggleStyle.margin;
            margin.right = 0;
            var height = 18f * MyGUI.Scale;
            
            // reset whatever offsets values the toggle had
            toggleStyle.border = new RectOffset(0, 0, 0, 0);
            toggleStyle.overflow = new RectOffset(0, 0, 0, 0);
            toggleStyle.padding = new RectOffset(0, 0, 0, 0); 
            
            // get rid of label part
            toggleStyle.imagePosition = ImagePosition.ImageOnly;
            
            // set box size
            toggleStyle.fixedHeight = height;
            toggleStyle.fixedWidth = height;

            var optsAugmented = new GUILayoutOption[options.Length + 1];
            optsAugmented[0] = GUILayout.Height(height);
            Array.Copy(options, 0, optsAugmented, 1, options.Length);
            
            // do some padding/margin resets below and adjust the margin given by our god and saviour (parent caller).
            // resets are needed bcs theres some intrinsic offset somewhere and idk where and I dont want to search for it.
            
            GUILayout.BeginHorizontal();
            
            var oldToggled = Toggle;
            toggleStyle.padding = MyGUI.RectOffset(0);
            toggleStyle.margin = MyGUI.RectOffset(toggleStyle.margin.left, 0, 0, 0);
            var newToggled = GUILayout.Toggle(Toggle, "", toggleStyle, optsAugmented);
            
            // render label
            var labelBtn = new MyButton(this.myLabel.Text);
            // use label style not the button style since we want label like appearance
            var labelStyle = this.myLabel.EnsureStyle();
            labelStyle.padding = MyGUI.RectOffset(0);
            labelStyle.margin = MyGUI.RectOffset(0, labelStyle.margin.right, 0, 0);
            labelStyle.hover = labelStyle.normal;
            labelStyle.active = labelStyle.normal;
            labelBtn.Draw(labelStyle, optsAugmented);
            // press = flip
            var flipToggle = labelBtn.IsPressed;
            
            GUILayout.EndHorizontal();
            
            if(flipToggle)
                newToggled = !newToggled;

            if (newToggled != oldToggled)
            {
                Toggle = newToggled;
                ValueChanged?.Invoke(newToggled);
            }
        }
        
        public MyToggle OnValueChanged(Action<bool> listener)
        {
            ValueChanged += listener;
            listener(Toggle); // for hot reload
            return this;
        }
        
        public void Destroy()
        {
            ValueChanged = null;
        }
    }
}