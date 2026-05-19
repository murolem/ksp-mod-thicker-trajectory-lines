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
            void SetLineWidth(float newWidth)
            {
                l.lineWidth = newWidth;
            }

            // good - OrbitRenderer - renders vessel and body orbits
            if (type == typeof(OrbitRenderer))
            {
                if (__instance.vessel)
                {
                    log.Verbose("Detected non-body, non-active-object orbit");
                    
                    SetLineWidth(SettingsGUI.Instance.InactiveObjectLineWidth);
                    SettingsGUI.Instance.InactiveObjectLineWidthChanged += SetLineWidth;
                }
                else
                {
                    log.Verbose("Detected body orbit");
                 
                    SetLineWidth(SettingsGUI.Instance.BodyOrbitsLineWidth);
                    SettingsGUI.Instance.BodyOrbitsLineWidthChanged += SetLineWidth;
                }
            }
            // good - ContractOrbitRenderer - renders contract orbits
            else if (type == typeof(ContractOrbitRenderer))
            {
                log.Verbose("Detected contract orbit");
                
                SetLineWidth(SettingsGUI.Instance.ContractOrbitsLineWidth);
                SettingsGUI.Instance.ContractOrbitsLineWidthChanged += SetLineWidth;
            }
            else
            {
                log.Verbose("Detected some other orbit, skipping");
                return;
            }
            
            // makes good looking lines
            l.joins = Joins.Weld;
            
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