using HarmonyLib;
using UnityEngine;

namespace ThickerTrajectoryLines
{
    public class TTLModMeta
    {
        public static readonly string MODID = "mod.aliser.thickertrajectorylines";
        public static readonly string MODNAME = "Thicker Trajectory Lines";
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
        }
    }
}