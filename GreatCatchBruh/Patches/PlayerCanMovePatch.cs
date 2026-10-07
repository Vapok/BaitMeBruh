using HarmonyLib;

namespace GreatCatchBruh.Patches;

[HarmonyPatch(typeof(Player), nameof(Player.CanMove))]
internal static class PlayerCanMovePatch
{
    [HarmonyPostfix]
    private static void Postfix(Player __instance, ref bool __result)
    {
        if (__result || !__instance.IsAttachedToShip())
        {
            return;
        }

        ItemDrop.ItemData rightItem = __instance.GetRightItem();
        if (rightItem != null && rightItem.m_dropPrefab != null && rightItem.m_dropPrefab.name.StartsWith("FishingRod"))
        {
            __result = true;
        }
    }
}
