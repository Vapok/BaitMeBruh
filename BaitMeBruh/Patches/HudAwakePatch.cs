using HarmonyLib;
using BaitMeBruh.UI;

namespace BaitMeBruh.Patches;

[HarmonyPatch(typeof(Hud), "Awake")]
internal static class HudAwakePatch
{
    [HarmonyPostfix]
    private static void Postfix(Hud __instance)
    {
        FishingTensionHud.EnsureInitialized(__instance);
    }
}
