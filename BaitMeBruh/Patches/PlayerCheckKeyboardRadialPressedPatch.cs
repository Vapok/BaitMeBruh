using HarmonyLib;
using UnityEngine;

namespace BaitMeBruh.Patches;

[HarmonyPatch(typeof(Player), "CheckKeyboardRadialPressed")]
internal static class PlayerCheckKeyboardRadialPressedPatch
{
    private static bool Prefix(Player __instance, ref bool __result)
    {
        if (__instance == null)
        {
            return true;
        }

        ItemDrop.ItemData currentWeapon = __instance.GetCurrentWeapon();
        if (currentWeapon != null && currentWeapon.m_dropPrefab != null && currentWeapon.m_dropPrefab.name.StartsWith("FishingRod"))
        {
            __result = false;
            return false;
        }

        return true;
    }
}
