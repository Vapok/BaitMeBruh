using System.Reflection;
using HarmonyLib;
using Jotunn.Managers;
using UnityEngine;
using BaitMeBruh.Managers;

namespace BaitMeBruh.Patches;

[HarmonyPatch(typeof(Player), "OnInventoryChanged")]
internal static class PlayerOnInventoryChangedPatch
{
    private static readonly MethodInfo AddKnownPieceMethod = AccessTools.Method(typeof(Player), "AddKnownPiece", new[] { typeof(Piece) });
    private static readonly MethodInfo UpdateAvailablePiecesListMethod = AccessTools.Method(typeof(Player), "UpdateAvailablePiecesList");

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

        CheckAndUnlockPiece(__instance, inventory, "$item_fishnet_coastal", "piece_fishnet_coastal");
        CheckAndUnlockPiece(__instance, inventory, "$item_fishnet_deep", "piece_fishnet_deep");
    }

    private static void CheckAndUnlockPiece(Player player, Inventory inventory, string itemSharedName, string piecePrefabName)
    {
        if (!inventory.HaveItem(itemSharedName) && !player.IsMaterialKnown(itemSharedName))
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

        if (!player.IsRecipeKnown(pieceComp.m_name))
        {
            if (AddKnownPieceMethod != null)
            {
                AddKnownPieceMethod.Invoke(player, new object[] { pieceComp });
            }

            if (UpdateAvailablePiecesListMethod != null)
            {
                UpdateAvailablePiecesListMethod.Invoke(player, null);
            }
        }
    }
}
