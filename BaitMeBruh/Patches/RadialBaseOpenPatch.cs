using HarmonyLib;
using UnityEngine;
using Valheim.UI;

namespace BaitMeBruh.Patches;

[HarmonyPatch(typeof(RadialBase), nameof(RadialBase.Open))]
internal static class RadialBaseOpenPatch
{
    private static bool Prefix()
    {
        Player localPlayer = Player.m_localPlayer;
        if (localPlayer == null)
        {
            return true;
        }

        ItemDrop.ItemData currentWeapon = localPlayer.GetCurrentWeapon();
        if (currentWeapon != null && currentWeapon.m_dropPrefab != null && currentWeapon.m_dropPrefab.name.StartsWith("FishingRod"))
        {
            return false;
        }

        return true;
    }
}
