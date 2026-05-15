using System;
using System.Collections.Generic;
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
            PrepareGUI();
        }
        
        
        void CreateButtonIcon()
        {
            toolbarControl = gameObject.AddComponent<ToolbarControl>();
            toolbarControl.AddToAllToolbars(Show, Hide,
                ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW | ApplicationLauncher.AppScenes.TRACKSTATION,
                TTLModMeta.MODID,
                TTLModMeta.MODID + ".button",
                $"{TTLModMeta.MODDIRNAME}/PluginData/Textures/ToolbarButtonIcon38.png",
                $"{TTLModMeta.MODDIRNAME}/PluginData/Textures/ToolbarButtonIcon24.png",
                "Thicker Trajectory Lines"
            );
        }


        private MyWindow settingsWindow;
        public static float OrbitalLineWidth = 5f;
        public static event Action<float> OrbitalLineWidthChanged;
        public static bool UseSolidTrajectoryLines = true;
        public static event Action<bool> UseSolidTrajectoryLinesChanged;
        void PrepareGUI()
        {
            settingsWindow = new MyWindow("[Settings] Thick Trajectories", 0, 0, 200, 300)
                .Center();

            {
                var slider = new MySlider("Window Scale", GameSettings.UI_SCALE, 1f, 4f, 0.1f);
                slider.ValueChanged += newValue =>
                {
                    settingsWindow.SetScale(newValue);
                };
                settingsWindow.Append(slider);
            }
            {
                var slider = new MySlider("Trajectory Line Width", OrbitalLineWidth, 5f, 50f, 1f);
                slider.ValueChanged += newValue =>
                    {
                        OrbitalLineWidth = newValue;
                        OrbitalLineWidthChanged?.Invoke(newValue);
                    };
                settingsWindow.Append(slider);
            }

            {
                var toggle = new MyCheckbox("Use Solid Trajectory Lines?", UseSolidTrajectoryLines);
                toggle.ValueChanged += newValue =>
                {
                    UseSolidTrajectoryLines = newValue;
                    UseSolidTrajectoryLinesChanged?.Invoke(newValue);
                };
                settingsWindow.Append(toggle);
            }
        }
 
        void OnGUI()
        {
            if (isVisible)
            {
                settingsWindow.Draw();
            }
        }
    }
}