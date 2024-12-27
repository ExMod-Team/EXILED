// -----------------------------------------------------------------------
// <copyright file="FixNWDisarmCommand.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Achievements.Handlers;

    using API.Features.Pools;
    using HarmonyLib;

    /// <summary>
    /// Patches the <see cref="JustResourcesHandler.HandleDisarmed(ReferenceHub, ReferenceHub)"/> delegate.
    /// Fix https://git.scpslgame.com/northwood-qa/scpsl-bug-reporting/-/issues/528.
    /// </summary>
    [HarmonyPatch(typeof(JustResourcesHandler), nameof(JustResourcesHandler.HandleDisarmed), new Type[] { typeof(ReferenceHub), typeof(ReferenceHub), })]
    internal class FixNWDisarmCommand
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_1),
                new(OpCodes.Brfalse_S, retLabel),
                new(OpCodes.Ldarg_2),
                new(OpCodes.Brfalse_S, retLabel),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
