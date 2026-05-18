// using System;
// using System.Collections.Generic;
// using HarmonyLib;
// using UnityEngine;
// using Vectrosity;
//
// namespace ThickerTrajectoryLines
// {
//     [HarmonyPatch(typeof(OrbitRendererBase))]
//     [HarmonyPatch("DrawOrbit")]
//     [HarmonyPatch(new Type[] {
//         typeof(OrbitRendererBase.DrawMode),
//     })]
//     
//     public class Patch_Orbit_DrawOrbit
//     {
//         private static Dictionary<OrbitRendererBase, string> ids = new ();
//         
//         static bool Prefix(OrbitRendererBase __instance)
//         {
//             if (__instance.GetType() != typeof(OrbitRenderer))
//                 return true;
//
//             // skip non-vessel orbits I think
//             if (__instance.vessel == null)
//                 return true;
//             
//             string id;
//             if (!ids.TryGetValue(__instance, out id))
//             {
//                 id = Guid.NewGuid().ToString();
//                 ids.Add(__instance, id);
//             }
//             
//             Debug.Log($"[wawa][{id}][{__instance.GetType().Name}] Vessel: {__instance.vessel?.id}");
//             return false;
//         }
//     }
// }