using HarmonyLib;
using UnityEngine;
using BaitMeBruh.Managers;

namespace BaitMeBruh.Patches;

[HarmonyPatch(typeof(FishingFloat), "TryToHook")]
internal static class FishingFloatTryToHookPatch
{
    private const float BaseHookWindow = 0.8f;
    private const float MaxSkillWindowBonus = 0.7f;
    private const float DawnDuskMultiplier = 1.4f;
    private const float HookXpAward = 0.5f;

    private static readonly AccessTools.FieldRef<FishingFloat, Fish> _nibblerRef =
        AccessTools.FieldRefAccess<FishingFloat, Fish>("m_nibbler");

    private static readonly AccessTools.FieldRef<FishingFloat, float> _nibbleTimeRef =
        AccessTools.FieldRefAccess<FishingFloat, float>("m_nibbleTime");

    [HarmonyPrefix]
    private static bool Prefix(FishingFloat __instance)
    {
        Fish nibbler = _nibblerRef(__instance);
        if (nibbler == null || __instance.GetCatch() != null)
        {
            return false;
        }

        Character owner = __instance.GetOwner();
        float skillFactor = owner != null ? owner.GetSkillFactor(Skills.SkillType.Fishing) : 0.0f;

        float allowedWindow = BaseHookWindow + (skillFactor * MaxSkillWindowBonus);

        if (EnvMan.instance != null)
        {
            float dayFraction = EnvMan.instance.GetDayFraction();
            float dawnDusk = 1.0f - Mathf.Abs(Mathf.Abs(dayFraction * 2.0f - 1.0f) - 0.5f) * 2.0f;
            if (dawnDusk > 0.3f)
            {
                allowedWindow *= DawnDuskMultiplier;
            }
        }

        float nibbleTime = _nibbleTimeRef(__instance);
        if (Time.time - nibbleTime < allowedWindow)
        {
            __instance.SetCatch(nibbler);
            _nibblerRef(__instance) = null;
            Game.instance.IncrementPlayerStat(PlayerStatType.FishHooked);

            if (owner != null)
            {
                owner.RaiseSkill(Skills.SkillType.Fishing, HookXpAward);
            }

            FishingCueManager.TriggerHookedCue(__instance, owner, nibbler);
        }

        return false;
    }
}
