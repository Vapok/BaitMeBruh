using HarmonyLib;
using UnityEngine;
using BaitMeBruh.Managers;

namespace BaitMeBruh.Patches;

[HarmonyPatch(typeof(Fish), "TestBate")]
internal static class FishTestBatePatch
{
    private const float RainBiteRateMultiplier = 1.5f;

    [HarmonyPrefix]
    private static bool Prefix(Fish __instance, FishingFloat ff, ref bool __result)
    {
        if (ff == null || __instance == null)
        {
            return true;
        }

        string bait = ff.GetBait();
        if (string.IsNullOrEmpty(bait))
        {
            __result = false;
            return false;
        }

        bool isMorning = MorningFishManager.IsMorningFishingHour();
        float weatherMultiplier = 1.0f;
        if (EnvMan.IsWet())
        {
            weatherMultiplier = RainBiteRateMultiplier;
        }

        foreach (Fish.BaitSetting setting in __instance.m_baits)
        {
            if (setting.m_bait != null && setting.m_bait.name == bait)
            {
                if (isMorning)
                {
                    __result = true;
                    return false;
                }

                float effectiveChance = Mathf.Clamp01(setting.m_chance * weatherMultiplier);
                if (Random.value < effectiveChance)
                {
                    __result = true;
                    return false;
                }
            }
        }

        __result = false;
        return false;
    }
}
