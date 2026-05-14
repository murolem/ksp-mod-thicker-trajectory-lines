using System;
using ClickThroughFix;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ThickerTrajectoryLines
{
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class SettingsGUI : MonoBehaviour
    {
        private static Rect? window = null;

        public static float OrbitalLineWidth = 5f;

        private static bool isVisible = true;
        
        public static void Show()
        {
            isVisible = true;
        }
        
        public static void Hide()
        {
            isVisible = false;
        }
 
        public void OnGUI()
        {
            Debug.Log("[wawa] on gui!");
            if (isVisible)
            {
                var id = 60273460;
                var rect = new Rect(0, 0, -1, -1);
                window = ClickThruBlocker.GUILayoutWindow(id, rect, PopulateGUI, "Thick Trajectories Settings");
            }
        }

        private static void PopulateGUI(int id)
        {
            GUILayout.Label("Orbital Line Width");
            // Label for the slider
            GUILayout.HorizontalSlider(OrbitalLineWidth, 5f, 100f);

        }
    }
}