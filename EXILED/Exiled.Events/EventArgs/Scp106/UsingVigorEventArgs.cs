// -----------------------------------------------------------------------
// <copyright file="UsingVigorEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp106
{
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before SCP-106 using vigor.
    /// </summary>
    public class UsingVigorEventArgs : IScp106Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsingVigorEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="oldVigor"><inheritdoc cref="OldVigor"/></param>
        /// <param name="newVigor"><inheritdoc cref="NewVigor"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public UsingVigorEventArgs(Player player, float oldVigor, float newVigor, bool isAllowed = true)
        {
            Player = player;
            Scp106 = player.Role.As<Scp106Role>();
            OldVigor = oldVigor;
            NewVigor = newVigor;
            IsVigorIncreasing = newVigor > oldVigor;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player controlling SCP-106.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp106Role Scp106 { get; }

        /// <summary>
        /// Gets the value current vigor amount.
        /// </summary>
        public float OldVigor { get; }

        /// <summary>
        /// Gets or sets a value new vigor value to be applied.
        /// </summary>
        public float NewVigor { get; set; }

        /// <summary>
        /// Gets a value indicating whether gets whether vigor is increasing (true) or decreasing (false).
        /// </summary>
        public bool IsVigorIncreasing { get; }

        /// <summary>
        /// Gets or sets a value indicating whether vigor can be using.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
