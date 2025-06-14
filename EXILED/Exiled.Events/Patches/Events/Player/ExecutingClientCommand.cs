// -----------------------------------------------------------------------
// <copyright file="ExecutingClientCommand.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;

    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using RemoteAdmin;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="QueryProcessor.ProcessGameConsoleQuery"/> to add the <see cref="PlayerCommands.ExecutingClientCommand"/> event.
    /// </summary>
    [EventPatch(typeof(PlayerCommands), nameof(PlayerCommands.ExecutingClientCommand))]
    [HarmonyPatch(typeof(QueryProcessor), nameof(QueryProcessor.ProcessGameConsoleQuery))]
    internal static class ExecutingClientCommand
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();

            // Find the index where CommandExecutingEventArgs is created by looking for the sequence starting with ldloc.0
            // This corresponds to IL_0036 in the original IL code, where sender1 is loaded before creating CommandExecutingEventArgs
            int index = newInstructions.FindIndex(i =>
                i.opcode == OpCodes.Ldloc_0 && // ldloc.0 (sender1)
                newInstructions[newInstructions.IndexOf(i) + 1].opcode == OpCodes.Ldc_I4_2 && // ldc.i4.2
                newInstructions[newInstructions.IndexOf(i) + 2].opcode == OpCodes.Ldloc_3); // ldloc.3 (command2)

            if (index == -1)
            {
                // Log an error or throw an exception if the pattern is not found
                throw new System.Exception("Could not find the target instruction sequence for ExecutingClientCommand patch.");
            }

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player player = Player.Get(sender1);
                    new(OpCodes.Ldloc_0), // sender1
                    new(OpCodes.Call, Method(typeof(LabApi.Features.Wrappers.Player), nameof(LabApi.Features.Wrappers.Player.Get), new[] { typeof(CommandSender) })),

                    // string command = array[0];
                    new(OpCodes.Ldloc_1), // array
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Ldelem_Ref),

                    // string[] arguments = array.Skip(1).ToArray();
                    new(OpCodes.Ldloc_1), // array
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Call, Method(typeof(System.Linq.Enumerable), nameof(System.Linq.Enumerable.Skip), null, new[] { typeof(string) })),
                    new(OpCodes.Call, Method(typeof(System.Linq.Enumerable), nameof(System.Linq.Enumerable.ToArray), null, new[] { typeof(string) })),

                    // bool isAllowed = true;
                    new(OpCodes.Ldc_I4_1),

                    // ExecutingClientCommandEventArgs ev = new(player, command, arguments, isAllowed);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ExecutingClientCommandEventArgs))[0]),
                    new(OpCodes.Dup),

                    // PlayerCommands.OnExecutingClientCommand(ev);
                    new(OpCodes.Call, Method(typeof(PlayerCommands), nameof(PlayerCommands.OnExecutingClientCommand))),

                    // if (!ev.IsAllowed)
                    //     return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ExecutingClientCommandEventArgs), nameof(ExecutingClientCommandEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueLabel),
                    new(OpCodes.Ret),

                    // Continue with original code
                    new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}