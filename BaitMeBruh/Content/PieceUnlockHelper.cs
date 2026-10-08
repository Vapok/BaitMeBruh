using System.Reflection;
using HarmonyLib;
using Jotunn.Managers;
using UnityEngine;

namespace BaitMeBruh.Content;

internal static class PieceUnlockHelper
{
    private static readonly MethodInfo AddKnownPieceMethod = AccessTools.Method(typeof(Player), "AddKnownPiece", new[] { typeof(Piece) });
    private static readonly MethodInfo UpdateAvailablePiecesListMethod = AccessTools.Method(typeof(Player), "UpdateAvailablePiecesList");

    public static void CheckAndUnlockPiece(Player player, Inventory inventory, string itemSharedName, string piecePrefabName, bool updateList = false)
    {
        if (player == null)
        {
            return;
        }

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

        if (!player.IsRecipeKnown(pieceComp.m_name))
        {
            if (AddKnownPieceMethod != null)
            {
                AddKnownPieceMethod.Invoke(player, new object[] { pieceComp });
            }

            if (updateList && UpdateAvailablePiecesListMethod != null)
            {
                UpdateAvailablePiecesListMethod.Invoke(player, null);
            }
        }
    }
}
