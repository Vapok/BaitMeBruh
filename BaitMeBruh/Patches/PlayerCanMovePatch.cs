using HarmonyLib;

namespace BaitMeBruh.Patches;

[HarmonyPatch(typeof(Player), nameof(Player.CanMove))]
internal static class PlayerCanMovePatch
{
    [HarmonyPostfix]
    private static void Postfix(Player __instance, ref bool __result)
    {
        if (__result)
        {
            return;
        }

        if ((!__instance.IsAttached() && !__instance.InEmote()) || __instance.InBed() || __instance.GetDoodadController() != null)
        {
            return;
        }

        ItemDrop.ItemData currentWeapon = __instance.GetCurrentWeapon();
        if (currentWeapon != null && currentWeapon.m_dropPrefab != null && currentWeapon.m_dropPrefab.name.StartsWith("FishingRod"))
        {
            __result = true;
        }
    }
}
