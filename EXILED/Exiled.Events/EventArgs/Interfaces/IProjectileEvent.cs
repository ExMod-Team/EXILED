// -----------------------------------------------------------------------
// <copyright file="IProjectileEvent.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces
{
    using Exiled.API.Features.Pickups.Projectiles;

    /// <summary>
    /// Event args used for all <see cref="API.Features.Pickups.Projectiles.Projectile" /> related events.
    /// </summary>
    public interface IProjectileEvent : IPickupEvent
    {
        /// <summary>
        /// Gets the <see cref="API.Features.Pickups.Projectiles.Projectile" /> triggering the event.
        /// </summary>
        public Projectile Projectile { get; }
    }
}