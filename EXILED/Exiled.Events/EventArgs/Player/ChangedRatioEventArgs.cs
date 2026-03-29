// -----------------------------------------------------------------------
// <copyright file="ChangedRatioEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information after a player's Aspect Ratio changes.
    /// </summary>
    public class ChangedRatioEventArgs : IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangedRatioEventArgs"/> class.
        /// </summary>
        /// <param name="player">The player who is changed ratio.
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="oldratio">Old aspect ratio of the player.
        /// <inheritdoc cref="float" />
        /// </param>
        /// <param name="newratio">New aspect ratio of the player.
        /// <inheritdoc cref="float" />
        /// </param>
        public ChangedRatioEventArgs(ReferenceHub player, float oldratio, float newratio)
        {
            Player = Player.Get(player);
            OldRatio = oldratio.GetAspectRatioLabel();
            NewRatio = newratio.GetAspectRatioLabel();
        }

        /// <summary>
        /// Gets the player who is changed ratio.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the players old ratio.
        /// </summary>
        public AspectRatioType OldRatio { get; }

        /// <summary>
        /// Gets the players new ratio.
        /// </summary>
        public AspectRatioType NewRatio { get; }
    }
}
