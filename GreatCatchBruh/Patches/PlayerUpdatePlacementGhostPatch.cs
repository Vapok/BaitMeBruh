using HarmonyLib;
using UnityEngine;
using GreatCatchBruh.Components;

namespace GreatCatchBruh.Patches;

[HarmonyPatch(typeof(Player), nameof(Player.UpdatePlacementGhost))]
internal static class PlayerUpdatePlacementGhostPatch
{
    private static void Postfix(Player __instance)
    {
        if (__instance == null || __instance.m_placementGhost == null)
        {
            return;
        }

        Piece piece = __instance.m_placementGhost.GetComponent<Piece>();
        if (piece == null || !piece.m_waterPiece)
        {
            return;
        }

        PassiveTrap trap = __instance.m_placementGhost.GetComponent<PassiveTrap>();
        if (trap != null)
        {
            __instance.m_placementGhost.transform.position -= new Vector3(0f, 3f, 0f);
        }
    }
}
