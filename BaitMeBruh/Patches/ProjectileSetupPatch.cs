using HarmonyLib;
using BaitMeBruh.Components;
using UnityEngine;

namespace BaitMeBruh.Patches;

[HarmonyPatch(typeof(Projectile), nameof(Projectile.Setup))]
internal static class ProjectileSetupPatch
{
    private static void Postfix(Projectile __instance, Character owner, ItemDrop.ItemData item)
    {
        if (__instance == null || owner == null || item == null || item.m_dropPrefab == null)
        {
            return;
        }

        if (item.m_dropPrefab.name.StartsWith("FishingRod"))
        {
            FishingProjectileWatcher watcher = __instance.gameObject.AddComponent<FishingProjectileWatcher>();
            bool isPrimitive = item.m_dropPrefab.name == "FishingRodPrimitive";
            float maxSafeDistance = isPrimitive ? 20.0f : 30.0f;
            watcher.Setup(owner, maxSafeDistance);
        }
    }
}
