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
            ToolbarControl.RegisterMod(ThickerTrajectoryLinesMod.MODID, ThickerTrajectoryLinesMod.MODNAME);
        }
    }
    
    [KSPAddon(KSPAddon.Startup.AllGameScenes, false)]
    public class SettingsGUI : MonoBehaviour
    {
        [CanBeNull] private static ToolbarControl toolbarControl = null;

        public static float OrbitalLineWidth = 5f;
        public static event Action<float> OrbitalLineWidthChanged;

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
                ThickerTrajectoryLinesMod.MODID,
                ThickerTrajectoryLinesMod.MODID + ".button",
                "ThickerTrajectoryLines/PluginData/Textures/ToolbarButtonIcon38.png",
                "ThickerTrajectoryLines/PluginData/Textures/ToolbarButtonIcon24.png",
                "Thicker Trajectory Lines"
            );
        }

 
        void OnGUI()
        {
            // Debug.Log("[wawa] on gui!");
            if (toolbarControl?.buttonActive == true)
            {
                var id = 60273460;
                var width = 200;
                var height = 300;
                var x = Screen.width / 2 - width / 2 + 300;
                var y = Screen.height / 2 - height / 2;
                var rect = new Rect(x, y, width, height);
                var window = ClickThruBlocker.GUILayoutWindow(id, rect, PopulateGUI, "Thick Trajectories Settings");
                GUI.DragWindow();

                // Debug.Log(OrbitalLineWidth);
            }
        }
        
        private static void PopulateGUI(int id)
        {
            GUILayout.Label("Orbital Line Width");
            // Label for the slider
            var oldValue = OrbitalLineWidth;
            OrbitalLineWidth = GUILayout.HorizontalSlider(OrbitalLineWidth, 5f, 1000f);
            if (OrbitalLineWidth != oldValue)
            {
                OrbitalLineWidthChanged?.Invoke(OrbitalLineWidth);
            }
        }
    }
}