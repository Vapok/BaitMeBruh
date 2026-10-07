using HarmonyLib;
using UnityEngine;

namespace GreatCatchBruh.Patches;

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

        foreach (FishingFloat floatInstance in FishingFloat.GetAllInstances())
        {
            if (floatInstance == null || !floatInstance.IsInWater() || floatInstance.GetCatch() != null)
            {
                continue;
            }

            float effectiveRange = floatInstance.m_range * rangeMultiplier;
            float distance = Vector3.Distance(__instance.transform.position, floatInstance.transform.position);

            if (distance <= effectiveRange)
            {
                if (Random.value < __instance.m_baseHookChance)
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
