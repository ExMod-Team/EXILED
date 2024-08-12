// -----------------------------------------------------------------------
// <copyright file="ExecuteCommand.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
#pragma warning disable SA1402
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Server;
    using Exiled.Events.Patches.Generic;
    using HarmonyLib;
    using RemoteAdmin;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="CommandProcessor.ProcessQuery"/> and <see cref="QueryProcessor.ProcessGameConsoleQuery"/>
    /// to add <see cref="Handlers.Server.ExecutingCommand"/> and <see cref="Handlers.Server.ExecutedCommand"/> events.
    /// </summary>
    [HarmonyPatch(typeof(CommandProcessor), nameof(CommandProcessor.ProcessQuery))]
    // [EventPatch(typeof(Handlers.Server), nameof(Handlers.Server.ExecutingCommand))]
    // [EventPatch(typeof(Handlers.Server), nameof(Handlers.Server.ExecutedCommand))]
    internal class ExecuteCommand
    {
        /// <summary>
        /// Logs a command to the RA log file.
        /// </summary>
        /// <param name="query">The command being logged.</param>
        /// <param name="sender">The sender of the command.</param>
        public static void LogCommand(string query, CommandSender sender)
        {
            try
            {
                if (query.StartsWith("$", StringComparison.Ordinal))
                    return;

                Player player = sender is PlayerCommandSender playerCommandSender && sender != Server.Host.Sender
                    ? Player.Get(playerCommandSender)
                    : Server.Host;

                string logMessage = string.Empty;

                try
                {
                    logMessage =
                        $"[{DateTime.Now}] {(player == Server.Host ? "Server Console" : $"{player?.Nickname} ({player?.UserId}) {player?.IPAddress}")}" +
                        $" has run the command {query}.\n";
                }
                catch (Exception exception)
                {
                    Log.Error($"{nameof(CommandLogging)}: Failed to log command; unable to parse log message.\n{player is null}\n{exception}");
                }

                if (string.IsNullOrEmpty(logMessage))
                    return;

                string directory = Path.Combine(Paths.Exiled, "Logs");

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                string filePath = Path.Combine(directory, $"{Server.Port}-RAlog.txt");

                if (!File.Exists(filePath))
                    File.Create(filePath).Close();

                File.AppendAllText(filePath, logMessage);
            }
            catch (Exception exception)
            {
                Log.Error($"{nameof(CommandLogging)}: Unable to log a command.\n{string.IsNullOrEmpty(query)} - {sender is null}\n{exception}");
            }
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(ExecutingCommandEventArgs));

            Label returnLabel = generator.DefineLabel();
            Label continueLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                0,
                new[]
                {
                    // if (!Events.Instance.Config.LogRaCommands)
                    //   goto continueLabel;
                    new(OpCodes.Call, PropertyGetter(typeof(Exiled.Events.Events), nameof(Exiled.Events.Events.Instance))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Exiled.Events.Events), nameof(Exiled.Events.Events.Config))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Config), nameof(Config.LogRaCommands))),
                    new(OpCodes.Brfalse, continueLabel),

                    // q
                    new(OpCodes.Ldarg_0),

                    // sender
                    new(OpCodes.Ldarg_1),

                    // LogCommand(q, sender)
                    new(OpCodes.Call, Method(typeof(CommandLogging), nameof(LogCommand))),

                    // continueLabel:
                    new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
                });

            int index = newInstructions.FindIndex(x => x.Is(OpCodes.Ldsfld, Field(typeof(CommandProcessor), nameof(CommandProcessor.RemoteAdminCommandHandler))));

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(CommandSender) })),

                new(OpCodes.Ldnull),

                new(OpCodes.Ldloc_0),
                new(OpCodes.Call, Method(typeof(ExecuteCommand), nameof(Convert))),

                new(OpCodes.Ldc_I4_1),

                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ExecutingCommandEventArgs))[0]),
                new(OpCodes.Stloc_S, ev.LocalIndex),
            });

            index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldloc_1);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Ldloc_1),
                new(OpCodes.Callvirt, PropertySetter(typeof(ExecutingCommandEventArgs), nameof(ExecutingCommandEventArgs.Command))),

                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnExecutingCommand))),

                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ExecutingCommandEventArgs), nameof(ExecutingCommandEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),
            });

            int offset = -3;
            index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldelem_Ref) + offset;

            newInstructions[index].labels.Add(continueLabel);

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Dup),

                new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnExecutingCommand))),

                new(OpCodes.Callvirt, PropertyGetter(typeof(ExecutingCommandEventArgs), nameof(ExecutingCommandEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, continueLabel),

                new CodeInstruction(OpCodes.Ldarg_1).WithLabels(returnLabel),
                new(OpCodes.Ldloc_0),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Ldelem_Ref),
                new(OpCodes.Callvirt, Method(typeof(string), nameof(string.ToUpperInvariant))),
                new(OpCodes.Ldstr, "#Command execution was aborted by a plugin."),
                new(OpCodes.Call, Method(typeof(string), nameof(string.Concat), new[] { typeof(string), typeof(string) })),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Ldstr, string.Empty),
                new(OpCodes.Callvirt, Method(typeof(CommandSender), nameof(CommandSender.RaReply))),

                new(OpCodes.Ldstr, "Command execution was aborted by a plugin."),
                new(OpCodes.Ret),
            });

            offset = 2;
            index = newInstructions.FindIndex(x => x.Calls(Method(typeof(Misc), nameof(Misc.CloseAllRichTextTags)))) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Dup),
                new(OpCodes.Dup),

                new(OpCodes.Callvirt, PropertyGetter(typeof(ExecutingCommandEventArgs), nameof(ExecutingCommandEventArgs.Player))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ExecutingCommandEventArgs), nameof(ExecutingCommandEventArgs.Command))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ExecutingCommandEventArgs), nameof(ExecutingCommandEventArgs.Arguments))),

                new(OpCodes.Ldloc_S, 6),

                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ExecutedCommandEventArgs))[0]),

                new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnExecutedCommand))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static ArraySegment<string> Convert(string[] strArray) => strArray.Segment(1);
    }

    [HarmonyPatch(typeof(QueryProcessor), nameof(QueryProcessor.ProcessGameConsoleQuery))]
    internal class ExecuteCommandClientConsole
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(ExecutingCommandEventArgs));

            Label continueLabel = generator.DefineLabel();
            Label returnLabel = generator.DefineLabel();

            int index = newInstructions.FindIndex(x => x.Is(OpCodes.Ldsfld, Field(typeof(QueryProcessor), nameof(QueryProcessor.DotCommandHandler))));

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(QueryProcessor), nameof(QueryProcessor._hub))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                new(OpCodes.Ldnull),

                new(OpCodes.Ldloc_0),
                new(OpCodes.Call, Method(typeof(ExecuteCommand), "Convert")),

                new(OpCodes.Ldc_I4_1),

                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ExecutingCommandEventArgs))[0]),
                new(OpCodes.Stloc_S, ev.LocalIndex),
            });

            index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldloc_1);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Ldloc_1),
                new(OpCodes.Callvirt, PropertySetter(typeof(ExecutingCommandEventArgs), nameof(ExecutingCommandEventArgs.Command))),

                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnExecutingCommand))),

                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ExecutingCommandEventArgs), nameof(ExecutingCommandEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),
            });

            int offset = -4;
            index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldelem_Ref) + offset;

            newInstructions[index].labels.Add(continueLabel);

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Dup),

                new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnExecutingCommand))),

                new(OpCodes.Callvirt, PropertyGetter(typeof(ExecutingCommandEventArgs), nameof(ExecutingCommandEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, continueLabel),

                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(returnLabel),
                new(OpCodes.Ldfld, Field(typeof(QueryProcessor), nameof(QueryProcessor._hub))),
                new(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.gameConsoleTransmission))),
                new(OpCodes.Ldstr, "Command execution was aborted by a plugin."),
                new(OpCodes.Ldstr, "red"),
                new(OpCodes.Callvirt, Method(typeof(GameConsoleTransmission), nameof(GameConsoleTransmission.SendToClient))),
                new(OpCodes.Ret),
            });

            offset = 2;
            index = newInstructions.FindIndex(x => x.Calls(Method(typeof(Misc), nameof(Misc.CloseAllRichTextTags)))) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Dup),
                new(OpCodes.Dup),

                new(OpCodes.Callvirt, PropertyGetter(typeof(ExecutingCommandEventArgs), nameof(ExecutingCommandEventArgs.Player))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ExecutingCommandEventArgs), nameof(ExecutingCommandEventArgs.Command))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ExecutingCommandEventArgs), nameof(ExecutingCommandEventArgs.Arguments))),

                new(OpCodes.Ldloc_3),

                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ExecutedCommandEventArgs))[0]),

                new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnExecutedCommand))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /*internal class ExecuteCommandGameConsole
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {

        }
    }*/
}