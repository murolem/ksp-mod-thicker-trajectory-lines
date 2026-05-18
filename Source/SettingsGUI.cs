using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
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
            ToolbarControl.RegisterMod(Meta.modId, Meta.modName);
        }
    }
    
    [KSPAddon(KSPAddon.Startup.AllGameScenes, false)]
    public class SettingsGUI : MonoBehaviour
    {
        public static SettingsGUI Instance { get; private set; }
        public static ToolbarControl ToolbarControl { get; private set; }
        
        // game default = 5f
        [Persistent]
        public float OrbitalLineWidth = 10f;
        public event Action<float> OrbitalLineWidthChanged;
        
        // game default = false
        [Persistent]
        public bool UseSolidGlowFadeTrajectoryTexture = true;
        public event Action<bool> UseSolidGlowFadeTrajectoryTextureChanged;
        
        private MyWindow settingsWindow;

        void Awake()
        {
            Instance = this;
            LoadSettings();
        }
        
        void Start()
        {
            PrepareGUI();
            CreateButtonIcon();

            SetupSaveSettingWatcher();
        }

        void Update()
        {
            TickSaveSettingWatchers();
        }
        
        void CreateButtonIcon()
        {
            ToolbarControl = gameObject.AddComponent<ToolbarControl>();
            // toggle instead of Hide/Show since our window is closable by the close button,
            // which the toolbar control doesn't know about and will not update its state. 
            ToolbarControl.AddToAllToolbars(() => settingsWindow.Toggle(), () => settingsWindow.Toggle(),
                ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW |
                ApplicationLauncher.AppScenes.TRACKSTATION,
                Meta.modId,
                Meta.modId + ".button",
                $"{Meta.modStaticDataDirpath}/Textures/ToolbarButtonIcon38.png",
                $"{Meta.modStaticDataDirpath}/Textures/ToolbarButtonIcon24.png",
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
                };
                settingsWindow.Append(slider);
            }
            {
                settingsWindow.Append(new MySection("TRAJECTORY/ORBIT"));
            }
            {
                var slider = new MySlider("Line Width", OrbitalLineWidth, 5f, 50f, 1f);
                slider.ValueChanged += newValue =>
                {
                    OrbitalLineWidth = newValue;
                    OrbitalLineWidthChanged?.Invoke(newValue);
                };
                settingsWindow.Append(slider);
            }
            {
                var toggle = new MyToggle("Use Solid Lines? (May Require View Change)", UseSolidGlowFadeTrajectoryTexture);
                toggle.ValueChanged += newValue =>
                {
                    UseSolidGlowFadeTrajectoryTexture = newValue;
                    UseSolidGlowFadeTrajectoryTextureChanged?.Invoke(newValue);
                };
                settingsWindow.Append(toggle);
            }
        }

        /// <summary>
        /// Save frequency, once max in <=N ms timespan.
        /// </summary>
        private float settingsSaveFrequencyMs = 3000f;

        /// <summary>
        /// Whether settings need saving.
        /// </summary>
        private bool settingsDirty = false;

        private float nextSaveWindowAtTs = -1f;

        /// <summary>
        /// Update() loop for setting save watchers.
        /// Saves settings once the "dirty" flag is set, but only with regular intervals when the rate of change is high. 
        /// </summary>
        void TickSaveSettingWatchers()
        {
            if (!settingsDirty)
                return;
            
            var timeUnscaledMs = Time.unscaledTime * 1000f;
            if (timeUnscaledMs >= nextSaveWindowAtTs)
            {
                SaveSettings();
                settingsDirty = false;
            }
        }
        
        /// <summary>
        /// Setup event listeners that watch settings for changes.
        /// Once change occurs, the "dirty" flag is set, scheduling a save.  
        /// </summary>
        void SetupSaveSettingWatcher()
        {
            void DirtySettings()
            {
                if(settingsDirty)
                    return;
                
                var timeUnscaledMs = Time.unscaledTime * 1000f;
                nextSaveWindowAtTs = timeUnscaledMs + settingsSaveFrequencyMs;
                settingsDirty = true;
            }
            
            OrbitalLineWidthChanged += (_) => DirtySettings();
            UseSolidGlowFadeTrajectoryTextureChanged += (_) => DirtySettings();
        }

        ConfigNode SaveSettings()
        {
            var log = new Logger();
            
            // ensure dirpath
            Directory.CreateDirectory(Path.GetDirectoryName(Meta.modUserConfigFilepath)!);

            var cfg = ConfigNode.CreateConfigFromObject(this);
            log.Verbose("Writing settings to: " + Meta.modUserConfigFilepath);
            cfg.Save(Meta.modUserConfigFilepath);

            return cfg;
        }

        void LoadSettings()
        {
            var log = new Logger();
            
            // ensure dirpath
            Directory.CreateDirectory(Path.GetDirectoryName(Meta.modUserConfigFilepath)!);

            ConfigNode cfg;
            if (File.Exists(Meta.modUserConfigFilepath))
            {
                log.Verbose("Reading settings from: " + Meta.modUserConfigFilepath);
                cfg = ConfigNode.Load(Meta.modUserConfigFilepath);
                // in case of errors or whatever reset the config
                if (cfg == null)
                {
                    cfg = SaveSettings();
                }
            }
            else
            {
                log.Verbose("Settings file not found, generating default config");
                cfg = SaveSettings();
            }
            
            ConfigNode.LoadObjectFromConfig(this, cfg);
        }
    }
}