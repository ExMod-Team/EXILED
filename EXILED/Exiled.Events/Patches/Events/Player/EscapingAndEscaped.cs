// -----------------------------------------------------------------------
// <copyright file="EscapingAndEscaped.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable IDE0060

    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using EventArgs.Player;
    using Exiled.API.Features.Roles;
    using Exiled.Events.Attributes;
    using HarmonyLib;
    using LabApi.Events.Arguments.PlayerEvents;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Escape.ServerHandlePlayer(ReferenceHub)"/> for <see cref="Handlers.Player.Escaping" /> and <see cref="Handlers.Player.Escaped"/>.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Escaping))]
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Escaped))]
    [HarmonyPatch(typeof(Escape), nameof(Escape.ServerHandlePlayer))]
    internal static class EscapingAndEscaped
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(EscapingEventArgs));
            LocalBuilder role = generator.DeclareLocal(typeof(Role));

            int offset = 2;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Callvirt && i.operand == (object)PropertyGetter(typeof(PlayerEscapingEventArgs), nameof(PlayerEscapingEventArgs.EscapeScenario))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // hub
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),

                    // roleTypeId
                    new(OpCodes.Ldloc_1),

                    // escapeScenario
                    new(OpCodes.Ldloc_2),

                    // EscapingEventArgs ev = new(Player, RoleTypeId, EscapeScenario)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(EscapingEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),

                    // Handlers.Player.OnEscaping(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnEscaping))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(EscapingEventArgs), nameof(EscapingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),

                    // roleTypeId = ev.NewRole
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(EscapingEventArgs), nameof(EscapingEventArgs.NewRole))),
                    new(OpCodes.Stloc_1),

                    // escapeScenario = ev.EscapeScenario
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(EscapingEventArgs), nameof(EscapingEventArgs.EscapeScenario))),
                    new(OpCodes.Stloc_2),
                });

            offset = 4;
            index = newInstructions.FindIndex(x => x.Is(OpCodes.Stfld, Field(typeof(Escape.EscapeMessage), nameof(Escape.EscapeMessage.EscapeTime)))) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // role = ev.Player.Role
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(EscapingEventArgs), nameof(EscapingEventArgs.Player))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Role))),
                new(OpCodes.Stloc_S, role.LocalIndex),
            });

            newInstructions.InsertRange(newInstructions.Count - 1, new CodeInstruction[]
            {
                // ev.Player
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(EscapingEventArgs), nameof(EscapingEventArgs.Player))),

                // escapeScenario
                new(OpCodes.Ldloc_2),

                // role
                new(OpCodes.Ldloc_S, role.LocalIndex),

                // EscapedEventArgs ev2 = new(ev.Player, ev.EscapeScenario, role);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(EscapedEventArgs))[0]),

                // Handlers.Player.OnEscaped(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnEscaped))),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}