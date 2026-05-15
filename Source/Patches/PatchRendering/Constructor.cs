using System;
using HarmonyLib;
using UnityEngine;

namespace ThickerTrajectoryLines
{
    [HarmonyPatch(typeof(PatchRendering))]
    [HarmonyPatch(MethodType.Constructor)]
    [HarmonyPatch(new Type[] {
        typeof(string),
        typeof(int),
        typeof(int),
        typeof(Orbit),
        typeof(Material),
        typeof(float),
        typeof(bool),
        typeof(PatchedConicRenderer),
    })]
    public class Patch_PatchRendering_Ctor
    {
        static void Postfix(PatchRendering __instance)
        {
            var log = new Logger("ThickerTrajectoryLines/PatchRendering__Ctor/Postfix");
            log.Verbose("Running");

            if (TILModConsts.GlowFade)
            {
                var oldTexture = __instance.lineMaterial.mainTexture;
                if (SettingsGUI.UseSolidTrajectoryLines)
                {
                    __instance.lineMaterial.mainTexture = TILModConsts.GlowFade;
                }

                SettingsGUI.UseSolidTrajectoryLinesChanged += toggle =>
                {
                    __instance.lineMaterial.mainTexture = toggle
                        ? TILModConsts.GlowFade
                        : oldTexture;
                };
            }
            
            Action<float> setLineWidth = newWidth => { __instance.lineWidth = newWidth; };
            setLineWidth(SettingsGUI.OrbitalLineWidth);
            SettingsGUI.OrbitalLineWidthChanged += setLineWidth;
        }
    }
}