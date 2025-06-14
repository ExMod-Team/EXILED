// -----------------------------------------------------------------------
// <copyright file="RoundStartedEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Server
{
    using System;

    /// <summary>
    /// Contains information about the round start event.
    /// </summary>
    public class RoundStartedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoundStartedEventArgs"/> class.
        /// </summary>
        /// <param name="roundStartTime">The UTC time when the round started.</param>
        /// <param name="playerCount">The number of non-NPC, non-host players in the server at round start.</param>
        /// <param name="keepRoundOnOne">Indicates whether the round continues with one player remaining.</param>
        public RoundStartedEventArgs(DateTime roundStartTime, int playerCount, bool keepRoundOnOne)
        {
            RoundStartTime = roundStartTime;
            PlayerCount = playerCount;
            KeepRoundOnOne = keepRoundOnOne;
        }

        /// <summary>
        /// Gets the UTC time when the round started.
        /// </summary>
        public DateTime RoundStartTime { get; }

        /// <summary>
        /// Gets the number of non-NPC, non-host players in the server at the time the round started.
        /// </summary>
        public int PlayerCount { get; }

        /// <summary>
        /// Gets a value indicating whether the round continues with one player remaining.
        /// </summary>
        public bool KeepRoundOnOne { get; }
    }
}