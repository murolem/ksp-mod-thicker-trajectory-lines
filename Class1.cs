using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using Vectrosity;

namespace ThickerTrajectoryLines
{
    [HarmonyPatch(typeof(PatchRendering), "MakeVector")]
    public class PatchRendering__MakeVector
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Debug.Log("[ThickerTrajectoryLines] Patching PatchRendering__MakeVector");
            var matcher = new CodeMatcher(instructions)
                .MatchStartForward(
                    // LineType enum
                    // new CodeMatch(i => i.opcode == OpCodes.Ldc_I4_1),
                    new CodeMatch(i => i.opcode == OpCodes.Newobj)
                );

            if (matcher.IsInvalid)
            {
                Debug.Log("[ThickerTrajectoryLines] Patch failed - match failed");
                return instructions;
            }
            
            var ctor = typeof(VectorLine).GetConstructor(new Type[]
            {
               typeof(string),
               typeof(List<Vector3>),
               typeof(float),
               typeof(LineType),
               typeof(Joins),
            });
            if (ctor == null)
            {
                Debug.Log("[ThickerTrajectoryLines] Patch failed - failed to match constructor");
                return instructions;
            }
            return matcher
                .RemoveInstruction()
                .Insert(
                    new CodeInstruction(OpCodes.Ldc_I4_1), // Joins.Weld
                    new CodeInstruction(OpCodes.Newobj, ctor)
                    )
                .Instructions();
        }
    }
    
    [HarmonyPatch(typeof(PatchedConicRenderer), "Start")]
    public class PatchedConicRenderer__Start : MonoBehaviour
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Debug.Log("[ThickerTrajectoryLines] Patching PatchedConicRenderer__Start");
            var matcher = new CodeMatcher(instructions)
                .MatchStartForward(
                    // find hardcoded line width
                    new CodeMatch(i => i.LoadsConstant(5f))
                );
            if (matcher.IsInvalid)
            {
                Debug.Log("[ThickerTrajectoryLines] Patch failed - match failed");
                return instructions;
            }
            
            return matcher
                .SetInstruction(
                    new CodeInstruction(
                        OpCodes.Ldsfld, 
                        AccessTools.Field(typeof(SettingsGUI), nameof(SettingsGUI.OrbitalLineWidth))
                    )
                )
                .Instructions();
        }
    }
    
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class ThickerTrajectoryLinesLoader : MonoBehaviour
    {
        void Start()
        {
            Debug.Log("[ThickerTrajectoryLines] Patching");
            var harmony = new Harmony(ThickerTrajectoryLinesMod.MODID);
            harmony.PatchAll(); // Automatically finds and applies all [HarmonyPatch] classes
            
            // var field = AccessTools.Field(typeof(OrbitRendererBase), "sampleResolution");
            // field.SetValue(null, 1d);
            //
            // Debug.Log("[ThickerTrajectoryLines] Patched field: sampleResolution");
        }
    }

    public class ThickerTrajectoryLinesMod
    {
        public static readonly string MODID = "mod.aliser.thickertrajectorylines";
        public static readonly string MODNAME = "Thicker Trajectory Lines";
    }
}