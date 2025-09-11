// -----------------------------------------------------------------------
// <copyright file="FixNW914DuplicationBug.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using HarmonyLib;
    using InventorySystem.Items.Pickups;
    using Scp914.Processors;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches the <see cref="Scp914ItemProcessor.UpgradeInventoryItem"/> delegate.
    /// Fix items upgraded in SCP-914 being duplicated.
    /// Bug reported to NW (https://git.scpslgame.com/northwood-qa/scpsl-bug-reporting/-/issues/1770).
    /// </summary>
    [HarmonyPatch(typeof(Scp914ItemProcessor), nameof(Scp914ItemProcessor.UpgradeInventoryItem))]
    internal class FixNW914DuplicationBug
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = 1;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldloca_S) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // itemPickupBase.DestroySelf();
                new CodeInstruction(OpCodes.Ldloc_S, 5),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(ItemPickupBase), nameof(ItemPickupBase.DestroySelf))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}