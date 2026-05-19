using System;
using System.IO;
using System.Reflection;
using KSP.UI.Screens;
using ThickerTrajectoryLines.Attributes;
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
        [PersistentEvent]
        public event Action<bool> UseSolidGlowFadeTrajectoryTextureChanged;
        
        // game default = 5f
        [Persistent]
        public float ActiveObjectLineWidth = 10f;
        [PersistentEvent]
        public event Action<float> ActiveObjectLineWidthChanged;
        
        // game default = 5f
        [Persistent]
        public float InactiveObjectLineWidth = 8f;
        [PersistentEvent]
        public event Action<float> InactiveObjectLineWidthChanged;
        
        // game default = ?
        [Persistent]
        public float BodyOrbitsLineWidth = 15f;
        [PersistentEvent]
        public event Action<float> BodyOrbitsLineWidthChanged;
        
        // game default = 5f
        [Persistent]
        public float ContractOrbitsLineWidth = 10f;
        [PersistentEvent]
        public event Action<float> ContractOrbitsLineWidthChanged;
        
        private MyWindow window;
        
        void Awake()
        {
            Instance = this;
            EnsureSnapshotOfDefaultSettings();
            LoadSettings();
        }
        
        void Start()
        {
            SetupWindow();
            CreateButtonIcon();
            SetupSaveSettingWatcher();
        }

        void Update()
        {
            TickSaveSettingWatchers();
        }

        void OnDestroy()
        {
            var log = Logger.log;
            log.Verbose("SettingsGUI OnDestroy()!");
        }
        
        void CreateButtonIcon()
        {
            ToolbarControl = gameObject.AddComponent<ToolbarControl>();
            // toggle instead of Hide/Show since our window is closable by the close button,
            // which the toolbar control doesn't know about and will not update its state. 
            ToolbarControl.AddToAllToolbars(() => window.Toggle(), () => window.Toggle(),
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
            if (window.Visible)
            {
                window.Draw();
            }
        }

        void SetupWindow()
        {
            window = new MyWindow("[Settings] Thick Trajectories", 0, 0, 400, 1)
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
                    new MyToggle("Solid Line Texture (May Require Screen Change)", UseSolidGlowFadeTrajectoryTexture)
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
                )
                
                .Append(
                    new MyButton("Reset Settings", GUILayout.ExpandWidth(true))
                        .OnPressed(ResetSettings)
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
            var log = Logger.log;
            
            void DirtySettings(object obj, EventArgs e)
            {
                if(settingsDirty)
                    return;
                
                var timeUnscaledMs = Time.unscaledTime * 1000f;
                nextSaveWindowAtTs = timeUnscaledMs + settingsSaveFrequencyMs;
                settingsDirty = true;
            }


            var events = PersistentEvent.GetEventsFromClass(typeof(SettingsGUI), BindingFlags.Public | BindingFlags.Instance);
            log.Verbose("Found events to setup setting value change watchers on: " + events.Length);

            foreach (var eventInfo in events)
            {
                var eventWrapper = new ModelEvent(this, eventInfo);
                eventWrapper.OnEvent += DirtySettings;
            }
        }
        
        ConfigNode SaveSettings(string saveFilepathOverride = null)
        {
            var log = Logger.log;

            var saveFilepath = saveFilepathOverride ?? Meta.modUserConfigFilepath;
            
            // ensure dirpath
            Directory.CreateDirectory(Path.GetDirectoryName(saveFilepath)!);

            var cfg = ConfigNode.CreateConfigFromObject(this);
            log.Verbose("Writing settings to: " + saveFilepath);
            cfg.Save(saveFilepath, "wawa");

            return cfg;
        }

        void LoadSettings(string saveFilepathOverride = null)
        {
            var log = Logger.log;
            
            var saveFilepath = saveFilepathOverride ?? Meta.modUserConfigFilepath;
            
            // ensure dirpath
            Directory.CreateDirectory(Path.GetDirectoryName(saveFilepath)!);

            ConfigNode cfg;
            if (File.Exists(saveFilepath))
            {
                log.Verbose("Reading settings from: " + saveFilepath);
                cfg = ConfigNode.Load(saveFilepath);
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

        /// <summary>
        /// Creates snapshot of current settings as defaults.
        ///
        /// Meant to be called before everything else.
        /// </summary>
        void EnsureSnapshotOfDefaultSettings()
        {
            var log = Logger.log;
            log.Verbose("Writing setting defaults");
            SaveSettings(Meta.modDefaultsConfigFilepath);
        }
        
        /// <summary>
        /// Loads settings from a default settings snapshot file that is assumed to be created at an earlier step.
        /// </summary>
        void ResetSettings()
        {
            var log = Logger.log;
            log.Verbose("Loading default settings");
            
            // load defaults
            LoadSettings(Meta.modDefaultsConfigFilepath);
            // destroy current window since gui controls don't get updated values
            window.Destroy();
            // make new window using the new default values
            SetupWindow();
            // and make it visible
            window.Show();
        }

    }
}