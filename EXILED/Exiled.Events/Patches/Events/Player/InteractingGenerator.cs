// -----------------------------------------------------------------------
// <copyright file="InteractingGenerator.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using Handlers;

    using HarmonyLib;

    using MapGeneration.Distributors;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp079Generator.ServerInteract(ReferenceHub, byte)"/>.
    /// Adds the <see cref="Player.ActivatingGenerator"/>, <see cref="Player.ClosingGenerator"/>, <see cref="Player.OpeningGenerator"/>, <see cref="Player.UnlockingGenerator"/> and <see cref="Player.StoppingGenerator"/> events.
    /// </summary>
    [EventPatch(typeof(Player), nameof(Player.ActivatingGenerator))]
    [EventPatch(typeof(Player), nameof(Player.ClosingGenerator))]
    [EventPatch(typeof(Player), nameof(Player.OpeningGenerator))]
    [EventPatch(typeof(Player), nameof(Player.UnlockingGenerator))]
    [EventPatch(typeof(Player), nameof(Player.StoppingGenerator))]
    [HarmonyPatch(typeof(Scp079Generator), nameof(Scp079Generator.ServerInteract))]
    internal static class InteractingGenerator
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder player = generator.DeclareLocal(typeof(API.Features.Player));

            int offset = -1;
            int index = newInstructions.FindLastIndex(i => i.LoadsField(Field(typeof(Scp079Generator), nameof(Scp079Generator._cooldownStopwatch)))) + offset;

            Label breakLabel = newInstructions[index].labels.First();

            offset = 1;
            index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(Stopwatch), nameof(Stopwatch.Stop)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player player = Player.Get(ply)
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Stloc_S, player.LocalIndex),
                });

            // closing generator index
            offset = -2;
            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Newobj && (ConstructorInfo)i.operand == GetDeclaredConstructors(typeof(LabApi.Events.Arguments.PlayerEvents.PlayerClosingGeneratorEventArgs))[0]) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // player
                    new(OpCodes.Ldloc_S, player.LocalIndex),

                    // Scp079Generator
                    new(OpCodes.Ldarg_0),

                    // ClosingGeneratorEventArgs ev = new(Player, Scp079Generator)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ClosingGeneratorEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Player.OnClosingGenerator(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnClosingGenerator))),

                    // if (!ev.IsAllowed) goto break;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ClosingGeneratorEventArgs), nameof(ClosingGeneratorEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, breakLabel),
                });

            // opening generator index
            offset = -2;
            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Newobj && (ConstructorInfo)i.operand == GetDeclaredConstructors(typeof(LabApi.Events.Arguments.PlayerEvents.PlayerOpeningGeneratorEventArgs))[0]) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // player
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex).MoveLabelsFrom(newInstructions[index]),

                    // Scp079Generator
                    new(OpCodes.Ldarg_0),

                    // OpeningGeneratorEventArgs ev = new(Player, Scp079Generator)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(OpeningGeneratorEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Player.OnOpeningGenerator(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnOpeningGenerator))),

                    // if (!ev.IsAllowed) goto break;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(OpeningGeneratorEventArgs), nameof(OpeningGeneratorEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, breakLabel),
                });

            // unlocking generator index
            offset = -2;
            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Newobj && (ConstructorInfo)i.operand == GetDeclaredConstructors(typeof(LabApi.Events.Arguments.PlayerEvents.PlayerUnlockingGeneratorEventArgs))[0]) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // player
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),

                    // Scp079Generator
                    new(OpCodes.Ldarg_0),

                    // flag
                    new(OpCodes.Ldloc_2),

                    // UnlockingGeneratorEventArgs ev = new(Player, Scp079Generator, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UnlockingGeneratorEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Player.OnUnlockingGenerator(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnUnlockingGenerator))),

                    // flag = ev.IsAllowed
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UnlockingGeneratorEventArgs), nameof(UnlockingGeneratorEventArgs.IsAllowed))),
                    new(OpCodes.Stloc_2),
                });

            // activating generator index
            offset = -2;
            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Newobj && (ConstructorInfo)i.operand == GetDeclaredConstructors(typeof(LabApi.Events.Arguments.PlayerEvents.PlayerActivatingGeneratorEventArgs))[0]) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // player
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),

                    // Scp079Generator
                    new(OpCodes.Ldarg_0),

                    // ActivatingGeneratorEventArgs ev = new(Player, Scp079Generator)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ActivatingGeneratorEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Player.OnActivatingGenerator(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnActivatingGenerator))),

                    // if (!ev.IsAllowed) goto break;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ActivatingGeneratorEventArgs), nameof(ActivatingGeneratorEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, breakLabel),
                });

            // deactivating generator instructions
            CodeInstruction[] deactivatingGeneratorEvent =
            {
                // Scp079Generator
                new(OpCodes.Ldarg_0),

                // StoppingGeneratorEventArgs ev = new(Player, Scp079Generator)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StoppingGeneratorEventArgs))[0]),
                new(OpCodes.Dup),

                // Player.OnStoppingGenerator(ev)
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnStoppingGenerator))),

                // if (!ev.IsAllowed) goto break;
                new(OpCodes.Callvirt, PropertyGetter(typeof(StoppingGeneratorEventArgs), nameof(StoppingGeneratorEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, breakLabel),
            };

            // deactivating generator index
            offset = -2;
            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Newobj && (ConstructorInfo)i.operand == GetDeclaredConstructors(typeof(LabApi.Events.Arguments.PlayerEvents.PlayerDeactivatingGeneratorEventArgs))[0]) + offset;
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex).MoveLabelsFrom(newInstructions[index]),
            }.AddRangeToArray(deactivatingGeneratorEvent));

            // second deactivating generator index
            index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Newobj && (ConstructorInfo)i.operand == GetDeclaredConstructors(typeof(LabApi.Events.Arguments.PlayerEvents.PlayerDeactivatingGeneratorEventArgs))[0]) + offset;
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),
            }.AddRangeToArray(deactivatingGeneratorEvent));

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}