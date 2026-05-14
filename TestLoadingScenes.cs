// using System;
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace ThickerTrajectoryLines
// {   
//     [KSPAddon(KSPAddon.Startup.FlightEditorAndKSC, false)]
//     public class TestModFlightEditorAndKSC : MonoBehaviour
//     {
//         public void Start()
//         {
//             Debug.Log("[WAWA] FlightEditorAndKSC");
//         }
//     }
//     [KSPAddon(KSPAddon.Startup.AllGameScenes, false)]
//     public class TestModAllGameScenes : MonoBehaviour
//     {
//         public void Start()
//         {
//             Debug.Log("[WAWA] AllGameScenes");
//         }
//     }
//     [KSPAddon(KSPAddon.Startup.FlightAndEditor, false)]
//     public class TestModFlightAndEditor : MonoBehaviour
//     {
//         public void Start()
//         {
//             Debug.Log("[WAWA] FlightAndEditor");
//         }
//     }
//     [KSPAddon(KSPAddon.Startup.FlightAndKSC, false)]
//     public class TestModFlightAndKSC : MonoBehaviour
//     {
//         public void Start()
//         {
//             Debug.Log("[WAWA] FlightAndKSC");
//         }
//     }
//     [KSPAddon(KSPAddon.Startup.Instantly, false)]
//     public class TestModInstantly : MonoBehaviour
//     {
//         public void Start()
//         {
//             Debug.Log("[WAWA] Instantly");
//         }
//     }
//     [KSPAddon(KSPAddon.Startup.EveryScene, false)]
//     public class TestModEveryScene : MonoBehaviour
//     {
//         public void Start()
//         {
//             Debug.Log("[WAWA] EveryScene");
//         }
//     }
//     [KSPAddon(KSPAddon.Startup.MainMenu, false)]
//     public class TestModMainMenu : MonoBehaviour
//     {
//         public void Start()
//         {
//             Debug.Log("[WAWA] MainMenu");
//         }
//     }
//     [KSPAddon(KSPAddon.Startup.Settings, false)]
//     public class TestModSettings : MonoBehaviour
//     {
//         public void Start()
//         {
//             Debug.Log("[WAWA] Settings");
//         }
//     }
//     [KSPAddon(KSPAddon.Startup.Credits, false)]
//     public class TestModCredits : MonoBehaviour
//     {
//         public void Start()
//         {
//             Debug.Log("[WAWA] Credits");
//         }
//     }
//     [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
//     public class TestModSpaceCentre : MonoBehaviour
//     {
//         public void Start()
//         {
//             Debug.Log("[WAWA] SpaceCentre");
//         }
//     }
//     [KSPAddon(KSPAddon.Startup.EditorAny, false)]
//     public class TestModEditorAny : MonoBehaviour
//     {
//         public void Start()
//         {
//             Debug.Log("[WAWA] EditorAny");
//         }
//     }
//     [KSPAddon(KSPAddon.Startup.EditorSPH, false)]
//     public class TestModEditorSPH : MonoBehaviour
//     {
//         public void Start()
//         {
//             Debug.Log("[WAWA] EditorSPH");
//         }
//     }
//     [KSPAddon(KSPAddon.Startup.EditorVAB, false)]
//     public class TestModEditorVAB : MonoBehaviour
//     {
//         public void Start()
//         {
//             Debug.Log("[WAWA] EditorVAB");
//         }
//     }
//     [KSPAddon(KSPAddon.Startup.Flight, false)]
//     public class TestModFlight : MonoBehaviour
//     {
//         public void Start()
//         {
//             Debug.Log("[WAWA] Flight");
//         }
//     }
//     [KSPAddon(KSPAddon.Startup.TrackingStation, false)]
//     public class TestModTrackingStation : MonoBehaviour
//     {
//         public void Start()
//         {
//             Debug.Log("[WAWA] TrackingStation");
//         }
//     }
//     [KSPAddon(KSPAddon.Startup.PSystemSpawn, false)]
//     public class TestModPSystemSpawn : MonoBehaviour
//     {
//         public void Start()
//         {
//             Debug.Log("[WAWA] PSystemSpawn");
//         }
//     }
//     
//     // [KSPAddon(KSPAddon.Startup.MainMenu, false)]
//     // public class TestMod : MonoBehaviour
//     // {
//     //     public void Start()
//     //     {
//     //         Debug.Log("wawa");
//     //     }
//     //
//     //     // public void Update()
//     //     // {
//     //     //     Debug.Log("wawa");
//     //     //     bool key = Input.GetKeyDown(KeyCode.P);
//     //     //     
//     //     //     Harmony
//     //     //     
//     //     //     // OrbitRendererBase.MakeL
//     //     //     if (key)
//     //     //     {
//     //     //         List<Part> parts = FlightGlobals.ActiveVessel.parts;
//     //     //         int index;
//     //     //         System.Random rnd =  new System.Random();
//     //     //         index = rnd.Next(1, parts.Count); //we ignore the root part by starting at 1
//     //     //         parts[index].explode();
//     //     //         Debug.Log("wawa!!");
//     //     //     }
//     //     // }
//     // }
// }