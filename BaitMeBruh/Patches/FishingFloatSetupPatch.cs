using HarmonyLib;

namespace BaitMeBruh.Patches;

[HarmonyPatch(typeof(FishingFloat), nameof(FishingFloat.Setup))]
internal static class FishingFloatSetupPatch
{
    private const float PrimitiveMaxDistance = 20.0f;
    private const float PrimitivePullLineSpeed = 1.2f;

    [HarmonyPostfix]
    private static void Postfix(FishingFloat __instance, ItemDrop.ItemData item)
    {
        if (item != null && item.m_dropPrefab != null && item.m_dropPrefab.name == "FishingRodPrimitive")
        {
            __instance.m_maxDistance = PrimitiveMaxDistance;
            __instance.m_pullLineSpeed = PrimitivePullLineSpeed;
        }
    }
}
