using HarmonyLib;
using UnityEngine;

namespace GreatCatchBruh.Patches;

[HarmonyPatch(typeof(Fish), nameof(Fish.Interact))]
internal static class FishInteractPatch
{
    private static readonly AccessTools.FieldRef<Fish, FishingFloat> _fishingFloatRef =
        AccessTools.FieldRefAccess<Fish, FishingFloat>("m_fishingFloat");

    [HarmonyPrefix]
    private static bool Prefix(Fish __instance, Humanoid character, bool repeat, bool alt, ref bool __result)
    {
        if (repeat)
        {
            __result = false;
            return false;
        }

        if (!__instance.IsOutOfWater())
        {
            __result = false;
            return false;
        }

        if (!__instance.IsHooked())
        {
            return true;
        }

        FishingFloat fishingFloat = _fishingFloatRef(__instance);
        if (fishingFloat == null)
        {
            return true;
        }

        string catchMessage = FishingFloat.Catch(__instance, character);
        character.Message(MessageHud.MessageType.Center, Localization.instance.Localize(catchMessage));

        fishingFloat.SetCatch(null);
        __instance.OnHooked(null);

        ZNetView netView = fishingFloat.GetComponent<ZNetView>();
        if (netView != null && netView.IsValid())
        {
            netView.Destroy();
        }

        __result = true;
        return false;
    }
}
