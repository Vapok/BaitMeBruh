using HarmonyLib;
using UnityEngine;

namespace BaitMeBruh.Patches;

[HarmonyPatch(typeof(Fish), nameof(Fish.CustomFixedUpdate))]
internal static class FishCustomFixedUpdatePatch
{
    [HarmonyPrefix]
    private static bool Prefix(Fish __instance)
    {
        if (InventoryGui.IsVisible())
        {
            FishingFloat ff = FishingFloat.FindFloat(__instance);
            if (ff != null)
            {
                Character owner = ff.GetOwner();
                if (owner != null && owner is Player)
                {
                    Rigidbody body = __instance.GetComponent<Rigidbody>();
                    if (body != null)
                    {
                        body.linearVelocity = Vector3.zero;
                        body.angularVelocity = Vector3.zero;
                    }

                    return false;
                }
            }
        }

        return true;
    }
}
