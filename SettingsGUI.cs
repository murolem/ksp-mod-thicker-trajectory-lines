using ClickThroughFix;
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
        private static Rect? window = null;

        public static float OrbitalLineWidth = 5f;

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
            Debug.Log("[wawa] CreateButtonIcon");
            var toolbarControl = gameObject.AddComponent<ToolbarControl>();
            toolbarControl.AddToAllToolbars(Show, Hide,
                ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW | ApplicationLauncher.AppScenes.TRACKSTATION,
                ThickerTrajectoryLinesMod.MODID,
                ThickerTrajectoryLinesMod.MODID + ".button",
                "ThickerTrajectoryLines/PluginData/Textures/ToolbarButtonIcon38.png",
                "ThickerTrajectoryLines/PluginData/Textures/ToolbarButtonIcon24.png",
                "Thicker Trajectory Lines"
            );
        }

 
        public void OnGUI()
        {
            // Debug.Log("[wawa] on gui!");
            if (isVisible)
            {
                var id = 60273460;
                var width = 200;
                var height = 300;
                var x = Screen.width / 2 - width / 2;
                var y = Screen.height / 2 - height / 2;
                var rect = new Rect(x, y, width, height);
                window = ClickThruBlocker.GUILayoutWindow(id, rect, PopulateGUI, "Thick Trajectories Settings");
            }
        }

        private static void PopulateGUI(int id)
        {
            GUILayout.Label("Orbital Line Width");
            // Label for the slider
            OrbitalLineWidth = GUILayout.HorizontalSlider(OrbitalLineWidth, 5f, 100f);
        }
    }
}