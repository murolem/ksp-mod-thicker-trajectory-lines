// using System;
// using System.Collections.Generic;
// using System.Reflection;
// using System.Reflection.Emit;
// using HarmonyLib;
// using UnityEngine;
// using Vectrosity;
//
// namespace ThickerTrajectoryLines
// {
//     [HarmonyPatch(typeof(OrbitRendererBase), "MakeLine")]
//     public class OrbitRendererBase__MethodMakeLine : MonoBehaviour
//     {
//         private static float LineThickness = 100f;
//         private static double LineSamples = 8;
//
//         private static FieldInfo LineThicknessField =
//             AccessTools.Field(typeof(OrbitRendererBase__MethodMakeLine), nameof(LineThickness));
//
//         private static FieldInfo LineSamplesField =
//             AccessTools.Field(typeof(OrbitRendererBase__MethodMakeLine), nameof(LineSamples));
//
//         private static FieldInfo LineSamplesOrigField =
//             AccessTools.Field(typeof(OrbitRendererBase), "sampleResolution");
//
//         static bool Prefix(ref VectorLine l, OrbitRendererBase __instance, double ___sampleResolution, int ___layerMask)
//         {
//             if (l != null)
//             {
//                 label_1:
//                 switch (3)
//                 {
//                     case 0:
//                         goto label_1;
//                     default:
//                         if (false)
//                         {
//                             // ISSUE: method reference
//                             // RuntimeMethodHandle runtimeMethodHandle = __methodref (OrbitRendererBase.MakeLine);
//                         }
//
//                         VectorLine.Destroy(ref l);
//                         break;
//                 }
//             }
//
//             l = new VectorLine(
//                 __instance.name + " orbit",
//                 new List<Vector3>(
//                     Traverse.Create(__instance).Method("GetSegmentCount", ___sampleResolution).GetValue<int>()
//                 ),
//                 LineThickness,
//                 LineType.Continuous,
//                 Joins.Weld
//             );
//             l.texture = MapView.OrbitLinesMaterial.mainTexture;
//             l.material = MapView.OrbitLinesMaterial;
//             l.material.SetFloat("_FadeStrength", GameSettings.ORBIT_FADE_STRENGTH);
//             Material material = l.material;
//             double num;
//             if (!GameSettings.ORBIT_FADE_DIRECTION_INV)
//             {
//                 label_6:
//                 switch (1)
//                 {
//                     case 0:
//                         goto label_6;
//                     default:
//                         num = 1.0;
//                         break;
//                 }
//             }
//             else
//                 num = -1.0;
//
//             material.SetFloat("_FadeSign", (float)num);
//             l.continuousTexture = true;
//             l.color = (Color32)Traverse.Create(__instance).Method("GetOrbitColour", ___sampleResolution)
//                 .GetValue<Color>();
//             l.rectTransform.gameObject.layer = ___layerMask;
//             l.UpdateImmediate = true;
//
//             return false;
//         }
//     }
//
//
//     //     static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
//     //     {
//     //         var patchCounter = 0;
//     //         foreach (var instruction in instructions)
//     //         {
//     //             if (instruction.LoadsConstant(5f))
//     //             {
//     //                 yield return new CodeInstruction(OpCodes.Ldsfld, LineThicknessField);
//     //                 Debug.Log("[ThickerTrajectoryLines] Thickness patched");
//     //                 patchCounter++;
//     //             }
//     //             // else if (instruction.LoadsField(LineSamplesOrigField))
//     //             // {
//     //             //     yield return new CodeInstruction(OpCodes.Ldsfld, LineSamplesField);
//     //             //     Debug.Log("[ThickerTrajectoryLines] Width patched");
//     //             //     patchCounter++;
//     //             // }
//     //             else
//     //             {
//     //                 yield return instruction;
//     //             }
//     //         }
//     //
//     //         Debug.Log("[ThickerTrajectoryLines] Patching complete, patched: " + patchCounter + " out of 2");
//     //     }
//     // }
//     
//     [KSPAddon(KSPAddon.Startup.Instantly, true)]
//     public class ThickerTrajectoryLinesLoader : MonoBehaviour
//     {
//         void Start()
//         {
//             Debug.Log("[ThickerTrajectoryLines] Patching");
//             var harmony = new Harmony("mod.aliser.thickertrajectorylines");
//             harmony.PatchAll(); // Automatically finds and applies all [HarmonyPatch] classes
//             
//             var field = AccessTools.Field(typeof(OrbitRendererBase), "sampleResolution");
//             field.SetValue(null, 1d);
//     
//             Debug.Log("[ThickerTrajectoryLines] Patched field: sampleResolution");
//         }
//     }
//
// }