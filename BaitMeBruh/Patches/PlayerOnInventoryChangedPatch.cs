using HarmonyLib;
using BaitMeBruh.Content;
using BaitMeBruh.Managers;

namespace BaitMeBruh.Patches;

[HarmonyPatch(typeof(Player), "OnInventoryChanged")]
internal static class PlayerOnInventoryChangedPatch
{
    [HarmonyPostfix]
    private static void Postfix(Player __instance)
    {
        if (__instance == null || __instance != Player.m_localPlayer)
        {
            return;
        }

        Inventory inventory = __instance.GetInventory();
        if (inventory == null)
        {
            return;
        }

        if (inventory.HaveItem("$item_fishingrod_primitive") && !__instance.HaveSeenTutorial(HuginTutorialManager.TutorialPrimitiveRod))
        {
            HuginTutorialManager.TriggerPrimitiveRodCrafted(__instance);
        }

        PieceUnlockHelper.CheckAndUnlockPiece(__instance, inventory, "$item_fishnet_coastal", "piece_fishnet_coastal", updateList: true);
        PieceUnlockHelper.CheckAndUnlockPiece(__instance, inventory, "$item_fishnet_deep", "piece_fishnet_deep", updateList: true);
    }
}
