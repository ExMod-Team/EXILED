// -----------------------------------------------------------------------
// <copyright file="PlayingFootstepEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp939
{
    using API.Features;
    using Exiled.API.Features.Roles;
    using Interfaces;

    /// <summary>
    /// Contains all information before the footsteps are being shown to SCP-939.
    /// </summary>
    public class PlayingFootstepEventArgs : IScp939Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayingFootstepEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// The player who's controlling SCP-939.
        /// </param>
        /// <param name="target">
        /// The player who's being shown to SCP-939.
        /// </param>
        /// <param name="isAllowed">
        /// Indicates whether the footstep action is allowed.
        /// </param>
        public PlayingFootstepEventArgs(Player target, Player player, bool isAllowed = true)
        {
            Player = player;
            Scp939 = Player.Role.As<Scp939Role>();
            Target = target;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's controlling SCP-939.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp939Role Scp939 { get; }

        /// <summary>
        /// Gets the player who's being shown to SCP-939.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets or sets a value indicating whether footsteps are visible.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
