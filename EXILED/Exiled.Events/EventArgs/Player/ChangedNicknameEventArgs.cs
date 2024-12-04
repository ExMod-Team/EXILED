// -----------------------------------------------------------------------
// <copyright file="ChangedNicknameEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information after changed a player's in-game nickname.
    /// </summary>
    public class ChangedNicknameEventArgs : IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangedNicknameEventArgs"/> class.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> who's name is changed.</param>
        /// <param name="oldName">The old name.</param>
        public ChangedNicknameEventArgs(Player player, string oldName)
        {
            Player = player;
            OldName = oldName;
            NewName = player.CustomName;
        }

        /// <summary>
        /// Gets the <see cref="Player"/>'s old name.
        /// </summary>
        public string OldName { get; }

        /// <summary>
        /// Gets or sets the <see cref="Player"/>'s new name.
        /// </summary>
        public string NewName { get; set; }

        /// <summary>
        /// Gets the <see cref="API.Features.Player"/> who's name is changed.
        /// </summary>
        public Player Player { get; }
    }
}