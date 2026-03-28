// -----------------------------------------------------------------------
// <copyright file="Verified.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using CentralAuth;
    using Exiled.API.Extensions;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PlayerAuthenticationManager.FinalizeAuthentication" />.
    /// Adds the <see cref="Handlers.Player.Verified" /> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerAuthenticationManager), nameof(PlayerAuthenticationManager.FinalizeAuthentication))]
    internal static class Verified
    {
        /// <summary>
        ///     Called after the player has been verified.
        /// </summary>
        /// <param name="hub">The player's hub.</param>
        internal static void PlayerVerified(ReferenceHub hub)
        {
            if (!Player.UnverifiedPlayers.TryGetValue(hub.gameObject, out Player player))
                Joined.CallEvent(hub, out player);

            Player.Dictionary.Add(hub.gameObject, player);

            player.IsVerified = true;
            player.RawUserId = player.UserId.GetRawUserId();

            Log.SendRaw($"Player {player.Nickname} ({player.UserId}) ({player.Id}) connected with the IP: {player.IPAddress}", ConsoleColor.Green);

            Handlers.Player.OnVerified(new VerifiedEventArgs(player));
        }

        private static void Postfix(PlayerAuthenticationManager __instance)
        {
            PlayerVerified(__instance._hub);
        }
    }

    /// <summary>
    ///     Patches <see cref="NicknameSync.UserCode_CmdSetNick__String" />.
    ///     Adds the <see cref="Handlers.Player.Verified" /> event during offline mode.
    /// </summary>
    [HarmonyPatch(typeof(NicknameSync), nameof(NicknameSync.UserCode_CmdSetNick__String))]
    internal static class VerifiedOfflineMode
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = 1;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Callvirt && x.OperandIs(Method(typeof(CharacterClassManager), nameof(CharacterClassManager.SyncServerCmdBinding)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Verified.PlayerVerified(this._hub);
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, Field(typeof(NicknameSync), nameof(NicknameSync._hub))),
                    new CodeInstruction(OpCodes.Call, Method(typeof(Verified), nameof(Verified.PlayerVerified))),
                });

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}