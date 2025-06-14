// -----------------------------------------------------------------------
// <copyright file="ServerHandler.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.Events.EventArgs.Server;

namespace Exiled.Example.Events
{
    using Exiled.API.Features;

    /// <summary>
    /// Handles server-related events.
    /// </summary>
    internal sealed class ServerHandler
    {
        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnWaitingForPlayers"/>
        public void OnWaitingForPlayers()
        {
            Log.Info("I'm waiting for players!"); // This is an example of information messages sent to your console!
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRoundStarted"/>
        public void OnRoundStarted(RoundStartedEventArgs ev)
        {
            if (ev.KeepRoundOnOne)
            {
                Log.Info($"Round started at {ev.RoundStartTime:yyyy-MM-dd HH:mm:ss} with players!");
            }
        }
    }
}