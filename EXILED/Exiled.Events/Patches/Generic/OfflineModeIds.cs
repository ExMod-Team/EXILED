// -----------------------------------------------------------------------
// <copyright file="OfflineModeIds.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1402 // File may only contain a single type
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using CentralAuth;
    using HarmonyLib;
    using PluginAPI.Core.Interfaces;
    using PluginAPI.Events;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="PlayerAuthenticationManager.Start"/> to add an @offline suffix to UserIds in Offline Mode.
    /// </summary>
    [HarmonyPatch(typeof(PlayerAuthenticationManager), nameof(PlayerAuthenticationManager.Start))]
    internal static class OfflineModeIds
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = -1;
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Call && instruction.OperandIs(PropertySetter(typeof(PlayerAuthenticationManager), nameof(PlayerAuthenticationManager.UserId)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    new CodeInstruction(OpCodes.Call, Method(typeof(OfflineModeIds), nameof(BuildUserId))),
                });

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static string BuildUserId(string userId) => $"{userId}@offline";
    }

    /// <summary>
    ///     Patches <see cref="PlayerAuthenticationManager.Start"/> to add the player's UserId to the <see cref="PluginAPI.Core.Player.PlayersUserIds"/> dictionary.
    /// </summary>
    [HarmonyPatch(typeof(PlayerAuthenticationManager), nameof(PlayerAuthenticationManager.Start))]
    internal static class OfflineModePlayerIds
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label skipLabel = generator.DefineLabel();

            const int offset = 1;
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Call && instruction.OperandIs(PropertySetter(typeof(PlayerAuthenticationManager), nameof(PlayerAuthenticationManager.UserId)))) + offset;

            // if (!Player.PlayersUserIds.ContainsKey(this.UserId))
            //       Player.PlayersUserIds.Add(this.UserId, this._hub);
            newInstructions.InsertRange(
                index,
                new[]
                {
                    // if (Player.PlayersUserIds.ContainsKey(this.UserId)) goto skip;
                    new(OpCodes.Ldsfld, Field(typeof(PluginAPI.Core.Player), nameof(PluginAPI.Core.Player.PlayersUserIds))),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(PlayerAuthenticationManager), nameof(PlayerAuthenticationManager.UserId))),
                    new(OpCodes.Callvirt, Method(typeof(Dictionary<string, IGameComponent>), nameof(Dictionary<string, IGameComponent>.ContainsKey))),
                    new(OpCodes.Brtrue_S, skipLabel),

                    // Player.PlayersUserIds.Add(this.UserId, this._hub);
                    new(OpCodes.Ldsfld, Field(typeof(PluginAPI.Core.Player), nameof(PluginAPI.Core.Player.PlayersUserIds))),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(PlayerAuthenticationManager), nameof(PlayerAuthenticationManager.UserId))),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(PlayerAuthenticationManager), nameof(PlayerAuthenticationManager._hub))),
                    new(OpCodes.Callvirt, Method(typeof(Dictionary<string, IGameComponent>), nameof(Dictionary<string, IGameComponent>.Add))),

                    // skip:
                    new CodeInstruction(OpCodes.Nop).WithLabels(skipLabel),
                });

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    ///     Patches <see cref="ReferenceHub.Start"/> to prevent it from executing the <see cref="PluginAPI.Events.PlayerLeftEvent"/> event when the server is in offline mode.
    /// </summary>
    [HarmonyPatch(typeof(ReferenceHub), nameof(ReferenceHub.Start))]
    internal static class OfflineModeReferenceHub
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = 1;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Callvirt) + offset;

            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new[]
                {
                    new CodeInstruction(OpCodes.Br_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    ///     Patches <see cref="NicknameSync.UserCode_CmdSetNick__String"/> to execute the <see cref="PlayerJoinedEvent"/> event when the server is in offline mode.
    /// </summary>
    [HarmonyPatch(typeof(NicknameSync), nameof(NicknameSync.UserCode_CmdSetNick__String))]
    internal static class OfflineModeJoin
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = 1;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Callvirt && x.OperandIs(Method(typeof(CharacterClassManager), nameof(CharacterClassManager.SyncServerCmdBinding)))) + offset;

            // EventManager.ExecuteEvent(new PlayerJoinedEvent(this._hub));
            newInstructions.InsertRange(
                index,
                new[]
                {
                    // EventManager.ExecuteEvent(new PlayerJoinedEvent(this._hub));
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, Field(typeof(NicknameSync), nameof(NicknameSync._hub))),
                    new CodeInstruction(OpCodes.Call, Method(typeof(OfflineModeJoin), nameof(ExecuteNwEvent))),
                });

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void ExecuteNwEvent(ReferenceHub hub)
        {
            EventManager.ExecuteEvent(new PlayerJoinedEvent(hub));
        }
    }
}