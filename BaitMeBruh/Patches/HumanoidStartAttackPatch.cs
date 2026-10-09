using HarmonyLib;

namespace BaitMeBruh.Patches;

[HarmonyPatch(typeof(Humanoid), nameof(Humanoid.StartAttack))]
internal static class HumanoidStartAttackPatch
{
    [HarmonyPrefix]
    private static bool Prefix(Humanoid __instance)
    {
        if (__instance is Player && InventoryGui.IsVisible())
        {
            ItemDrop.ItemData weapon = __instance.GetCurrentWeapon();
            if (weapon != null && weapon.m_dropPrefab != null && weapon.m_dropPrefab.name.StartsWith("FishingRod"))
            {
                return false;
            }
        }

        return true;
    }
}
