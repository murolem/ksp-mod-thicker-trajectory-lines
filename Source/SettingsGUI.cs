using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Generic;
using KSP.UI.Screens;
using ToolbarControl_NS;
using UnityEngine;
using UnityEngine.Serialization;

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
        public static SettingsGUI Instance { get; private set; }
        public static ToolbarControl ToolbarControl { get; private set; }
        
        public static float OrbitalLineWidth = 5f;
        public static event Action<float> OrbitalLineWidthChanged;
        public static bool UseSolidTrajectoryLines = true;
        public static event Action<bool> UseSolidTrajectoryLinesChanged;
        
        private MyWindow settingsWindow;

        void Awake()
        {
            Instance = this;
        }
        
        void Start()
        {
            PrepareGUI();
            CreateButtonIcon();
        }
        
        void CreateButtonIcon()
        {
            ToolbarControl = gameObject.AddComponent<ToolbarControl>();
            // toggle instead of Hide/Show since our window is closable by the close button,
            // which the toolbar control doesn't know about and will not update its state. 
            ToolbarControl.AddToAllToolbars(() => settingsWindow.Toggle(), () => settingsWindow.Toggle(),
                ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW |
                ApplicationLauncher.AppScenes.TRACKSTATION,
                TTLModMeta.MODID,
                TTLModMeta.MODID + ".button",
                $"{TTLModMeta.MODDIRNAME}/PluginData/Textures/ToolbarButtonIcon38.png",
                $"{TTLModMeta.MODDIRNAME}/PluginData/Textures/ToolbarButtonIcon24.png",
                "Thicker Trajectory Lines"
            );
        }
        
        void OnGUI()
        {
            if (settingsWindow.Visible)
            {
                settingsWindow.Draw();
            }
        }

        void PrepareGUI()
        {
            settingsWindow = new MyWindow("[Settings] Thick Trajectories", 0, 0, 400, 1)
                .Center();

            {
                settingsWindow.Append(
                    new MySection("TECHNICAL")
                );
            }
            {
                var slider = new MySlider("Window Scale", MyGUI.Scale, 1f, 4f, 0.1f);
                slider.ValueChanged += newValue => { 
                    MyGUI.DirtyScale = newValue;
                    MyGUI.DirtyScaleReady = true;
                };
                settingsWindow.Append(slider);
            }
            {
                settingsWindow.Append(new MySection("TRAJECTORY"));
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
                var toggle = new MyToggle("Use Solid Trajectory Lines?", UseSolidTrajectoryLines);
                toggle.ValueChanged += newValue =>
                {
                    UseSolidTrajectoryLines = newValue;
                    UseSolidTrajectoryLinesChanged?.Invoke(newValue);
                };
                settingsWindow.Append(toggle);
            }
        }
    }
}