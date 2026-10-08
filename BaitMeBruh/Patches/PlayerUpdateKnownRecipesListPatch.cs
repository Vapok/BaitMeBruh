using HarmonyLib;
using BaitMeBruh.Content;

namespace BaitMeBruh.Patches;

[HarmonyPatch(typeof(Player), "UpdateKnownRecipesList")]
internal static class PlayerUpdateKnownRecipesListPatch
{
    [HarmonyPostfix]
    private static void Postfix(Player __instance)
    {
        if (__instance == null || __instance != Player.m_localPlayer)
        {
            return;
        }

        Inventory inventory = __instance.GetInventory();
        PieceUnlockHelper.CheckAndUnlockPiece(__instance, inventory, "$item_fishnet_coastal", "piece_fishnet_coastal");
        PieceUnlockHelper.CheckAndUnlockPiece(__instance, inventory, "$item_fishnet_deep", "piece_fishnet_deep");
    }
}
