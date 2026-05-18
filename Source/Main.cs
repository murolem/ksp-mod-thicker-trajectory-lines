using System.IO;
using HarmonyLib;
using UnityEngine;

// todo click blocking
// todo replace all Debug. with the Logger calls

namespace ThickerTrajectoryLines
{
    public static class Meta
    {
        public static readonly string modId = "mod.aliser.thickertrajectorylines";
        public static readonly string modName = "Thicker Trajectory Lines";
        public static readonly string modDirname = "ThickerTrajectoryLines";

        public static string modDirpath =>
            Path.GetFullPath($"{KSPUtil.ApplicationRootPath}/GameData/{Meta.modDirname}/");

        /// <summary>
        /// Static data folder (Static folder in the repo). Meant to be unchanged.
        /// </summary>
        public static string modStaticDataDirpath => Path.GetFullPath($"{modDirpath}/PluginData/");
        
        /// <summary>
        /// Static config filepath. Config that is used internally and is shipped with the mod. Meant to be unchanged.
        /// </summary>
        public static string modStaticConfigFilepath => Path.GetFullPath($"{modDirpath}/config.cfg");
        
        /// <summary>
        /// Mod user config dirpath. Holds configs that can change throughout the game.
        /// </summary>
        public static string modUserConfigDirpath => Path.GetFullPath($"{KSPUtil.ApplicationRootPath}/PluginData/{modDirname}/");
        /// <summary>
        /// Mod user config filepath. Config that can change throughout the game.
        /// </summary>
        public static string modUserConfigFilepath => Path.GetFullPath($"{modUserConfigDirpath}/config.cfg");
    }

    public static class Globals
    {
        public static Texture2D GlowFade;
    }

    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class ModLoader : MonoBehaviour
    {
        void Start()
        {
            // todo remove on release
            Logger.SetLogLevel(LogLevel.VERBOSE);

            var log = new Logger("ThickerTrajectoryLines");
            log.Debug("Patching");

            var harmony = new Harmony(Meta.modId);
            harmony.PatchAll(); // Automatically finds and applies all [HarmonyPatch] classes

            log.Debug("Patching complete");
            
            // replacement texture that doesn't have vertical fade
            Globals.GlowFade = LoadTextureFromFile(
                Path.GetFullPath($"{Meta.modStaticDataDirpath}/Textures/GlowFade.png"),
                256,
                16
            );
            log.Debug("Loading glow fade texture: " + (Globals.GlowFade == null ? "failed" : "successful" ));
        }

        static Texture2D LoadTextureFromFile(string filepath, int width, int height)
        {
            var log = new Logger("ThickerTrajectoryLines/LoadTextureFromFile");
            
            Texture2D tex = new Texture2D(width, height, UnityEngine.TextureFormat.ARGB32, false);
            if (!File.Exists(filepath))
            {
                log.Error("Failed to load texture - texture path not found: " + filepath);
                return null;
            }
            
            tex.LoadImage(File.ReadAllBytes(filepath));
            return tex;
        }
    }
}