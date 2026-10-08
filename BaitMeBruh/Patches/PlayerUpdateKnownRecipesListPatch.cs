using System.Reflection;
using HarmonyLib;
using Jotunn.Managers;
using UnityEngine;

namespace BaitMeBruh.Patches;

[HarmonyPatch(typeof(Player), "UpdateKnownRecipesList")]
internal static class PlayerUpdateKnownRecipesListPatch
{
    private static readonly MethodInfo AddKnownPieceMethod = AccessTools.Method(typeof(Player), "AddKnownPiece", new[] { typeof(Piece) });

    [HarmonyPostfix]
    private static void Postfix(Player __instance)
    {
        if (__instance == null || __instance != Player.m_localPlayer)
        {
            return;
        }

        Inventory inventory = __instance.GetInventory();
        CheckAndUnlockPiece(__instance, inventory, "$item_fishnet_coastal", "piece_fishnet_coastal");
        CheckAndUnlockPiece(__instance, inventory, "$item_fishnet_deep", "piece_fishnet_deep");
    }

    private static void CheckAndUnlockPiece(Player player, Inventory inventory, string itemSharedName, string piecePrefabName)
    {
        bool hasItem = inventory != null && inventory.HaveItem(itemSharedName);
        if (!hasItem && !player.IsMaterialKnown(itemSharedName))
        {
            return;
        }

        GameObject piecePrefab = PrefabManager.Instance.GetPrefab(piecePrefabName);
        if (piecePrefab == null)
        {
            return;
        }

        Piece pieceComp = piecePrefab.GetComponent<Piece>();
        if (pieceComp == null)
        {
            return;
        }

        if (!player.IsRecipeKnown(pieceComp.m_name) && AddKnownPieceMethod != null)
        {
            AddKnownPieceMethod.Invoke(player, new object[] { pieceComp });
        }
    }
}
