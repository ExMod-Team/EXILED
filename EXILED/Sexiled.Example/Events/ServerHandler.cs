// -----------------------------------------------------------------------
// <copyright file="ServerHandler.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Sexiled.Example.Events
{
    using Sexiled.API.Features;

    /// <summary>
    /// Handles server-related events.
    /// </summary>
    internal sealed class ServerHandler
    {
        /// <inheritdoc cref="Sexiled.Events.Handlers.Server.OnWaitingForPlayers"/>
        public void OnWaitingForPlayers()
        {
            Log.Info("I'm waiting for players!"); // This is an example of information messages sent to your console!
        }

        /// <inheritdoc cref="Sexiled.Events.Handlers.Server.OnRoundStarted"/>
        public void OnRoundStarted()
        {
            Log.Info($"A round has started with {Player.Dictionary.Count} players!");
        }
    }
}