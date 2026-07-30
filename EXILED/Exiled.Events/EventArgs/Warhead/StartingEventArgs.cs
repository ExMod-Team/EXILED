// -----------------------------------------------------------------------
// <copyright file="StartingEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Warhead
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before starting the warhead.
    /// </summary>
    public class StartingEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartingEventArgs" /> class.
        /// </summary>
        /// <param name="player">The player who's going to start the warhead.</param>
        /// <param name="isAuto">Indicating whether the nuke was set off automatically.</param>
        /// <param name="isAllowed">Indicating whether the event can be executed.</param>
        public StartingEventArgs(Player player, bool isAuto, bool isAllowed = true)
        {
            IsAuto = isAuto;
            Player = player ?? Server.Host;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the nuke was set off automatically.
        /// </summary>
        public bool IsAuto { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the warhead can be started.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the player who's going to start the warhead.
        /// </summary>
        public Player Player { get; }
    }
}