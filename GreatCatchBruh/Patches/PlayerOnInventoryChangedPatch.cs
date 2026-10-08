using HarmonyLib;
using GreatCatchBruh.Managers;

namespace GreatCatchBruh.Patches;

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

        if (__instance.HaveSeenTutorial(HuginTutorialManager.TutorialPrimitiveRod))
        {
            return;
        }

        Inventory inventory = __instance.GetInventory();
        if (inventory != null && inventory.HaveItem("$item_fishingrod_primitive"))
        {
            HuginTutorialManager.TriggerPrimitiveRodCrafted(__instance);
        }
    }
}
