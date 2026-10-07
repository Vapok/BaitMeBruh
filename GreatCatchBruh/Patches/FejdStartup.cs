using HarmonyLib;

namespace GreatCatchBruh.Patches;

internal class FejdStartupPatches
{
    [HarmonyPatch(typeof(FejdStartup), nameof(FejdStartup.Awake))]
    [HarmonyAfter("org.bepinex.helpers.LocalizationManager")]
    [HarmonyBefore("org.bepinex.helpers.ItemManager")]
    internal static class FejdStartupAwakePatch
    {
        private static void Prefix()
        {
            GreatCatchBruh.Waiter.ValheimIsAwake(true);
        }
    }
}
