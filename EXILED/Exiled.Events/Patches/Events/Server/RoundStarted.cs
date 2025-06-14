// -----------------------------------------------------------------------
// <copyright file="RoundStarted.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.EventArgs.Server;
    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="RoundSummary.Start"/> to invoke the <see cref="Handlers.Server.RoundStarted"/> event.
    /// </summary>
    [HarmonyPatch(typeof(RoundSummary), nameof(RoundSummary.Start))]
    internal static class RoundStarted
    {
        private static readonly System.Func<Player, bool> NonNpcNonHostFilter = p => !p.IsNPC && !p.IsHost;

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            // Local variables for event args
            LocalBuilder ev = generator.DeclareLocal(typeof(RoundStartedEventArgs));
            LocalBuilder keepRoundOnOne = generator.DeclareLocal(typeof(bool));

            // Find index before ceq (IL_0033) to capture KeepRoundOnOne
            const int offset = -2;
            int index = newInstructions.FindIndex(instruction =>
                instruction.opcode == OpCodes.Ceq &&
                (newInstructions[newInstructions.IndexOf(instruction) + 1].opcode == OpCodes.Stfld &&
                 (newInstructions[newInstructions.IndexOf(instruction) + 1].operand is FieldInfo field && field.Name == "KeepRoundOnOne")));

            // Insert instructions to create and invoke RoundStarted event
            newInstructions.InsertRange(
                index + offset,
                new[]
                {
                    // Store KeepRoundOnOne from stack (result of !GetBool)
                    new CodeInstruction(OpCodes.Stloc_S, keepRoundOnOne),

                    // DateTime.UtcNow
                    new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(System.DateTime), nameof(System.DateTime.UtcNow))),

                    // Player.List.Where(p => !p.IsNPC && !p.IsHost).Count()
                    new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Player), nameof(Player.List))),
                    new CodeInstruction(OpCodes.Ldsfld, Field(typeof(RoundStarted), nameof(NonNpcNonHostFilter))),
                    new CodeInstruction(OpCodes.Callvirt, Method(typeof(System.Linq.Enumerable), nameof(System.Linq.Enumerable.Where), new[] { typeof(Player) })),
                    new CodeInstruction(OpCodes.Call, Method(typeof(System.Linq.Enumerable), nameof(System.Linq.Enumerable.Count), new[] { typeof(Player) })),

                    // Load KeepRoundOnOne from local
                    new CodeInstruction(OpCodes.Ldloc_S, keepRoundOnOne),

                    // new RoundStartedEventArgs(DateTime.UtcNow, playerCount, keepRoundOnOne)
                    new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(RoundStartedEventArgs))[0]),
                    new CodeInstruction(OpCodes.Stloc_S, ev),

                    // Handlers.Server.OnRoundStarted(ev)
                    new CodeInstruction(OpCodes.Ldloc_S, ev),
                    new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnRoundStarted))),

                    // Reload KeepRoundOnOne for original stfld
                    new CodeInstruction(OpCodes.Ldloc_S, keepRoundOnOne),
                });

            // Yield all instructions
            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}