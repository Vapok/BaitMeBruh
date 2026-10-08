using HarmonyLib;
using BaitMeBruh.Managers;

namespace BaitMeBruh.Patches;

[HarmonyPatch(typeof(Tutorial), "Awake")]
internal static class TutorialAwakePatch
{
    [HarmonyPostfix]
    private static void Postfix(Tutorial __instance)
    {
        HuginTutorialManager.RegisterTutorials(__instance);
    }
}
