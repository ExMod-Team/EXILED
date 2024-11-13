// -----------------------------------------------------------------------
// <copyright file="Jailbird914CoarseFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
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
    /// Patches <see cref="JailbirdDeteriorationTracker.Setup(JailbirdItem, JailbirdHitreg)" />.
    /// Bug reported to NW (https://trello.com/c/kyr3hV9B).
    /// </summary>
    [HarmonyPatch(typeof(JailbirdDeteriorationTracker), nameof(JailbirdDeteriorationTracker.Setup))]
    internal static class Jailbird914CoarseFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            var offset = -1;
            var index = newInstructions.FindIndex(i => i.opcode == OpCodes.Blt_S) + offset;

            var labels = newInstructions[index].ExtractLabels();

            var offsetToRemove = 2;
            var indexToRemove = newInstructions.FindIndex(i => i.opcode == OpCodes.Blt_S) + offsetToRemove;
            var countToRemove = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Blt_S) - indexToRemove + offsetToRemove;

            newInstructions.RemoveRange(indexToRemove, countToRemove);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // JailbirdDeteriorationTracker.Scp914CoarseCharges = JailbirdWearState.AlmostBroken
                    new CodeInstruction(OpCodes.Ldc_I4_4).WithLabels(labels),
                    new(OpCodes.Call, PropertySetter(typeof(JailbirdDeteriorationTracker), nameof(JailbirdDeteriorationTracker.Scp914CoarseCharges))),
                });

            for (var z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}