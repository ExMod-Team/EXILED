// -----------------------------------------------------------------------
// <copyright file="SendingValidGameConsoleCommand.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using CommandSystem;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;
    using LabApi.Features.Enums;
    using RemoteAdmin;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="QueryProcessor.ProcessGameConsoleQuery(string)" />.
    /// Adds the <see cref="Handlers.Player.SendingValidCommand" /> and
    /// the <see cref="Handlers.Player.SentValidCommand" /> events.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.SendingValidCommand))]
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.SentValidCommand))]
    [HarmonyPatch(typeof(QueryProcessor), nameof(QueryProcessor.ProcessGameConsoleQuery))]
    internal static class SendingValidGameConsoleCommand
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label ret = generator.DefineLabel();
            newInstructions[newInstructions.Count - 1].WithLabels(ret);

            Label setproperresp = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(SendingValidCommandEventArgs));

            int offset = 2;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(CommandHandler), nameof(CommandHandler.TryGetCommand)))) + offset;
            Label contlabel = generator.DefineLabel();

            offset = -6;
            int sendreplyidx = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldstr && (string)instruction.operand == "magenta") + offset;
            Label sendreply = generator.DefineLabel();
            newInstructions[sendreplyidx].WithLabels(sendreply);

            newInstructions[index].WithLabels(contlabel);

            newInstructions.InsertRange(
                index,
                new[]
                {
                   // Player.Get(Hub)
                   new(OpCodes.Ldarg_0),
                   new(OpCodes.Ldfld, Field(typeof(QueryProcessor), nameof(QueryProcessor._hub))),
                   new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                   // command
                   new (OpCodes.Ldloc_S, 4),

                   // CommandType.Client
                   new(OpCodes.Ldc_I4_S, (sbyte)CommandType.Client),

                   // query
                   new(OpCodes.Ldarg_1),

                   // response
                   new(OpCodes.Ldloc_S, 6),

                   // new SendingValidCommandEventArgs
                   new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SendingValidCommandEventArgs))[0]),
                   new(OpCodes.Dup),
                   new(OpCodes.Stloc_S, ev.LocalIndex),

                    // OnSendingValidCommad(ev)
                   new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnSendingValidCommand))),

                    // if ev.IsAllowed cont
                   new(OpCodes.Ldloc_S, ev.LocalIndex),
                   new(OpCodes.Callvirt, PropertyGetter(typeof(SendingValidCommandEventArgs), nameof(SendingValidCommandEventArgs.IsAllowed))),
                   new(OpCodes.Brtrue_S, contlabel),

                   // if ev.Response.IsNullOrEmpty rets
                   new(OpCodes.Ldloc_S, ev.LocalIndex),
                   new(OpCodes.Callvirt, PropertyGetter(typeof (SendingValidCommandEventArgs), nameof(SendingValidCommandEventArgs.Response))),
                   new(OpCodes.Call, Method(typeof(string), nameof(string.IsNullOrEmpty))),
                   new(OpCodes.Brtrue_S, setproperresp),

                   // response = ev.Response
                   new(OpCodes.Ldloc_S, ev.LocalIndex),
                   new(OpCodes.Callvirt, PropertyGetter(typeof (SendingValidCommandEventArgs), nameof(SendingValidCommandEventArgs.Response))),
                   new(OpCodes.Stloc_S, 6),

                   // goto sendreply
                   new(OpCodes.Br, sendreply),

                   // response = "The Command Execution Was Prevented By Plugin."
                   new CodeInstruction(OpCodes.Ldstr, "The Command Execution Was Prevented By Plugin.").WithLabels(setproperresp),
                   new(OpCodes.Stloc_S, 6),
                   new(OpCodes.Br, sendreply),
                });

            offset = 2;
            index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(Misc), nameof(Misc.CloseAllRichTextTags)))) + offset;
            Label skip = generator.DefineLabel();
            newInstructions[index].WithLabels(skip);
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                   // if ev.Response.IsNullOrEmpty skip
                   new(OpCodes.Ldloc_S, ev.LocalIndex),
                   new(OpCodes.Callvirt, PropertyGetter(typeof (SendingValidCommandEventArgs), nameof(SendingValidCommandEventArgs.Response))),
                   new(OpCodes.Call, Method(typeof(string), nameof(string.IsNullOrEmpty))),
                   new(OpCodes.Brtrue_S, skip),

                   // response = ev.Response
                   new(OpCodes.Ldloc_S, ev.LocalIndex),
                   new(OpCodes.Callvirt, PropertyGetter(typeof (SendingValidCommandEventArgs), nameof(SendingValidCommandEventArgs.Response))),
                   new(OpCodes.Stloc_S, 6),
                });

            offset = 1;
            index = newInstructions.FindLastIndex(i => i.Calls(Method(typeof(GameConsoleTransmission), nameof(GameConsoleTransmission.SendToClient)))) + offset;
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(sender)
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(CommandSender) })),

                    // command
                    new(OpCodes.Ldloc_S, 4),

                    // CommandType.Client
                    new (OpCodes.Ldc_I4_S, (sbyte)CommandType.Client),

                    // query
                    new(OpCodes.Ldarg_1),

                    // response
                    new(OpCodes.Ldloc_S, 6),

                    // result
                    new (OpCodes.Ldloc_S, 7),

                    // new SentValidCommandEventArgs
                    new (OpCodes.Newobj, GetDeclaredConstructors(typeof(SentValidCommandEventArgs))[0]),

                    // OnSentValidCommand(ev)
                    new (OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnSentValidCommand))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
