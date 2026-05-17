using System;
using JetBrains.Annotations;
using UnityEngine;

namespace ThickerTrajectoryLines
{
    public enum GUI_STYLE_NAME {
        box,
        label,
        textField,
        textArea,
        button,
        toggle,
        window,
        horizontalSlider,
        horizontalSliderThumb,
        verticalSlider,
        verticalSliderThumb,
        horizontalScrollbar,
        horizontalScrollbarThumb,
        horizontalScrollbarLeftButton,
        horizontalScrollbarRightButton,
        verticalScrollbar,
        verticalScrollbarThumb,
        verticalScrollbarUpButton,
        verticalScrollbarDownButton,
        scrollView,
    }
    
    /// <summary>
    /// GUI utils and consts. 
    /// </summary>
    public class MyGUI
    {
        public static GUISkin defaultSkin = HighLogic.Skin;
        
        /// <summary>
        /// UI Scale. Only updates when user lets go of the scale slider.
        /// </summary>
        public static float Scale = GameSettings.UI_SCALE;
        public static Action<float> ScaleChanged;
        /// <summary>
        /// Same as `ScaleChanged`, but subscribes get invoked only after all the subscribes at the main event.
        ///
        /// Meant to only be used internally by `MyWindow`.
        /// </summary>
        public static Action<float> ScaleChangedLowPriority;

        /// <summary>
        /// Called once before first draw happens but during OnGUI. Useful for setting up Unity GUI related stuff. 
        /// </summary>
        public static Action BeforeFirstDraw;

        /// <summary>
        /// Scale but changes constantly while user drags the scale slider.
        /// The normal scale is only updated when users lets go of the slider.
        /// </summary>
        public static float DirtyScale = 1f;

        // a random value I found in ksp source code. idk how to get actual font size
        public static readonly int BaseFontSize = 14;

        /// <summary>
        /// Current font size with respect to the current scale.
        /// </summary>
        public static int FontSize => Mathf.RoundToInt(BaseFontSize * Scale);

        /// <summary>
        /// Get GUI style with a predefined name.
        /// </summary>
        /// <param name="styleName">A GUI style name. Uses the styles defined by Unity.</param>
        /// <returns>Found style or a fallback style.</returns>
        public static GUIStyle MakeGUIStyle(GUI_STYLE_NAME styleName)
        {
            var style = defaultSkin.FindStyle(Enum.GetName(typeof(GUI_STYLE_NAME), styleName));
            return new GUIStyle(style ?? GUI.skin.box);
        }
        
        /// <summary>
        /// Get GUI style with a given name.
        /// </summary>
        /// <param name="name">A GUI style name.</param>
        /// <returns>Found style or a fallback style</returns>
        public static GUIStyle MakeGUIStyle(string name)
        {
            var style = defaultSkin.FindStyle(name);
            return new GUIStyle(style ?? GUI.skin.box);
        }
        
        public static RectOffset RectOffset(int offset)
        {
            return new RectOffset(offset, offset, offset, offset);
        }

        public static RectOffset RectOffset(int verticalOffset, int horizontalOffset)
        {
            return new RectOffset(horizontalOffset,  horizontalOffset, verticalOffset, verticalOffset);
        }
        
        public static RectOffset RectOffset(float offset)
        {
            return MyGUI.RectOffset((int)offset);
        }

        public static RectOffset RectOffset(float verticalOffset, float horizontalOffset)
        {
            return MyGUI.RectOffset((int)verticalOffset, (int)horizontalOffset);
        }

        public static RectOffset RectOffset(float left, float right, float top, float bottom)
        {
            return new RectOffset((int)left, (int)right,  (int)top, (int)bottom);
        }

        public static RectOffset ScaleRectOffset(RectOffset baseOffset, RectOffset currentOffset)
        {
            currentOffset.left = Mathf.RoundToInt(baseOffset.left * Scale);
            currentOffset.right = Mathf.RoundToInt(baseOffset.right * Scale);
            currentOffset.top = Mathf.RoundToInt(baseOffset.top * Scale);
            currentOffset.bottom = Mathf.RoundToInt(baseOffset.bottom * Scale);
            return currentOffset;
        }

        public static RectOffset CloneRectOffset(RectOffset rectOffset)
        {
            return new RectOffset(rectOffset.left, rectOffset.right, rectOffset.top, rectOffset.bottom);
        }

        /// <summary>
        /// Similar to Color.HSVToRGB, but the arguments are not scaled.
        /// </summary>
        /// <param name="h">Hue (0-360)</param>
        /// <param name="s">Saturation (0-100)</param>
        /// <param name="v">Value (0-100)</param>
        /// <returns>RGB Color</returns>
        public static Color HSVColor(float h, float s, float v)
        {
            return Color.HSVToRGB(h / 360f, s / 100f, v / 100f);
        }
                
        public static Texture2D MakeTex( int width, int height, Color col )
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
    }
}