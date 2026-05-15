using System;
using ClickThroughFix;
using JetBrains.Annotations;
using KSP.UI.Screens;
using ToolbarControl_NS;
using UnityEngine;

namespace ThickerTrajectoryLines
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class SettingsGUIRegister : MonoBehaviour
    {
        void Start()
        {
            ToolbarControl.RegisterMod(TTLModMeta.MODID, TTLModMeta.MODNAME);
        }
    }
    
    [KSPAddon(KSPAddon.Startup.AllGameScenes, false)]
    public class SettingsGUI : MonoBehaviour
    {
        public static float OrbitalLineWidth = 5f;
        public static event Action<float> OrbitalLineWidthChanged;
        
        private static ToolbarControl toolbarControl;
        private static Rect? windowRect;
        private static bool isVisible = false;
        
        public static void Show()
        {
            isVisible = true;
        }
        
        public static void Hide()
        {
            isVisible = false;
        }
        
        void Start()
        {
            CreateButtonIcon();
        }
        
        
        void CreateButtonIcon()
        {
            // Debug.Log("[wawa] CreateButtonIcon");
            toolbarControl = gameObject.AddComponent<ToolbarControl>();
            toolbarControl.AddToAllToolbars(Show, Hide,
                ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW | ApplicationLauncher.AppScenes.TRACKSTATION,
                TTLModMeta.MODID,
                TTLModMeta.MODID + ".button",
                "ThickerTrajectoryLines/PluginData/Textures/ToolbarButtonIcon38.png",
                "ThickerTrajectoryLines/PluginData/Textures/ToolbarButtonIcon24.png",
                "Thicker Trajectory Lines"
            );
        }

 
        void OnGUI()
        {
            if (isVisible)
            {
                var id = 60273460;
                if (windowRect == null)
                {
                    var width = 200;
                    var height = 300;
                    // todo pick position near the toolbar button instead
                    var x = Screen.width - width / 2 - 100;
                    var y = Screen.height / 2 - height / 2;
                    windowRect = new Rect(x, y, width, height);
                }
                windowRect = ClickThruBlocker.GUILayoutWindow(id, (Rect)windowRect, PopulateGUI, "Thick Trajectories Settings");
            }
        }
        
        // todo add scale slider (100% - 300%?)
        private static void PopulateGUI(int id)
        {
            GUILayout.Label("Orbital Line Width");
            // Label for the slider
            var oldValue = OrbitalLineWidth;
            // todo fine tune max value
            // todo add min and max values as labels, add current value as label
            OrbitalLineWidth = GUILayout.HorizontalSlider(OrbitalLineWidth, 5f, 50f);
            if (OrbitalLineWidth != oldValue)
            {
                OrbitalLineWidthChanged?.Invoke(OrbitalLineWidth);
            }
            
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }
    }
}