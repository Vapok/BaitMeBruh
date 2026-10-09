using HarmonyLib;
using UnityEngine;

namespace BaitMeBruh.Patches;

[HarmonyPatch(typeof(FishingFloat), nameof(FishingFloat.Setup))]
internal static class FishingFloatSetupPatch
{
    private static void Postfix(FishingFloat __instance, Character owner, ItemDrop.ItemData item)
    {
        if (__instance == null || owner == null)
        {
            return;
        }

        Humanoid humanoid = owner as Humanoid;
        ItemDrop.ItemData weapon = item != null ? item : (humanoid != null ? humanoid.GetCurrentWeapon() : null);
        if (weapon != null && weapon.m_dropPrefab != null)
        {
            if (weapon.m_dropPrefab.name == "FishingRodPrimitive")
            {
                __instance.m_maxDistance = 20.5f;
            }
            else
            {
                __instance.m_maxDistance = 30.5f;
            }
        }
    }
}
