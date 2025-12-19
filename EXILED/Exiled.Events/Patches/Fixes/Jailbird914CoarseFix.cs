// -----------------------------------------------------------------------
// <copyright file="Jailbird914CoarseFix.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
#pragma warning disable IDE0060

    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;

    using HarmonyLib;
    using InventorySystem.Items.Jailbird;
    using Mirror;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="JailbirdDeteriorationTracker.Setup(JailbirdItem)" />.
    /// Bug reported to NW (https://git.scpslgame.com/northwood-qa/scpsl-bug-reporting/-/issues/88).
    /// </summary>
    [HarmonyPatch(typeof(JailbirdDeteriorationTracker), nameof(JailbirdDeteriorationTracker.Setup))]
    internal static class Jailbird914CoarseFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = -1;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Blt_S) + offset;

            List<Label> labels = newInstructions[index].ExtractLabels();

            int offsetToRemove = 2;
            int indexToRemove = newInstructions.FindIndex(i => i.opcode == OpCodes.Blt_S) + offsetToRemove;
            int countToRemove = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Blt_S) - indexToRemove + offsetToRemove;

            newInstructions.RemoveRange(indexToRemove, countToRemove);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // JailbirdDeteriorationTracker.Scp914CoarseCharges = JailbirdWearState.AlmostBroken
                    new CodeInstruction(OpCodes.Ldc_I4_4).WithLabels(labels),
                    new(OpCodes.Call, PropertySetter(typeof(JailbirdDeteriorationTracker), nameof(JailbirdDeteriorationTracker.Scp914CoarseCharges))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}