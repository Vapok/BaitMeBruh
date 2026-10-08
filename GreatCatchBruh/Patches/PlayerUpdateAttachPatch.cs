using HarmonyLib;
using UnityEngine;

namespace GreatCatchBruh.Patches;

[HarmonyPatch(typeof(Player), "UpdateAttach")]
internal static class PlayerUpdateAttachPatch
{
    private const float MinStarboardOffset = 0.15f;

    [HarmonyPostfix]
    private static void Postfix(Player __instance)
    {
        if (__instance == null || !__instance.IsAttached())
        {
            return;
        }

        if (__instance.InBed() || __instance.GetDoodadController() != null)
        {
            return;
        }

        ItemDrop.ItemData currentWeapon = __instance.GetCurrentWeapon();
        if (currentWeapon == null || currentWeapon.m_dropPrefab == null || !currentWeapon.m_dropPrefab.name.StartsWith("FishingRod"))
        {
            return;
        }

        Transform attachPoint = __instance.GetAttachPoint();
        if (attachPoint == null)
        {
            return;
        }

        Ship ship = attachPoint.GetComponentInParent<Ship>();
        if (ship == null)
        {
            ship = __instance.GetComponentInParent<Ship>();
        }

        if (ship == null)
        {
            return;
        }

        Vector3 localPos = ship.transform.InverseTransformPoint(attachPoint.position);
        Vector3 shipUp = ship.transform.up;

        if (localPos.x > MinStarboardOffset)
        {
            Vector3 desiredForward = -ship.transform.forward;
            __instance.transform.rotation = Quaternion.LookRotation(desiredForward, shipUp);
        }
        else if (localPos.x < -MinStarboardOffset)
        {
            Vector3 desiredForward = ship.transform.forward;
            __instance.transform.rotation = Quaternion.LookRotation(desiredForward, shipUp);
        }
    }
}
