using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using GreatCatchBruh.Components;
using GreatCatchBruh.Managers;

namespace GreatCatchBruh.Patches;

[HarmonyPatch(typeof(FishingFloat), "FixedUpdate")]
internal static class FishingFloatFixedUpdatePatch
{
    private static readonly MethodInfo _useStaminaMethod =
        AccessTools.Method(typeof(Character), nameof(Character.UseStamina), new[] { typeof(float) });

    private static readonly MethodInfo _interceptHookedMethod =
        AccessTools.Method(typeof(FishingFloatFixedUpdatePatch), nameof(InterceptHookedUseStamina));

    private static readonly MethodInfo _interceptReelMethod =
        AccessTools.Method(typeof(FishingFloatFixedUpdatePatch), nameof(InterceptReelUseStamina));

    private static readonly AccessTools.FieldRef<FishingFloat, float> _lineLengthRef =
        AccessTools.FieldRefAccess<FishingFloat, float>("m_lineLength");

    [HarmonyPrefix]
    private static bool Prefix(FishingFloat __instance)
    {
        Character owner = __instance.GetOwner();
        if (owner != null && (owner.IsAttachedToShip() || owner.GetStandingOnShip() != null))
        {
            Transform rodTop = __instance.GetRodTop(owner);
            if (rodTop != null)
            {
                float currentDistance = Vector3.Distance(rodTop.position, __instance.transform.position);
                float lineLength = _lineLengthRef(__instance);
                if (currentDistance > lineLength && currentDistance <= __instance.m_maxDistance)
                {
                    _lineLengthRef(__instance) = currentDistance;
                }
            }
        }

        Fish fish = __instance.GetCatch();
        FishingLineState lineState = __instance.GetComponent<FishingLineState>();

        if (fish == null)
        {
            if (lineState != null)
            {
                lineState.ResetTension();
            }
            return true;
        }

        if (lineState == null)
        {
            lineState = __instance.gameObject.AddComponent<FishingLineState>();
        }

        if (owner == null)
        {
            return true;
        }

        bool isStruggling = fish.IsEscaping();
        bool isReeling = owner.IsBlocking() && owner.HaveStamina();

        ItemDrop itemDrop = fish.GetComponent<ItemDrop>();
        int quality = itemDrop != null ? itemDrop.m_itemData.m_quality : 1;

        float skillFactor = owner.GetSkillFactor(Skills.SkillType.Fishing);
        float rodDamping = FishingTensionManager.GetRodTensionDamping(owner);

        lineState.UpdateTension(Time.fixedDeltaTime, isStruggling, isReeling, quality, skillFactor, rodDamping);

        if (lineState.IsSnapped)
        {
            __instance.Message("$msg_fishing_linebroke", true);
            fish.OnHooked(null);

            ZNetView netView = __instance.GetComponent<ZNetView>();
            if (netView != null && netView.IsValid())
            {
                netView.Destroy();
            }

            if (__instance.m_lineBreakEffect != null)
            {
                __instance.m_lineBreakEffect.Create(__instance.transform.position, Quaternion.identity);
            }

            return false;
        }

        return true;
    }

    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        int useStaminaCount = 0;

        foreach (CodeInstruction instruction in instructions)
        {
            if (instruction.Calls(_useStaminaMethod))
            {
                useStaminaCount++;

                yield return new CodeInstruction(OpCodes.Ldarg_0);

                if (useStaminaCount == 1)
                {
                    yield return new CodeInstruction(OpCodes.Call, _interceptHookedMethod);
                }
                else
                {
                    yield return new CodeInstruction(OpCodes.Call, _interceptReelMethod);
                }
            }
            else
            {
                yield return instruction;
            }
        }
    }

    public static void InterceptHookedUseStamina(Character owner, float fallbackStamina, FishingFloat floatInstance)
    {
        if (floatInstance == null)
        {
            owner.UseStamina(fallbackStamina);
            return;
        }

        Fish fish = floatInstance.GetCatch();
        FishingTensionManager.ApplyHookedStaminaDrain(owner, fallbackStamina, floatInstance, fish);
    }

    public static void InterceptReelUseStamina(Character owner, float fallbackStamina, FishingFloat floatInstance)
    {
        if (floatInstance == null)
        {
            owner.UseStamina(fallbackStamina);
            return;
        }

        Fish fish = floatInstance.GetCatch();
        FishingTensionManager.ApplyReelStaminaDrain(owner, fallbackStamina, floatInstance, fish);
    }
}
