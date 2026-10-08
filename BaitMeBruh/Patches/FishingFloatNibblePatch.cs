using HarmonyLib;
using UnityEngine;
using BaitMeBruh.Managers;

namespace BaitMeBruh.Patches;

[HarmonyPatch(typeof(FishingFloat), nameof(FishingFloat.RPC_Nibble))]
internal static class FishingFloatNibblePatch
{
    private static readonly AccessTools.FieldRef<FishingFloat, float> _nibbleTimeRef =
        AccessTools.FieldRefAccess<FishingFloat, float>("m_nibbleTime");

    [HarmonyPostfix]
    private static void Postfix(FishingFloat __instance, ZDOID fishID, bool correctBait)
    {
        if (!correctBait || __instance == null || __instance.GetCatch() != null)
        {
            return;
        }

        float nibbleTime = _nibbleTimeRef(__instance);
        if (Mathf.Abs(Time.time - nibbleTime) > 0.05f)
        {
            return;
        }

        Character owner = __instance.GetOwner();
        if (owner == null)
        {
            return;
        }

        FishingCueManager.TriggerBiteCue(__instance, owner);
    }
}
