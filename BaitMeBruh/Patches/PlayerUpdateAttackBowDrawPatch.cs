using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace BaitMeBruh.Patches;

[HarmonyPatch(typeof(Player), "UpdateAttackBowDraw")]
internal static class PlayerUpdateAttackBowDrawPatch
{
    private static readonly MethodInfo _isAttachedPlayerMethod =
        AccessTools.Method(typeof(Player), nameof(Player.IsAttached));

    private static readonly MethodInfo _isAttachedCharacterMethod =
        AccessTools.Method(typeof(Character), nameof(Character.IsAttached));

    private static readonly MethodInfo _canBowDrawWhenAttachedMethod =
        AccessTools.Method(typeof(PlayerUpdateAttackBowDrawPatch), nameof(CanBowDrawWhenAttached));

    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (CodeInstruction instruction in instructions)
        {
            if (instruction.Calls(_isAttachedPlayerMethod) || instruction.Calls(_isAttachedCharacterMethod))
            {
                yield return new CodeInstruction(OpCodes.Ldarg_1);
                yield return new CodeInstruction(OpCodes.Call, _canBowDrawWhenAttachedMethod);
            }
            else
            {
                yield return instruction;
            }
        }
    }

    public static bool CanBowDrawWhenAttached(Player player, ItemDrop.ItemData weapon)
    {
        if (player == null)
        {
            return false;
        }

        if (player.InBed() || player.GetDoodadController() != null)
        {
            return player.IsAttached();
        }

        if (weapon != null && weapon.m_dropPrefab != null && weapon.m_dropPrefab.name.StartsWith("FishingRod"))
        {
            return false;
        }

        return player.IsAttached();
    }
}
