// -----------------------------------------------------------------------
// <copyright file="RevokingMuteEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before unmuting a player.
    /// </summary>
    public class RevokingMuteEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RevokingMuteEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///    The player who's being unmuted.
        /// </param>
        /// <param name="isIntercom">
        ///    Indicates whether the player is being intercom unmuted.
        /// </param>
        /// <param name="isAllowed">
        ///    Indicates whether the player can be unmuted.
        /// </param>
        public RevokingMuteEventArgs(Player player, bool isIntercom, bool isAllowed = true)
        {
            Player = player;
            IsIntercom = isIntercom;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's being revoking the mute.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the player is being revoking intercom muted.
        /// </summary>
        public bool IsIntercom { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player can be revoked muted.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}