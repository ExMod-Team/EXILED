// -----------------------------------------------------------------------
// <copyright file="ThrowingProjectileEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Pickups.Projectiles;
    using Exiled.Events.EventArgs.Interfaces;

    using InventorySystem.Items.ThrowableProjectiles;

    /// <summary>
    /// Contains all information before player throws a grenade.
    /// </summary>
    public class ThrowingProjectileEventArgs : IPlayerEvent, IItemEvent, IProjectileEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThrowingProjectileEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="item"><inheritdoc cref="Throwable"/></param>
        /// <param name="projectile"><inheritdoc cref="Projectile"/></param>
        public ThrowingProjectileEventArgs(ThrownProjectile projectile, Player player, ThrowableItem item)
        {
            Player = player;
            Throwable = Item.Get<Throwable>(item);
            Projectile = Pickup.Get<Projectile>(projectile);
        }

        /// <summary>
        /// Gets the player who's thrown the grenade.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the item being thrown.
        /// </summary>
        public Throwable Throwable { get; }

        /// <inheritdoc/>
        public Item Item => Throwable;

        /// <inheritdoc/>
        public Projectile Projectile { get; }

        /// <inheritdoc/>
        public Pickup Pickup => Projectile;
    }
}
