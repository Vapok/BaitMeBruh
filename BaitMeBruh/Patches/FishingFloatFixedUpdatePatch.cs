using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using BaitMeBruh.Components;
using BaitMeBruh.Managers;

namespace BaitMeBruh.Patches;

[HarmonyPatch(typeof(FishingFloat), "FixedUpdate")]
internal static class FishingFloatFixedUpdatePatch
{
    private static readonly MethodInfo _useStaminaMethod =
        AccessTools.Method(typeof(Character), nameof(Character.UseStamina), new[] { typeof(float) });

    private static readonly MethodInfo _interceptHookedMethod =
        AccessTools.Method(typeof(FishingFloatFixedUpdatePatch), nameof(InterceptHookedUseStamina));

    private static readonly MethodInfo _interceptReelMethod =
        AccessTools.Method(typeof(FishingFloatFixedUpdatePatch), nameof(InterceptReelUseStamina));

    private static readonly MethodInfo _raiseSkillMethod =
        AccessTools.Method(typeof(Character), nameof(Character.RaiseSkill), new[] { typeof(Skills.SkillType), typeof(float) });

    private static readonly MethodInfo _interceptRaiseSkillMethod =
        AccessTools.Method(typeof(FishingFloatFixedUpdatePatch), nameof(InterceptRaiseSkill));

    private static readonly AccessTools.FieldRef<FishingFloat, float> _lineLengthRef =
        AccessTools.FieldRefAccess<FishingFloat, float>("m_lineLength");

    [HarmonyPrefix]
    private static bool Prefix(FishingFloat __instance)
    {
        Character owner = __instance.GetOwner();
        if (owner == null)
        {
            return true;
        }

        if (owner is Player && InventoryGui.IsVisible())
        {
            Rigidbody floatBody = __instance.GetComponent<Rigidbody>();
            if (floatBody != null)
            {
                floatBody.linearVelocity = Vector3.zero;
                floatBody.angularVelocity = Vector3.zero;
            }
            return false;
        }

        Transform rodTop = __instance.GetRodTop(owner);
        if (rodTop != null)
        {
            float currentDistance = Vector3.Distance(rodTop.position, __instance.transform.position);
            float lineLength = _lineLengthRef(__instance);

            Fish currentFish = __instance.GetCatch();

            if (!__instance.IsInWater())
            {
                if (currentDistance >= __instance.m_maxDistance - 0.25f)
                {
                    Vector3 directionFromRod = (__instance.transform.position - rodTop.position).normalized;
                    __instance.transform.position = rodTop.position + (directionFromRod * (__instance.m_maxDistance - 0.3f));
                    currentDistance = __instance.m_maxDistance - 0.3f;
                    _lineLengthRef(__instance) = currentDistance;

                    Rigidbody body = __instance.GetComponent<Rigidbody>();
                    if (body != null)
                    {
                        body.linearVelocity = new Vector3(0f, Mathf.Min(body.linearVelocity.y, 0f), 0f);
                    }
                }
                else if (currentDistance > lineLength)
                {
                    _lineLengthRef(__instance) = currentDistance;
                }
            }
            else
            {
                if (currentDistance >= __instance.m_maxDistance - 0.25f)
                {
                    Vector3 directionFromRod = (__instance.transform.position - rodTop.position).normalized;
                    __instance.transform.position = rodTop.position + (directionFromRod * (__instance.m_maxDistance - 0.3f));
                    currentDistance = __instance.m_maxDistance - 0.3f;
                    _lineLengthRef(__instance) = currentDistance;
                }
                else if (currentFish == null && !owner.IsBlocking() && (owner.IsAttachedToShip() || owner.GetStandingOnShip() != null) && currentDistance > lineLength)
                {
                    _lineLengthRef(__instance) = currentDistance;
                }
            }

            if (owner.IsBlocking())
            {
                if (currentFish != null && owner.HaveStamina())
                {
                    float fishDistance = Vector3.Distance(currentFish.transform.position, owner.transform.position);
                    if (lineLength <= 1.8f || currentDistance <= 2.2f || fishDistance <= 2.5f)
                    {
                        string msg = FishingFloat.Catch(currentFish, owner);
                        __instance.Message(msg, true);
                        __instance.SetCatch(null);
                        currentFish.OnHooked(null);
                        ZNetView catchNetView = __instance.GetComponent<ZNetView>();
                        if (catchNetView != null && catchNetView.IsValid())
                        {
                            catchNetView.Destroy();
                        }
                        return false;
                    }
                }
                else if (currentFish == null)
                {
                    if (lineLength <= 1.5f || currentDistance <= 1.8f || (!__instance.IsInWater() && currentDistance <= 2.5f))
                    {
                        MethodInfo returnBaitMethod = AccessTools.Method(typeof(FishingFloat), "ReturnBait");
                        if (returnBaitMethod != null)
                        {
                            returnBaitMethod.Invoke(__instance, null);
                        }
                        ZNetView baitNetView = __instance.GetComponent<ZNetView>();
                        if (baitNetView != null && baitNetView.IsValid())
                        {
                            baitNetView.Destroy();
                        }
                        return false;
                    }
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

        bool isStruggling = fish.IsEscaping();
        bool isReeling = owner.IsBlocking() && owner.HaveStamina();

        ItemDrop itemDrop = fish.GetComponent<ItemDrop>();
        int quality = itemDrop != null ? itemDrop.m_itemData.m_quality : 1;

        float skillFactor = owner.GetSkillFactor(Skills.SkillType.Fishing);
        float rodDamping = FishingTensionManager.GetRodTensionDamping(owner);

        float snapMultiplier = 1.0f;
        if (owner is Player player && player.m_helmetItem != null && player.m_helmetItem.m_dropPrefab != null && player.m_helmetItem.m_dropPrefab.name == "HelmetFishingHat")
        {
            snapMultiplier = 1.30f;
        }

        lineState.UpdateTension(Time.fixedDeltaTime, isStruggling, isReeling, quality, skillFactor, rodDamping, snapMultiplier);

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
            else if (instruction.Calls(_raiseSkillMethod))
            {
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Call, _interceptRaiseSkillMethod);
            }
            else
            {
                yield return instruction;
            }
        }
    }

    public static void InterceptRaiseSkill(Character owner, Skills.SkillType skill, float value, FishingFloat floatInstance)
    {
        if (owner == null || floatInstance == null || floatInstance.GetCatch() == null)
        {
            return;
        }

        owner.RaiseSkill(skill, value);
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
