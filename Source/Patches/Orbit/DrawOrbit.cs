using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using Vectrosity;

namespace ThickerTrajectoryLines
{
    [HarmonyPatch(typeof(OrbitRendererBase))]
    [HarmonyPatch("MakeLine")]
    
    public class Patch_OrbitRendererBase_MakeLine
    {
        private static Type OrbitRendererType = typeof(OrbitRenderer);
        private static Type ContractOrbitRendererType = typeof(ContractOrbitRenderer);
        
        static void Postfix(OrbitRendererBase __instance, VectorLine l)
        {
            var log = new Logger("Patch_OrbitRendererBase_MakeLine/Postfix");
            
            log.Verbose("Running");
            
            var type = __instance.GetType();

            // OrbitRenderer - renders vessel and body orbits; skip body (non-vessel) orbits
            if (type == typeof(OrbitRenderer) && __instance.vessel != null)
            {
                // good
                log.Verbose("Detected as a vessel orbit");
            }
            // ContractOrbitRenderer - renders contract orbits
            else if (type == typeof(ContractOrbitRenderer))
            {
                // good
                log.Verbose("Detected as a contract orbit");
            }
            else
            {
                return;
            }
            
            l.joins = Joins.Weld;
            
            Action<float> setLineWidth = newWidth => { l.lineWidth = newWidth; };
            setLineWidth(SettingsGUI.Instance.OrbitalLineWidth);
            SettingsGUI.Instance.OrbitalLineWidthChanged += setLineWidth;
            
            // keep a tab at requested texture for the line
            if (Globals.GlowFade)
            {
                var oldTexture = l.material.mainTexture;
                void SetLineTexture(bool toggle)
                {
                    // log.Debug("Switching texture to: " + toggle);
                    l.material.mainTexture = toggle
                        ? Globals.GlowFade
                        : oldTexture;
                }
                SetLineTexture(SettingsGUI.Instance.UseSolidGlowFadeTrajectoryTexture);
                SettingsGUI.Instance.UseSolidGlowFadeTrajectoryTextureChanged += SetLineTexture;
            }
            // Debug.Log("[wawa] Line width set");
        }
    }
}