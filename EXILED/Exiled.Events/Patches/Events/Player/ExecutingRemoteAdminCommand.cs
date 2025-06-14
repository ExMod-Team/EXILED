// -----------------------------------------------------------------------
// <copyright file="ExecutingRemoteAdminCommand.cs" company="ExMod Team">
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
    /// Patches <see cref="CommandProcessor.ProcessQuery"/> to add the <see cref="PlayerCommands.ExecutingRemoteAdminCommand"/> event.
    /// </summary>
    [EventPatch(typeof(PlayerCommands), nameof(PlayerCommands.ExecutingRemoteAdminCommand))]
    [HarmonyPatch(typeof(CommandProcessor), nameof(CommandProcessor.ProcessQuery))]
    internal static class ExecutingRemoteAdminCommand
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();

            int index = newInstructions.FindIndex(i =>
                i.opcode == OpCodes.Ldarg_1 && // ldarg.1 (sender)
                newInstructions[newInstructions.IndexOf(i) + 1].opcode == OpCodes.Ldc_I4_1 && // ldc.i4.1
                newInstructions[newInstructions.IndexOf(i) + 2].opcode == OpCodes.Ldloc_1); // ldloc.1 (command2)

            if (index == -1)
            {
                // Log an error or throw an exception if the pattern is not found
                throw new System.Exception("Could not find the target instruction sequence for ExecutingRemoteAdminCommand patch.");
            }

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player player = Player.Get(sender);
                    new(OpCodes.Ldarg_1), // sender
                    new(OpCodes.Call, Method(typeof(LabApi.Features.Wrappers.Player), nameof(LabApi.Features.Wrappers.Player.Get), new[] { typeof(CommandSender) })),

                    // string command = array[0];
                    new(OpCodes.Ldloc_0), // array
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Ldelem_Ref),

                    // string[] arguments = array.Skip(1).ToArray();
                    new(OpCodes.Ldloc_0), // array
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Call, Method(typeof(System.Linq.Enumerable), nameof(System.Linq.Enumerable.Skip), null, new[] { typeof(string) })),
                    new(OpCodes.Call, Method(typeof(System.Linq.Enumerable), nameof(System.Linq.Enumerable.ToArray), null, new[] { typeof(string) })),

                    // bool isAllowed = true;
                    new(OpCodes.Ldc_I4_1),

                    // ExecutingRemoteAdminCommandEventArgs ev = new(player, command, arguments, isAllowed);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ExecutingRemoteAdminCommandEventArgs))[0]),
                    new(OpCodes.Dup),

                    // PlayerCommands.OnExecutingRemoteAdminCommand(ev);
                    new(OpCodes.Call, Method(typeof(PlayerCommands), nameof(PlayerCommands.OnExecutingRemoteAdminCommand))),

                    // if (!ev.IsAllowed)
                    //     return null;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ExecutingRemoteAdminCommandEventArgs), nameof(ExecutingRemoteAdminCommandEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueLabel),
                    new(OpCodes.Ldnull),
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