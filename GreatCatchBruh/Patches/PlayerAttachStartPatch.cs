using HarmonyLib;

namespace GreatCatchBruh.Patches;

[HarmonyPatch(typeof(Player), nameof(Player.AttachStart))]
internal static class PlayerAttachStartPatch
{
    [HarmonyPrefix]
    private static void Prefix(Player __instance, bool onShip, ref bool hideWeapons)
    {
        if (!onShip)
        {
            return;
        }

        ItemDrop.ItemData rightItem = __instance.GetRightItem();
        if (rightItem != null && rightItem.m_dropPrefab != null && rightItem.m_dropPrefab.name.StartsWith("FishingRod"))
        {
            hideWeapons = false;
        }
    }
}
