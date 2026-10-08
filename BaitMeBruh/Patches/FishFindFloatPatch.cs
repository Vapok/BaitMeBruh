using HarmonyLib;
using UnityEngine;
using BaitMeBruh.Managers;

namespace BaitMeBruh.Patches;

[HarmonyPatch(typeof(Fish), "FindFloat")]
internal static class FishFindFloatPatch
{
    private const float FogRangeMultiplier = 1.3f;

    [HarmonyPrefix]
    private static bool Prefix(Fish __instance, ref FishingFloat __result)
    {
        float rangeMultiplier = 1.0f;
        if (EnvMan.instance != null)
        {
            EnvSetup currentEnv = EnvMan.instance.GetCurrentEnvironment();
            if (currentEnv != null && (currentEnv.m_name.IndexOf("fog", System.StringComparison.OrdinalIgnoreCase) >= 0 || currentEnv.m_fogDensityDay > 0.03f))
            {
                rangeMultiplier = FogRangeMultiplier;
            }
        }

        float baseDetectionRange = MorningFishManager.GetDetectionRange();
        float hookChance = MorningFishManager.GetBaseHookChance(__instance.m_baseHookChance);

        foreach (FishingFloat floatInstance in FishingFloat.GetAllInstances())
        {
            if (floatInstance == null || !floatInstance.IsInWater() || floatInstance.GetCatch() != null)
            {
                continue;
            }

            float effectiveRange = Mathf.Max(floatInstance.m_range, baseDetectionRange) * rangeMultiplier;
            float distance = Vector3.Distance(__instance.transform.position, floatInstance.transform.position);

            if (distance <= effectiveRange)
            {
                if (Random.value < hookChance)
                {
                    __result = floatInstance;
                    return false;
                }
            }
        }

        __result = null;
        return false;
    }
}
