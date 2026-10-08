using HarmonyLib;

namespace BaitMeBruh.Patches;

[HarmonyPatch(typeof(Player), nameof(Player.AttachStart))]
internal static class PlayerAttachStartPatch
{
    [HarmonyPrefix]
    private static void Prefix(Player __instance, bool isBed, ref bool hideWeapons)
    {
        if (isBed || __instance.GetDoodadController() != null)
        {
            return;
        }

        ItemDrop.ItemData currentWeapon = __instance.GetCurrentWeapon();
        if (currentWeapon != null && currentWeapon.m_dropPrefab != null && currentWeapon.m_dropPrefab.name.StartsWith("FishingRod"))
        {
            hideWeapons = false;
        }
    }
}
