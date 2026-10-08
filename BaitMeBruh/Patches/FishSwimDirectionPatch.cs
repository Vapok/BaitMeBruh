using HarmonyLib;
using UnityEngine;
using BaitMeBruh.Managers;

namespace BaitMeBruh.Patches;

[HarmonyPatch(typeof(Fish), "SwimDirection")]
internal static class FishSwimDirectionPatch
{
    private static readonly AccessTools.FieldRef<Fish, FishingFloat> _waypointFFRef =
        AccessTools.FieldRefAccess<Fish, FishingFloat>("m_waypointFF");

    [HarmonyPrefix]
    private static void Prefix(Fish __instance, ref bool fast)
    {
        if (!fast && MorningFishManager.IsMorningFishingHour())
        {
            FishingFloat targetFloat = _waypointFFRef(__instance);
            if (targetFloat != null)
            {
                fast = true;
            }
        }
    }
}
