// -----------------------------------------------------------------------
// <copyright file="RestartingRound.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using CustomPlayerEffects.Danger;
    using Exiled.API.Enums;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using GameCore;

    using HarmonyLib;

    using RoundRestarting;

    using static HarmonyLib.AccessTools;
    using static PlayerList;

    /// <summary>
    /// Patches <see cref="RoundRestart.InitiateRoundRestart"/>.
    /// Adds the <see cref="Handlers.Server.RestartingRound" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Server), nameof(Handlers.Server.RestartingRound))]
    [HarmonyPatch(typeof(RoundRestart), nameof(RoundRestart.IsRoundRestarting), MethodType.Setter)]
    internal static class RestartingRound
    {
        // TODO: Convert to transpiler and bring back old features
        private static void Prefix(bool value)
        {
            if (!value || value == RoundRestart.IsRoundRestarting)
                return;

            Handlers.Server.OnRestartingRound();
        }
    }
}
