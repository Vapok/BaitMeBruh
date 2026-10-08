using UnityEngine;
using BaitMeBruh.Components;

namespace BaitMeBruh.Managers;

public static class FishingTensionManager
{
    private const float BaseReelStamina = 2.0f;
    private const float TensionReelStaminaFactor = 2.5f;
    private const float BaseHookedStamina = 0.3f;
    private const float TensionHookedStaminaFactor = 0.5f;

    private const float DefaultRodDamping = 1.3f;
    private const float PrimitiveRodDamping = 1.0f;
    private const float ReinforcedRodDamping = 1.6f;

    public static float GetRodTensionDamping(Character owner)
    {
        if (owner is Humanoid humanoid)
        {
            ItemDrop.ItemData equippedItem = humanoid.GetRightItem();
            if (equippedItem != null && equippedItem.m_dropPrefab != null)
            {
                string prefabName = equippedItem.m_dropPrefab.name;
                if (prefabName == "FishingRodPrimitive")
                {
                    return PrimitiveRodDamping;
                }
                if (prefabName == "FishingRodReinforced")
                {
                    return ReinforcedRodDamping;
                }
            }
        }

        return DefaultRodDamping;
    }

    public static float CalculateReelStaminaDrain(float tension, int fishQuality, float skillFactor, float fixedDeltaTime)
    {
        float drainPerSecond = (BaseReelStamina + TensionReelStaminaFactor * tension * fishQuality) * (1.0f - 0.5f * skillFactor);
        return drainPerSecond * fixedDeltaTime;
    }

    public static float CalculateHookedStaminaDrain(float tension, float skillFactor, float fixedDeltaTime)
    {
        float drainPerSecond = (BaseHookedStamina + TensionHookedStaminaFactor * tension) * (1.0f - 0.5f * skillFactor);
        return drainPerSecond * fixedDeltaTime;
    }

    public static void ApplyHookedStaminaDrain(Character owner, float fallbackStamina, FishingFloat floatInstance, Fish fish)
    {
        if (owner == null || fish == null)
        {
            return;
        }

        if (owner.IsBlocking())
        {
            return;
        }

        FishingLineState lineState = floatInstance != null ? floatInstance.GetComponent<FishingLineState>() : null;
        if (lineState == null)
        {
            owner.UseStamina(fallbackStamina);
            return;
        }

        float skillFactor = owner.GetSkillFactor(Skills.SkillType.Fishing);
        float staminaDrain = CalculateHookedStaminaDrain(lineState.CurrentTension, skillFactor, Time.fixedDeltaTime);
        owner.UseStamina(staminaDrain);
    }

    public static void ApplyReelStaminaDrain(Character owner, float fallbackStamina, FishingFloat floatInstance, Fish fish)
    {
        if (owner == null)
        {
            return;
        }

        if (fish == null)
        {
            owner.UseStamina(fallbackStamina);
            return;
        }

        FishingLineState lineState = floatInstance != null ? floatInstance.GetComponent<FishingLineState>() : null;
        if (lineState == null)
        {
            owner.UseStamina(fallbackStamina);
            return;
        }

        ItemDrop itemDrop = fish.GetComponent<ItemDrop>();
        int quality = itemDrop != null ? itemDrop.m_itemData.m_quality : 1;
        float skillFactor = owner.GetSkillFactor(Skills.SkillType.Fishing);
        float staminaDrain = CalculateReelStaminaDrain(lineState.CurrentTension, quality, skillFactor, Time.fixedDeltaTime);
        owner.UseStamina(staminaDrain);
    }
}
