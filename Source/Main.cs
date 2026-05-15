using System.IO;
using HarmonyLib;
using UnityEngine;

namespace ThickerTrajectoryLines
{
    public class TTLModMeta
    {
        public static readonly string MODID = "mod.aliser.thickertrajectorylines";
        public static readonly string MODNAME = "Thicker Trajectory Lines";
        public static readonly string MODDIRNAME = "ThickerTrajectoryLines";
    }

    public class TILModConsts
    {
        public static Texture2D GlowFade;
    }

    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class TTLModLoader : MonoBehaviour
    {
        void Start()
        {
            // todo remove on release
            Logger.SetLogLevel(LogLevel.VERBOSE);

            var log = new Logger("ThickerTrajectoryLines");
            log.Debug("Patching");

            var harmony = new Harmony(TTLModMeta.MODID);
            harmony.PatchAll(); // Automatically finds and applies all [HarmonyPatch] classes

            log.Debug("Patching complete");
            
            // replacement texture that doesn't have vertical fade
            TILModConsts.GlowFade = LoadTextureFromFile(
                $"{KSPUtil.ApplicationRootPath}GameData/{TTLModMeta.MODDIRNAME}/PluginData/Textures/GlowFade.png",
                256,
                16
            );
            log.Debug("Loading glow fade texture: " + (TILModConsts.GlowFade == null ? "failed" : "successful" ));
        }

        public static Texture2D LoadTextureFromFile(string filepath, int width, int height)
        {
            var log =  new Logger("ThickerTrajectoryLines/LoadTextureFromFile");
            
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