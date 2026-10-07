using HarmonyLib;

namespace GreatCatchBruh.Patches;

[HarmonyPatch(typeof(FishingFloat), nameof(FishingFloat.Catch))]
internal static class FishingFloatCatchPatch
{
    private const float CatchXpBaseMultiplier = 2.0f;

    [HarmonyPostfix]
    private static void Postfix(Fish fish, Character owner)
    {
        if (fish == null || owner == null)
        {
            return;
        }

        ItemDrop itemDrop = fish.GetComponent<ItemDrop>();
        int quality = itemDrop != null ? itemDrop.m_itemData.m_quality : 1;

        float xpAward = quality * CatchXpBaseMultiplier;
        owner.RaiseSkill(Skills.SkillType.Fishing, xpAward);
    }
}
