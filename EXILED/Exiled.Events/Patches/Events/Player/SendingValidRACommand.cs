// -----------------------------------------------------------------------
// <copyright file="SendingValidRACommand.cs" company="ExMod Team">
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

    using RemoteAdmin;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="CommandProcessor.ProcessQuery(string, CommandSender)" />.
    /// Adds the <see cref="Handlers.Player.SendingValidCommand" /> and
    /// the <see cref="Handlers.Player.SentValidCommand" /> events.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.SendingValidCommand))]
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.SentValidCommand))]
    [HarmonyPatch(typeof(CommandProcessor), nameof(CommandProcessor.ProcessQuery))]
    internal static class SendingValidRACommand
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label setptroperresp = generator.DefineLabel();

            Label ret = generator.DefineLabel();
            newInstructions[newInstructions.Count - 1].WithLabels(ret);
            LocalBuilder ev = generator.DeclareLocal(typeof(SendingValidCommandEventArgs));
            int offset = 2;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(CommandHandler), nameof(CommandHandler.TryGetCommand)))) + offset;

            Label contlabel = generator.DefineLabel();
            newInstructions[index].WithLabels(contlabel);

            int sendreplyidx = newInstructions.FindIndex(instructions => instructions.Calls(Method(typeof(string), nameof(string.IsNullOrEmpty)))) + offset;

            Label sendreply = generator.DefineLabel();
            newInstructions[sendreplyidx].WithLabels(sendreply);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                   // sender
                   new (OpCodes.Ldarg_1),

                   // Player.get(sender)
                   new (OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new Type[] { typeof(CommandSender) })),

                   // command
                   new (OpCodes.Ldloc_1),

                   // commandtype
                   new (OpCodes.Ldc_I4_4),

                   // query
                   new (OpCodes.Ldarg_0),

                   // response
                   new (OpCodes.Ldloc_S, 6),

                   // new SendingValidCommandEventArgs
                   new (OpCodes.Newobj, GetDeclaredConstructors(typeof(SendingValidCommandEventArgs))[0]),
                   new (OpCodes.Dup),
                   new (OpCodes.Stloc_S, ev.LocalIndex),

                   // OnSendingValidCommad(ev)
                   new (OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnSendingValidCommand))),

                   // if ev.IsAllowed cont
                   new (OpCodes.Ldloc_S, ev.LocalIndex),
                   new (OpCodes.Callvirt, PropertyGetter(typeof(SendingValidCommandEventArgs), nameof(SendingValidCommandEventArgs.IsAllowed))),
                   new (OpCodes.Brtrue_S, contlabel),

                   // if ev.Response.IsNullOrEmpty rets
                   new (OpCodes.Ldloc_S, ev.LocalIndex),
                   new (OpCodes.Callvirt, PropertyGetter(typeof (SendingValidCommandEventArgs), nameof(SendingValidCommandEventArgs.Response))),
                   new (OpCodes.Call, Method(typeof(string), nameof(string.IsNullOrEmpty))),
                   new (OpCodes.Brtrue_S, setptroperresp),

                   // response = ev.Response
                   new (OpCodes.Ldloc_S, ev.LocalIndex),
                   new (OpCodes.Callvirt, PropertyGetter(typeof (SendingValidCommandEventArgs), nameof(SendingValidCommandEventArgs.Response))),
                   new (OpCodes.Stloc_S, 6),

                   // goto sendreply
                   new (OpCodes.Br, sendreply),

                   // response = "The Command Execution Was Prevented By Plugin."
                   new CodeInstruction(OpCodes.Ldstr, "The Command Execution Was Prevented By Plugin.").WithLabels(setptroperresp),
                   new (OpCodes.Stloc_S, 6),
                   new (OpCodes.Br, sendreply),
                });
            offset = -4;
            index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(string), nameof(string.ToUpperInvariant)))) + offset;
            Label skip = generator.DefineLabel();
            newInstructions[index].WithLabels(skip);
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                   // if ev.Response.IsNullOrEmpty skip
                   new (OpCodes.Ldloc_S, ev.LocalIndex),
                   new (OpCodes.Callvirt, PropertyGetter(typeof (SendingValidCommandEventArgs), nameof(SendingValidCommandEventArgs.Response))),
                   new (OpCodes.Call, Method(typeof(string), nameof(string.IsNullOrEmpty))),
                   new (OpCodes.Brtrue_S, skip),

                   // response = ev.Response
                   new (OpCodes.Ldloc_S, ev.LocalIndex),
                   new (OpCodes.Callvirt, PropertyGetter(typeof (SendingValidCommandEventArgs), nameof(SendingValidCommandEventArgs.Response))),
                   new (OpCodes.Stloc_S, 6),
                });

            offset = 0;
            index = newInstructions.FindIndex(instrction => instrction.Calls(Method(typeof(CommandSender), nameof(CommandSender.RaReply)))) + offset;
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // sender
                    new (OpCodes.Ldarg_1),

                    // Player.get(sender)
                    new (OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new Type[] { typeof(CommandSender) })),

                    // command
                    new (OpCodes.Ldloc_1),

                    // commandtype
                    new (OpCodes.Ldc_I4_4),

                    // query
                    new (OpCodes.Ldarg_0),

                    // response
                    new (OpCodes.Ldloc_S, 6),

                    // result
                    new (OpCodes.Ldloc_S, 5),

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
