using HarmonyLib;
using GreatCatchBruh.Components;

namespace GreatCatchBruh.Patches;

[HarmonyPatch(typeof(Container), nameof(Container.GetHoverText))]
internal static class ContainerGetHoverTextPatch
{
    private static void Postfix(Container __instance, ref string __result)
    {
        if (__instance == null)
        {
            return;
        }

        PassiveTrap trap = __instance.GetComponent<PassiveTrap>();
        if (trap == null)
        {
            return;
        }

        string status = trap.GetStatusHoverText();
        if (!string.IsNullOrEmpty(status))
        {
            __result += "\n" + Localization.instance.Localize(status);
        }
    }
}
