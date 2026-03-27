// -----------------------------------------------------------------------
// <copyright file="RoundStartingEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before the start of a round.
    /// </summary>
    public class RoundStartingEventArgs : IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoundStartingEventArgs" /> class.
        /// </summary>
        /// <param name="timeLeft"><inheritdoc cref="TimeLeft"/></param>
        /// <param name="playerCount"><inheritdoc cref="PlayerCount"/></param>
        /// <param name="minimumPlayerCount"><inheritdoc cref="MinimumPlayerCount"/></param>
        /// <param name="originalTimeLeft"><inheritdoc cref="OriginalTimeLeft"/></param>
        public RoundStartingEventArgs(short timeLeft, short originalTimeLeft, int minimumPlayerCount, int playerCount)
        {
            TimeLeft = timeLeft;
            OriginalTimeLeft = originalTimeLeft;
            MinimumPlayerCount = minimumPlayerCount;
            PlayerCount = playerCount;
            IsAllowed = TimeLeft == -1;
        }

        /// <summary>
        /// Gets or sets the time before the start of the Round.
        /// </summary>
        public int TimeLeft { get; set; }

        /// <summary>
        /// Gets or sets the time before the start of the Round.
        /// </summary>
        public int OriginalTimeLeft { get; set; }

        /// <summary>
        /// Gets or sets the number of Player.
        /// </summary>
        public int MinimumPlayerCount { get; set; }

        /// <summary>
        /// Gets the number of Player.
        /// </summary>
        public int PlayerCount { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }
    }
}
