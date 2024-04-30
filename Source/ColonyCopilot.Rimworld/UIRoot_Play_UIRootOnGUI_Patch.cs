using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace ColonyCopilot.Rimworld;



/// <summary>
/// A Harmony Patch to add the Colony Copilot UI to the game.
/// Patches:
/// - Prefix to add the Colony Copilot UI to the game.
/// </summary>
[HarmonyPatch(typeof (UIRoot_Play), "UIRootOnGUI")]
public class UIRoot_Play_UIRootOnGUI_Patch
{

    
    /// <summary>
    /// Uses a boolean to determine if the UI should be shown.
    /// </summary>
    public static void Prefix()
    {
        CcpOverlay.Render();
    }
}