// -----------------------------------------------------------------------
// <copyright file="LosingSignalEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp079
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before SCP-079 loses a signal by SCP-2176.
    /// </summary>
    public class LosingSignalEventArgs : IScp079Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LosingSignalEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        public LosingSignalEventArgs(ReferenceHub player)
        {
            Player = Player.Get(player);
            Scp079 = Player.Role.As<API.Features.Roles.Scp079Role>();
            IsAllowed = true;
        }

        /// <summary>
        /// Gets the player who's controlling SCP-079.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-079 will lose his signal.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <inheritdoc/>
        public API.Features.Roles.Scp079Role Scp079 { get; }
    }
}
