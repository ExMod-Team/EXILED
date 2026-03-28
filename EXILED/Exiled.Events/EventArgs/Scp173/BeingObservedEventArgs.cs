// -----------------------------------------------------------------------
// <copyright file="BeingObservedEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp173
{
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all the information before SCP-173 is observed.
    /// </summary>
    public class BeingObservedEventArgs : IScp173Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BeingObservedEventArgs" /> class.
        /// </summary>
        /// <param name="target">
        /// The <see cref="Exiled.API.Features.Player"/> instance of the target.
        /// </param>
        /// <param name="scp173">
        /// The <see cref="Exiled.API.Features.Player"/> instance of the SCP-173.
        /// </param>
        /// <param name="isAllowed">
        /// Whether the target will be counted as observing the SCP-173.
        /// </param>
        public BeingObservedEventArgs(API.Features.Player target, API.Features.Player scp173, bool isAllowed = true)
        {
            Target = target;
            Player = scp173;
            Scp173 = scp173.Role.As<Scp173Role>();
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's observing the SCP-173.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets the player who's being observed.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp173Role Scp173 { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the player can be counted as observing.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
