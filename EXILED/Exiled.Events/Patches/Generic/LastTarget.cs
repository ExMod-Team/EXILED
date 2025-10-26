// -----------------------------------------------------------------------
// <copyright file="LastTarget.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.HumanTracker;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="LastHumanTracker.TryGetLastTarget"/> so <see cref="Round.IgnoredPlayers"/> does not count to it.
    /// </summary>
    [HarmonyPatch(typeof(LastHumanTracker), nameof(LastHumanTracker.TryGetLastTarget))]
    public class LastTarget
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = (Label)newInstructions.First(instruction => instruction.opcode == OpCodes.Br_S).operand;

            // can break between versions!
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_3) + 1;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Round.IgnoredPlayers.Contains(allHub)
                new(OpCodes.Call, PropertyGetter(typeof(Round), nameof(Round.IgnoredPlayers))),
                new(OpCodes.Ldloc_3),
                new(OpCodes.Callvirt, Method(typeof(HashSet<ReferenceHub>), nameof(HashSet<ReferenceHub>.Contains))),

                new(OpCodes.Brtrue_S, continueLabel),
            });

            for (int z = 0; z < newInstructions.Count; ++z)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}