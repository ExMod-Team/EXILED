// -----------------------------------------------------------------------
// <copyright file="FixGeneratorActivatedObjective.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Footprinting;
    using HarmonyLib;
    using InventorySystem;
    using InventorySystem.Items.Firearms.Ammo;
    using InventorySystem.Items.Pickups;
    using Respawning.Objectives;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="GeneratorActivatedObjective.OnGeneratorEngaged"/> delegate.
    /// Fix than LabAPI get an incorect value for when Scp079 is not prevent and than any change on timer for LabAPI will not be affected.
    /// Reported at NW (https://git.scpslgame.com/northwood-qa/scpsl-bug-reporting/-/issues/2184).
    /// </summary>
    [HarmonyPatch(typeof(GeneratorActivatedObjective), nameof(GeneratorActivatedObjective.OnGeneratorEngaged))]
    internal class FixGeneratorActivatedObjective
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            Label jump = generator.DefineLabel();
            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_2) + offset;

            newInstructions[index].labels.Add(jump);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldloc_0),
                new(OpCodes.Brtrue_S, jump),
                new(OpCodes.Ldc_R4, 0f),
                new(OpCodes.Stloc_2),
            });

            offset = 0;
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldloc_0) + offset;
            newInstructions.RemoveRange(index, 2);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
