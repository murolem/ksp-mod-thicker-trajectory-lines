using System;
using HarmonyLib;
using Vectrosity;

namespace ThickerTrajectoryLines
{
    [HarmonyPatch(typeof(PatchRendering), nameof(PatchRendering.MakeVector))]
    public class Patch_PatchRendering__MakeVector
    {
        static void Postfix(PatchRendering __instance)
        {
            var log = new Logger("ThickerTrajectoryLines/PatchRendering__MakeVector/Postfix");
            
            log.Verbose("Running");
            
            var vectorLineTraverse = Traverse.Create(__instance).Field("vectorLine");
            if (!vectorLineTraverse.FieldExists())
            {
                log.Error("Field 'vectorLine' not found");
                return;
            }
            var vectorLine = vectorLineTraverse.GetValue<VectorLine>();
            if (vectorLine == null)
            {
                log.Error("Field 'vectorLine' is null");
                return;
            }

            // makes better looking lines
            vectorLine.joins = Joins.Weld;
            // vectorLine.material.text
            
            Action<float> setLineWidth = newWidth => { vectorLine.lineWidth = newWidth; };
            setLineWidth(SettingsGUI.OrbitalLineWidth);
            SettingsGUI.OrbitalLineWidthChanged += setLineWidth;
        }
    }
}