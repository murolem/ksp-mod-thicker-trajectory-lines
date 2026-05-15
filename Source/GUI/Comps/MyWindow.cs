using System.Collections.Generic;
using ClickThroughFix;
using KSP.UI;
using UnityEngine;

namespace ThickerTrajectoryLines
{
    public class MyWindow
    {
        private static int windowsCreatedCounter = 0;
        
        public GUIStyle guiStyle { get; private set; }
        
        private int id;
        private string header;
        private Rect rect;
        private List<MyGUIElement> children =  new ();
        private MySpace spacer;
        
        public MyWindow(string header, int x, int y, int width, int height)
        {
            this.id = 60273460 + windowsCreatedCounter++;
            this.rect = new Rect(x, y, width, height);
            this.header = header;
            this.spacer = new MySpace(5);
        }

        // todo implement
        public void Dock()
        {
            throw new System.NotImplementedException();
        }

        public MyWindow Center()
        {
            rect.x = Screen.width / 2f - rect.width / 2f;
            rect.y = Screen.height / 2f - rect.height / 2f;
            return this;
        }

        public void Draw()
        {
            if (this.guiStyle == null)
            {
                this.guiStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            }
            
            rect = ClickThruBlocker.GUILayoutWindow(id, rect, _Draw, header);
        }

        public MyWindow Append(params MyGUIElement[] elements)
        {
            children.AddRange(elements);
            return this;
        }

        private void _Draw(int id)
        {
            for (int i = 0; i < children.Count; i++)
            {
                var element = children[i];
                element.Draw(guiStyle);

                // add padding between each element but not after last
                if (i < children.Count - 1)
                {
                    spacer.Draw(guiStyle);
                }
            }
            // todo account for scale in height
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }
        
        public void SetScale(float newValue)
        {
            // a random value I found in ksp source code. idk how to get actual font size
            int DefaultFontSize = 14;
            this.guiStyle.fontSize = Mathf.RoundToInt(DefaultFontSize * GameSettings.UI_SCALE);
        }
    }
}