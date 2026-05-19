using System;
using System.Collections.Generic;
using ClickThroughFix;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ThickerTrajectoryLines
{
    // do not implement the mygui interface since it requires passing a style to draw
    // which we dont want for this upper level element
    public class MyWindow
    {
        private static int windowsCreatedCounter = 0;

        public bool Visible
        {
            get => this.showStage != ShowStage.HIDDEN;
        }
        
        private int id;
        private int baseWidth;
        private int baseHeight;
        private Rect rect;
        private List<MyGUIElement> children = new List<MyGUIElement>();
        private bool firstDrawDone = false;

        private GUIStyle style;
        private MyLabel titleLabel;
        private MyButton closeButton;
        private Rect dragRect;

        private GUIStyle titleContainerStyle;
        private GUIStyle contentContainerStyle;
        
        public MyWindow(string title, int x, int y, int width, int height)
        {
            this.id = 60273460 + windowsCreatedCounter++;
            this.baseWidth = width;
            this.baseHeight = height;
            this.rect = new Rect(x, y, width, height);
            this.titleLabel =  new MyLabel(title);
            this.closeButton = new MyButton("x");
            this.dragRect = new Rect(0, 0, 10000, 0);
            
            this.closeButton.Pressed += () => Hide();
        }

        public Rect GetRectCopy()
        {
            return rect;
        }

        private MyWindow EnsureStyles()
        {
            var titlePaddingBase = MyGUI.RectOffset(5f);
            var contentPaddingBase = MyGUI.RectOffset(10f, 25f);
            
            titleLabel.EnsureStyle();
            titleLabel.Style.normal.textColor = Color.white;
            titleLabel.Style.wordWrap = false;
            titleLabel.Style.alignment = TextAnchor.MiddleCenter;
            titleLabel.Style.fontStyle = FontStyle.Bold;
            
            closeButton.EnsureStyle();
            closeButton.Style.fontStyle = FontStyle.Bold;

            this.titleContainerStyle = MyGUI.MakeGUIStyle(GUI_STYLE_NAME.box);
            this.titleContainerStyle.padding = MyGUI.RectOffset(0);
            this.titleContainerStyle.margin = MyGUI.RectOffset(0);

            this.contentContainerStyle = MyGUI.MakeGUIStyle(GUI_STYLE_NAME.box);
            this.contentContainerStyle.padding = MyGUI.RectOffset(0);
            this.contentContainerStyle.margin = MyGUI.RectOffset(0);
            
            // trigger here for initial calculation
            OnScaleChanged(SettingsGUI.Instance.Scale);
            
            void OnScaleChanged(float newScale)
            {
                var newWidth = baseWidth * newScale;
                var newHeight = baseHeight * newScale;
                var horizOffsetDueToResize = (newWidth - this.rect.width) / 2f;
                var vertOffsetDueToResize = (newHeight - this.rect.height) / 2f;
                this.rect.x -= horizOffsetDueToResize;
                // this.rect.y -= horizOffsetDueToResize;
                this.rect.width = newWidth;
                this.rect.height = newHeight;
                
                this.titleContainerStyle.padding = MyGUI.ScaleRectOffset(titlePaddingBase, this.titleContainerStyle.padding);
                this.contentContainerStyle.padding = MyGUI.ScaleRectOffset(contentPaddingBase, contentContainerStyle.padding);
                
                titleLabel.Style.fontSize = Mathf.RoundToInt(MyGUI.FontSize * 1.25f);

                closeButton.Style.fontSize = Mathf.RoundToInt(MyGUI.FontSize * 2f);
                
                this.dragRect.height = titleLabel.Style.CalcSize(new GUIContent(titleLabel.Text)).y +
                                       titleContainerStyle.padding.vertical * 2f;
            }
            SettingsGUI.Instance.ScaleChangedLowPriority += OnScaleChanged;
            
            return this;
        }
        
        public MyWindow Append(params MyGUIElement[] elements)
        {
            children.AddRange(elements);
            return this;
        }

        enum ShowStage
        {
            HIDDEN,
            PENDING_OFFSCREEN_DRAW1,
            PENDING_OFFSCREEN_DRAW2,
            PENDING_DOCKING,
            VISIBLE,
        }

        private ShowStage showStage = ShowStage.HIDDEN;
        private Rect afterDockRect;

        public MyWindow Toggle()
        {
            if (Visible)
            {
                Hide();
            }
            else
            {
                Show();
            }

            return this;
        }
        
        public MyWindow Show()
        {
            // Debug.Log("Show requested");
            
            // if rect changed after dock = user moved the window = do not force the dock position over user choice.
            if (afterDockRect != default && (rect.x != afterDockRect.x || afterDockRect.y != afterDockRect.y))
            {
                showStage = ShowStage.VISIBLE;    
            }
            else
            {
                showStage = ShowStage.PENDING_OFFSCREEN_DRAW1;
            }
            return this;
        }
        
        public MyWindow Hide()
        {
            // Debug.Log("Hide requested");
            showStage = ShowStage.HIDDEN;
            return this;
        }

        // !must remain clockwise
        enum ToolbarStateDock
        {
            LEFT,
            TOP,
            RIGHT,
            BOTTOM
        }
        
        struct ToolbarState
        {
            public ToolbarStateDock dockSide;
            public Rect toolbarButtonRect;
        }

        private ToolbarState GuessToolbarState()
        {
            var toolbarBtnRect = SettingsGUI.ToolbarControl.StockPosition 
                ?? SettingsGUI.ToolbarControl.BlizzyPosition 
                ?? new Rect(0f, 0f, Screen.width, Screen.height / 2f);

            var toolbarBtnCenter = new Vector2(
                toolbarBtnRect.x + toolbarBtnRect.width / 2f,
                toolbarBtnRect.y + toolbarBtnRect.height / 2f
            );
            // Debug.Log("toolbarBtnCenter: " + toolbarBtnCenter);
            var vecToToolbarBtnCenter = toolbarBtnCenter - new Vector2(Screen.width / 2f, Screen.height / 2f);
            // Debug.Log("vecToToolbarBtnCenter: " + vecToToolbarBtnCenter);
            var angleRadSigned = Mathf.Atan2(vecToToolbarBtnCenter.y, vecToToolbarBtnCenter.x);
            // Debug.Log("angleRadSigned: " + angleRadSigned);
            // begins CW at (-1, 0)
            // take the signed angle, make it unsigned.
            // by doing this, the vector will be 0 at (-1, 0), which is the zero index for the dock enum.
            var angleRadUnsignedSectored = angleRadSigned + Math.PI;
            // Debug.Log("angleRadUnsignedSectored: " + angleRadUnsignedSectored);
            // then just mod it by 1/4 of a circle and floor it to get the dock index.
            // this will get us a pie sector, indexed 0 to 3. 
            // in case my math if fucked up, make it safe positive integer 0-3 so that it's always a valid index.
            var sector = Math.Ceiling(angleRadUnsignedSectored / (Math.PI / 2));
            // Debug.Log("sector: " + sector);
            var dock = (ToolbarStateDock)(Math.Floor(Math.Abs(sector % 4)));
            // Debug.Log("dock: " + dock);

            return new ToolbarState(){
                dockSide = dock,
                toolbarButtonRect = toolbarBtnRect
            };
        }
        
        /// <summary>
        /// Move near the toolbar button.
        /// </summary>
        /// <returns></returns>
        public MyWindow Dock()
        {
            var toolbarState = GuessToolbarState();
            var gap = 5f;

            var offsetVec = new Vector2();
            switch (toolbarState.dockSide)
            {
                case ToolbarStateDock.RIGHT:
                    offsetVec.x = -1 * (gap + rect.width);
                    break;
                case ToolbarStateDock.BOTTOM:
                    offsetVec.y = -1 * (gap + rect.height);
                    break;
                case ToolbarStateDock.TOP:
                    // assuming top left start going right
                    offsetVec.x += toolbarState.toolbarButtonRect.width + gap;
                    offsetVec.y += toolbarState.toolbarButtonRect.height + gap;
                    break;
                case ToolbarStateDock.LEFT:
                    // assuming bottom left start going up
                    offsetVec.x += toolbarState.toolbarButtonRect.width + gap;
                    break;
            }

            // offset by the offset vec and compensate if we get offscreen
            rect.x = Mathf.Clamp(toolbarState.toolbarButtonRect.x + offsetVec.x, rect.width, Screen.width - rect.width);
            rect.y = Mathf.Clamp(toolbarState.toolbarButtonRect.y + offsetVec.y, rect.width, Screen.height - rect.height);
            afterDockRect = new Rect(rect);
            // Debug.Log("Via docking rect set to: " + rect.ToString());

            return this;
        }
        
        public MyWindow PositionAndResize(Rect fromRect)
        {
            rect = fromRect;
            return this;
        }

        public MyWindow Center()
        {
            rect.x = Screen.width / 2f - rect.width / 2f;
            rect.y = Screen.height / 2f - rect.height / 2f;
            return this;
        }

        public void Draw()
        {
            // Debug.Log("Draw called, show stage: " + showStage + " Rect: " + rect.ToString());
            
            if (!this.Visible)
            {
                return;
            }
            
            switch (this.showStage)
            {
                // off screen pending dock is a pre visible stage.
                // here we figure out the rect's size by doing an offscreen draw. 
                case ShowStage.PENDING_OFFSCREEN_DRAW1:
                    this.rect.x = 10000;
                    break;
                // once off screen draw is done we can begin docking
                case ShowStage.PENDING_DOCKING:
                    Dock();
                    showStage = ShowStage.VISIBLE;
                    break;
            }
            
            // Debug.Log("(After showStage switch) Show stage: " + showStage + " Rect: " + rect.ToString());
            
            if (!firstDrawDone)
            {
                style = MyGUI.MakeGUIStyle(GUI_STYLE_NAME.window);
                style.padding = MyGUI.RectOffset(0);
                style.margin = MyGUI.RectOffset(0);
                style.stretchWidth = true;
            }
            
            // do not put title here, draw it as a separate label instead since the normal one can't scale properly
            rect = GUILayout.Window(id, rect, _Draw, "", style, GUILayout.Width(rect.width), GUILayout.ExpandWidth(true));
            
            // scale if possible (when user lets go of the scale slider)
            if (SettingsGUI.Instance.Scale != SettingsGUI.Instance.DirtyScale && !Input.GetMouseButton(0))
            {
                SettingsGUI.Instance.SetScale(SettingsGUI.Instance.DirtyScale);
            }
        }
        
        private void _Draw(int id)
        {
            if (!this.Visible)
            {
                return;
            }
            
            if (!firstDrawDone)
            {
                this.EnsureStyles();
            }
            
            // title
            GUILayout.BeginHorizontal(titleContainerStyle);
            
            GUILayout.BeginVertical();
            GUILayout.Space(5f * SettingsGUI.Instance.Scale);
            this.titleLabel.Draw(null, GUILayout.ExpandWidth(true));
            GUILayout.Space(5f * SettingsGUI.Instance.Scale);
            GUILayout.EndVertical();

            var btnTextSize = closeButton.Style.CalcSize(new GUIContent(closeButton.Text));
            this.closeButton.Draw(null, GUILayout.Width(btnTextSize.y), GUILayout.Height(btnTextSize.y));

            GUILayout.EndHorizontal();
            
            // content
            GUILayout.BeginVertical(contentContainerStyle);
            var firstSectionPassed = false;
            var firstSectionItemPassed = false;
            for (int i = 0; i < children.Count; i++)
            {
                var element = children[i];
                var elementType = element.GetType();
                var isFirst = i == 0;
                var isLast =  i == children.Count - 1;
                
                // add extra spacing between sections when at 2nd+ section
                if (elementType == typeof(MySection))
                {
                    if (firstSectionPassed)
                    {
                        GUILayout.Space(20f * SettingsGUI.Instance.Scale);
                    }
                    else
                    {
                        firstSectionPassed = true;
                    }

                    firstSectionItemPassed = false;
                }

                if (!isFirst && elementType != typeof(MySection))
                {
                    // add normal spacing between other elements.
                    // only do so after the first element since we don't want spacing between the section and its first element
                    if (!firstSectionItemPassed)
                    {
                        firstSectionItemPassed = true;
                    }
                    else
                    {
                        GUILayout.Space(10f * SettingsGUI.Instance.Scale);
                    }
                }
                
                element.Draw();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            GUI.DragWindow(this.dragRect);

            if (!firstDrawDone)
            {
                firstDrawDone = true;
            }

            switch (this.showStage)
            {
                case ShowStage.PENDING_OFFSCREEN_DRAW1:
                    this.showStage = ShowStage.PENDING_OFFSCREEN_DRAW2;
                    break;
                case ShowStage.PENDING_OFFSCREEN_DRAW2:
                    this.showStage = ShowStage.PENDING_DOCKING;
                    break;
            }
        }

        /// <summary>
        /// Calls Destroy() on all children. Doesn't actually do anything else. That's your job lazy programmer!
        ///
        /// Meant as a way to stop using this window without side effects like event handlers continuing.
        /// </summary>
        public void Destroy()
        {
            foreach (var child in children)
            {
                child.Destroy();
            }
        }
    }
}