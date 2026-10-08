using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace BaitMeBruh.Patches;

[HarmonyPatch(typeof(Player), nameof(Player.SetControls))]
internal static class PlayerSetControlsPatch
{
    private static readonly MethodInfo _inEmoteMethod =
        AccessTools.Method(typeof(Character), nameof(Character.InEmote));

    private static readonly MethodInfo _shouldDetachMethod =
        AccessTools.Method(typeof(PlayerSetControlsPatch), nameof(ShouldDetachOnInput));

    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> list = new List<CodeInstruction>(instructions);
        int inEmoteIndex = -1;

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Calls(_inEmoteMethod))
            {
                inEmoteIndex = i;
                break;
            }
        }

        if (inEmoteIndex == -1 || inEmoteIndex + 2 >= list.Count)
        {
            return list;
        }

        int exprStart = inEmoteIndex + 2;
        int exprEnd = -1;

        for (int i = exprStart; i < list.Count; i++)
        {
            if (list[i].opcode == OpCodes.Brfalse || list[i].opcode == OpCodes.Brfalse_S)
            {
                exprEnd = i;
                break;
            }
        }

        if (exprEnd == -1)
        {
            return list;
        }

        List<CodeInstruction> replacement = new List<CodeInstruction>
        {
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldarg_1),
            new CodeInstruction(OpCodes.Ldarg_2),
            new CodeInstruction(OpCodes.Ldarg_S, (byte)4),
            new CodeInstruction(OpCodes.Ldarg_S, (byte)6),
            new CodeInstruction(OpCodes.Ldarg_S, (byte)7),
            new CodeInstruction(OpCodes.Ldarg_S, (byte)8),
            new CodeInstruction(OpCodes.Ldarg_S, (byte)9),
            new CodeInstruction(OpCodes.Call, _shouldDetachMethod)
        };

        for (int i = exprStart; i < exprEnd; i++)
        {
            if (list[i].labels != null && list[i].labels.Count > 0)
            {
                replacement[0].labels.AddRange(list[i].labels);
            }
        }

        list.RemoveRange(exprStart, exprEnd - exprStart);
        list.InsertRange(exprStart, replacement);

        return list;
    }

    public static bool ShouldDetachOnInput(Player player, Vector3 movedir, bool attack, bool secondaryAttack, bool block, bool blockHold, bool jump, bool crouch)
    {
        if ((player.IsAttached() || player.InEmote()) && !player.InBed() && player.GetDoodadController() == null)
        {
            ItemDrop.ItemData currentWeapon = player.GetCurrentWeapon();
            if (currentWeapon != null && currentWeapon.m_dropPrefab != null && currentWeapon.m_dropPrefab.name.StartsWith("FishingRod"))
            {
                return (movedir != Vector3.zero) || jump || crouch;
            }
        }

        return (movedir != Vector3.zero) || attack || secondaryAttack || block || blockHold || jump || crouch;
    }
}
