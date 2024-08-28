// -----------------------------------------------------------------------
// <copyright file="BeingObservedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp173
{
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all the information before being observed.
    /// </summary>
    public class BeingObservedEventArgs : IScp173Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BeingObservedEventArgs" /> class.
        /// </summary>
        /// <param name="target">
        /// <inheritdoc cref="Target" />
        /// </param>
        /// <param name="scp173">
        /// <inheritdoc cref="Player"/>
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed"/>
        /// </param>
        public BeingObservedEventArgs(API.Features.Player target, API.Features.Player scp173, bool isAllowed = true)
        {
            Target = target;
            Player = scp173;
            Scp173 = scp173.Role.As<Scp173Role>();
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's observing the Scp 173.
        /// </summary>
        public API.Features.Player Target { get; }

        /// <summary>
        /// Gets the player who's being observed.
        /// </summary>
        public API.Features.Player Player { get; }

        /// <inheritdoc/>
        public Scp173Role Scp173 { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can be counted as observing.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
