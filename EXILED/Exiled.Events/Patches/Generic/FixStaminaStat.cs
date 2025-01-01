// -----------------------------------------------------------------------
// <copyright file="FixStaminaStat.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.CustomStats;
    using Exiled.API.Features.Pools;
    using HarmonyLib;
    using PlayerRoles.FirstPersonControl;
    using PlayerStatsSystem;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Fix for <see cref="CustomStaminaStat"/>.
    /// </summary>
    [HarmonyPatch(typeof(FpcStateProcessor), nameof(FpcStateProcessor.UpdateMovementState))]
    internal static class FixStaminaStat
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ldc_R4);
            newInstructions.RemoveAt(index);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(FpcStateProcessor), nameof(FpcStateProcessor._stat))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(StaminaStat), nameof(StaminaStat.MaxValue))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}