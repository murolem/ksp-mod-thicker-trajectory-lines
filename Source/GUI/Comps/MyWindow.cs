using System;
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
        private MyLabel titleLabel;
        private MySpace spacer;
        private MyButton resizeDragButton;
        
        public MyWindow(string header, int x, int y, int width, int height)
        {
            this.id = 60273460 + windowsCreatedCounter++;
            this.rect = new Rect(x, y, width, height);
            this.header = header;
            this.titleLabel = new MyLabel(header);
            this.spacer = new MySpace(5);
            this.resizeDragButton = new MyButton("#");
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
            // set style on first call
            if (this.guiStyle == null)
            {
                this.guiStyle = new GUIStyle();
                SetScale(GameSettings.UI_SCALE);
            }
            
            rect = ClickThruBlocker.GUILayoutWindow(id, rect, _Draw, "", GUIStyle.none, GUILayout.ExpandWidth(true));
        }

        public MyWindow Append(params MyGUIElement[] elements)
        {
            children.AddRange(elements);
            return this;
        }

        private void _Draw(int id)
        {
            // title
            
            var titleStyle =  new GUIStyle(this.guiStyle);
            // Add these lines:
            titleStyle.stretchWidth = true;  // Allow stretching
            titleStyle.fixedWidth = 0;       // No fixed width constraint
            titleStyle.wordWrap = false;
            titleStyle.alignment = TextAnchor.UpperCenter;
            titleStyle.normal.background = MakeTex( 2, 2, new Color( 0f, 1f, 0f, 0.5f ) );
            int titlePadding = Mathf.RoundToInt(5f * GameSettings.UI_SCALE);
            titleStyle.margin = new RectOffset(titlePadding, titlePadding, titlePadding, titlePadding);
            GUILayout.BeginHorizontal(titleStyle, GUILayout.Height(this.guiStyle.fontSize + titlePadding * 2));
            
            this.titleLabel.Draw(titleStyle);
            GUILayout.EndHorizontal();
            
            // content
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

            // this little block of code apparently makes the window auto resize itself (?) glad I dont have to code that
            // GUILayout.BeginHorizontal();
            // GUILayout.FlexibleSpace();
            // GUILayout.EndHorizontal();
            
            // todo account for scale in height
            GUI.DragWindow(new Rect(0, 0, 10000, this.guiStyle.fontSize));
        }
        
        private Texture2D MakeTex( int width, int height, Color col )
        {
            Color[] pix = new Color[width * height];
            for( int i = 0; i < pix.Length; ++i )
            {
                pix[ i ] = col;
            }
            Texture2D result = new Texture2D( width, height );
            result.SetPixels( pix );
            result.Apply();
            return result;
        }
        
        public void SetScale(float newValue)
        {
            // a random value I found in ksp source code. idk how to get actual font size
            int defaultFontSize = 14;
            this.guiStyle.fontSize = Mathf.RoundToInt(defaultFontSize * newValue);
            // title container height
            this.guiStyle.border.top = this.guiStyle.fontSize;
        }
    }
}