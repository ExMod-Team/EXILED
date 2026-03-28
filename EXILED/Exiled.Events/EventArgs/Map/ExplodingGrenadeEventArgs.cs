// -----------------------------------------------------------------------
// <copyright file="ExplodingGrenadeEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using System.Collections.Generic;

    using Exiled.API.Features;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Pickups.Projectiles;
    using Exiled.API.Features.Pools;
    using Exiled.Events.EventArgs.Interfaces;
    using Exiled.Events.Patches.Generic;

    using Footprinting;

    using InventorySystem.Items.ThrowableProjectiles;

    using UnityEngine;

    /// <summary>
    /// Contains all information before a grenade explodes.
    /// </summary>
    public class ExplodingGrenadeEventArgs : IPlayerEvent, IDeniableEvent, IPickupEvent
    {
        private ExplosionType explosionType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExplodingGrenadeEventArgs"/> class.
        /// </summary>
        /// <param name="thrower"><inheritdoc cref="Player"/></param>
        /// <param name="position"><inheritdoc cref="Position"/></param>
        /// <param name="grenade"><inheritdoc cref="Projectile"/></param>
        /// <param name="targets"><inheritdoc cref="TargetsToAffect"/></param>
        /// <param name="explosionType"><inheritdoc cref="ExplosionType"/></param>
        public ExplodingGrenadeEventArgs(Footprint thrower, Vector3 position, ExplosionGrenade grenade, Collider[] targets, ExplosionType explosionType)
        {
            Player = Player.Get(thrower.Hub);
            Projectile = Pickup.Get<EffectGrenadeProjectile>(grenade);
            Position = position;
            TargetsToAffect = HashSetPool<Player>.Pool.Get();
            ExplosionType = explosionType;

            if (Projectile.Base is not ExplosionGrenade)
                return;

            foreach (Collider collider in targets)
            {
                if (!collider.TryGetComponent(out IDestructible destructible) || !ReferenceHub.TryGetHubNetID(destructible.NetworkId, out ReferenceHub hub))
                    continue;

                Player player = Player.Get(hub);
                if (player is null)
                    continue;

                switch (Player is null)
                {
                    case false:
                        {
                            if (Server.FriendlyFire || IndividualFriendlyFire.CheckFriendlyFirePlayer(thrower, hub))
                            {
                                TargetsToAffect.Add(player);
                            }
                        }

                        break;
                    case true:
                        {
                            if (Server.FriendlyFire || thrower.Hub == Server.Host.ReferenceHub || HitboxIdentity.IsEnemy(thrower.Role, hub.roleManager.CurrentRole.RoleTypeId))
                            {
                                TargetsToAffect.Add(player);
                            }
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExplodingGrenadeEventArgs" /> class.
        /// </summary>
        /// <param name="thrower">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="grenade">
        /// <inheritdoc cref="Projectile" />
        /// </param>
        /// <param name="targetsToAffect">
        /// <inheritdoc cref="TargetsToAffect" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public ExplodingGrenadeEventArgs(Player thrower, EffectGrenade grenade, HashSet<Player> targetsToAffect, bool isAllowed = true)
        {
            Player = thrower ?? Server.Host;
            Projectile = Pickup.Get<EffectGrenadeProjectile>(grenade);
            Position = Projectile.Position;
            ExplosionType = ExplosionType.Custom;
            TargetsToAffect = HashSetPool<Player>.Pool.Get(targetsToAffect ?? new HashSet<Player>());
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ExplodingGrenadeEventArgs"/> class.
        /// </summary>
        ~ExplodingGrenadeEventArgs()
        {
            HashSetPool<Player>.Pool.Return(TargetsToAffect);
        }

        /// <summary>
        /// Gets the position where the grenade is exploding.
        /// </summary>
        public Vector3 Position { get; }

        /// <summary>
        /// Gets or sets the Explosion type.
        /// </summary>
        /// <remarks>Explosion that are not from <see cref="ExplosionGrenadeProjectile"/> will return <see cref="ExplosionType.Custom"/> and can't be modified.</remarks>
        public ExplosionType ExplosionType
        {
            get => explosionType;
            set => explosionType = Projectile is ExplosionGrenadeProjectile ? value : ExplosionType.Custom;
        }

        /// <summary>
        /// Gets the players who could be affected by the grenade, if any, and the damage that be dealt.
        /// </summary>
        public HashSet<Player> TargetsToAffect { get; }

        /// <summary>
        /// Gets the grenade that is exploding.
        /// </summary>
        public EffectGrenadeProjectile Projectile { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the grenade can be thrown.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        /// Gets the player who thrown the grenade.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        Pickup IPickupEvent.Pickup => Projectile;
    }
}