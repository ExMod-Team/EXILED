// -----------------------------------------------------------------------
// <copyright file="Scp127TryGetCurPairFix.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using HarmonyLib;
    using InventorySystem.Items.Firearms.Modules.Scp127;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp127Hitscan.TryGetCurPair"/> to work and return false when the module has no owner.
    /// </summary>
    [HarmonyPatch(typeof(Scp127Hitscan), nameof(Scp127Hitscan.TryGetCurPair))]
    public class Scp127TryGetCurPairFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();

            newInstructions.FindLast(x => x.opcode == OpCodes.Ldarg_1).labels.Add(retLabel);

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // check if owner is null, if so, jump to last 2 lines of method, else, run the method.
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp127Hitscan), nameof(Scp127Hitscan.Owner))),
                new(OpCodes.Call, Method(typeof(Object), "op_Implicit")),
                new(OpCodes.Brfalse_S, retLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}