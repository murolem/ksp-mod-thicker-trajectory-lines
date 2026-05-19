using System;
using System.IO;
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
            ToolbarControl.RegisterMod(Meta.modId, Meta.modName);
        }
    }
    
    [KSPAddon(KSPAddon.Startup.AllGameScenes, false)]
    public class SettingsGUI : MonoBehaviour
    {
        public static SettingsGUI Instance { get; private set; }
        public static ToolbarControl ToolbarControl { get; private set; }
        
        // game default = false
        [Persistent]
        public bool UseSolidGlowFadeTrajectoryTexture = true;
        public event Action<bool> UseSolidGlowFadeTrajectoryTextureChanged;
        
        private MyWindow settingsWindow;
        
        // game default = 5f
        [Persistent]
        public float ActiveObjectLineWidth = 10f;
        public event Action<float> ActiveObjectLineWidthChanged;
        
        // game default = 5f
        [Persistent]
        public float InactiveObjectLineWidth = 8f;
        public event Action<float> InactiveObjectLineWidthChanged;
        
        // game default = ?
        [Persistent]
        public float BodyOrbitsLineWidth = 15f;
        public event Action<float> BodyOrbitsLineWidthChanged;
        
        // game default = 5f
        [Persistent]
        public float ContractOrbitsLineWidth = 10f;
        public event Action<float> ContractOrbitsLineWidthChanged;
        


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
                // only supports relative paths
                $"{Meta.modDirname}/{Meta.modStaticDataDirname}/Textures/ToolbarButtonIcon38.png",
                $"{Meta.modDirname}/{Meta.modStaticDataDirname}/Textures/ToolbarButtonIcon24.png",
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
                .Center()

                .Append(new MySection("TECHNICAL"))
                .Append(
                    new MySlider("Window Scale", MyGUI.Scale, 1f, 4f, 0.1f)
                        .OnValueChanged(newValue =>
                        {
                            MyGUI.DirtyScale = newValue;
                        })
                )

                .Append(new MySection("TRAJECTORIES & ORBITS"))
                .Append(
                    new MyToggle("Solid Line Texture (May Require View Change)", UseSolidGlowFadeTrajectoryTexture)
                        .OnValueChanged(toggle =>
                        {
                            UseSolidGlowFadeTrajectoryTexture = toggle;
                            UseSolidGlowFadeTrajectoryTextureChanged?.Invoke(toggle);
                        })
                )

                .Append(
                    new MySlider("Line Width for the Active Object", ActiveObjectLineWidth, 5f, 50f, 1f)
                        .OnValueChanged(newValue =>
                        {
                            ActiveObjectLineWidth = newValue;
                            ActiveObjectLineWidthChanged?.Invoke(newValue);
                        })
                )
                // .Append(
                //     new MySlider("Active Vessel Trajectory Line Width Decay", VesselLineWidth, 0f, 1f, 0.05f)
                //         .OnValueChanged(newValue =>
                //         {
                //             VesselLineWidth = newValue;
                //             VesselLineWidthChanged?.Invoke(newValue);
                //         })
                // )
                .Append(
                    new MySlider("Line Width for Inactive Object", InactiveObjectLineWidth, 5f, 50f, 1f)
                        .OnValueChanged(newValue =>
                        {
                            InactiveObjectLineWidth = newValue;
                            InactiveObjectLineWidthChanged?.Invoke(newValue);
                        })
                )

                .Append(
                    new MySlider("Line Width for Body Orbits", BodyOrbitsLineWidth, 5f, 50f, 1f)
                        .OnValueChanged(newValue =>
                        {
                            BodyOrbitsLineWidth = newValue;
                            BodyOrbitsLineWidthChanged?.Invoke(newValue);
                        })
                )

                .Append(
                    new MySlider("Line Width for Contract Orbits", ContractOrbitsLineWidth, 5f, 50f, 1f)
                        .OnValueChanged(newValue =>
                        {
                            ContractOrbitsLineWidth = newValue;
                            ContractOrbitsLineWidthChanged?.Invoke(newValue);
                        })
                );
        }

        /// <summary>
        /// Save frequency, once max in <=N ms timespan.
        /// Meant to prevent a billion writes each time user drags a slider.
        /// </summary>
        private float settingsSaveFrequencyMs = 1000f;

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
            
            ActiveObjectLineWidthChanged += (_) => DirtySettings();
            InactiveObjectLineWidthChanged += (_) => DirtySettings();
            BodyOrbitsLineWidthChanged += (_) => DirtySettings();
            ContractOrbitsLineWidthChanged += (_) => DirtySettings();
            UseSolidGlowFadeTrajectoryTextureChanged += (_) => DirtySettings();
        }

        ConfigNode SaveSettings()
        {
            var log = new Logger();
            
            // ensure dirpath
            Directory.CreateDirectory(Path.GetDirectoryName(Meta.modUserConfigFilepath)!);

            var cfg = ConfigNode.CreateConfigFromObject(this);
            log.Verbose("Writing settings to: " + Meta.modUserConfigFilepath);
            cfg.Save(Meta.modUserConfigFilepath, "wawa");

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