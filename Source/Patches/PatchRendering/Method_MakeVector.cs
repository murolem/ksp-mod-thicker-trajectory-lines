using System;
using HarmonyLib;
using UnityEngine;
using Vectrosity;

namespace ThickerTrajectoryLines
{
    [HarmonyPatch(typeof(PatchRendering), nameof(PatchRendering.MakeVector))]
    public class Patch_PatchRendering_MakeVector
    {
        static void Postfix(PatchRendering __instance)
        {
            var log = new Logger("Patch_PatchRendering_MakeVector/Postfix");
            
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
            
            // adjust width immediately and keep a tab at it
            void SetLineWidth(float newWidth)
            {
                vectorLine.lineWidth = newWidth;
            }
            SetLineWidth(SettingsGUI.Instance.OrbitalLineWidth);
            SettingsGUI.Instance.OrbitalLineWidthChanged += SetLineWidth;
            
            // keep a tab at requested texture for the line
            if (Globals.GlowFade)
            {
                var oldTexture = vectorLine.material.mainTexture;
                void SetLineTexture(bool toggle)
                {
                    // log.Debug("Switching texture to: " + toggle);
                    vectorLine.material.mainTexture = toggle
                        ? Globals.GlowFade
                        : oldTexture;
                }
                SetLineTexture(SettingsGUI.Instance.UseSolidGlowFadeTrajectoryTexture);
                SettingsGUI.Instance.UseSolidGlowFadeTrajectoryTextureChanged += SetLineTexture;
            }
        }
    }
}