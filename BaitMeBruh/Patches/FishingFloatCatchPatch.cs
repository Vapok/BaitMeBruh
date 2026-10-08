using HarmonyLib;
using UnityEngine;
using BaitMeBruh.Managers;

namespace BaitMeBruh.Patches;

[HarmonyPatch(typeof(FishingFloat), nameof(FishingFloat.Catch))]
internal static class FishingFloatCatchPatch
{
    private const float CatchXpBaseMultiplier = 2.0f;

    [HarmonyPostfix]
    private static void Postfix(FishingFloat __instance, Fish fish, Character owner)
    {
        if (fish == null || owner == null)
        {
            return;
        }

        ItemDrop itemDrop = fish.GetComponent<ItemDrop>();
        int quality = itemDrop != null ? itemDrop.m_itemData.m_quality : 1;

        float xpAward = quality * CatchXpBaseMultiplier;
        owner.RaiseSkill(Skills.SkillType.Fishing, xpAward);

        if (owner is Player player)
        {
            HuginTutorialManager.TriggerFirstFishCaught(player);

            if (player.m_helmetItem != null && player.m_helmetItem.m_dropPrefab != null && player.m_helmetItem.m_dropPrefab.name == "HelmetFishingHat")
            {
                if (UnityEngine.Random.value < 0.25f && __instance != null)
                {
                    string baitName = __instance.GetBait();
                    if (!string.IsNullOrEmpty(baitName) && ZNetScene.instance != null)
                    {
                        GameObject baitPrefab = ZNetScene.instance.GetPrefab(baitName);
                        if (baitPrefab != null)
                        {
                            player.GetInventory().AddItem(baitPrefab, 1);
                            player.Message(MessageHud.MessageType.Center, "$msg_bait_salvaged");
                        }
                    }
                }
            }
        }
    }
}
