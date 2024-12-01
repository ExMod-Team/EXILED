// -----------------------------------------------------------------------
// <copyright file="ShotEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Exiled.API.Features.Items;
    using Interfaces;
    using InventorySystem.Items.Firearms.Modules;
    using UnityEngine;

    /// <summary>
    /// Contains all information after a player has fired a weapon.
    /// </summary>
    public class ShotEventArgs : IPlayerEvent, IFirearmEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShotEventArgs" /> class.
        /// </summary>
        /// <param name="instance">
        /// The <see cref="HitscanHitregModuleBase"/> instance.
        /// </param>
        /// <param name="firearm">
        /// <inheritdoc cref="Firearm"/>
        /// </param>
        /// <param name="ray">
        /// <inheritdoc cref="Ray" />
        /// </param>
        /// <param name="maxDistance">
        /// <inheritdoc cref="MaxDistance" />
        /// </param>
        public ShotEventArgs(HitscanHitregModuleBase instance, InventorySystem.Items.Firearms.Firearm firearm, Ray ray, float maxDistance)
        {
            Player = Player.Get(firearm.Owner);
            Firearm = Item.Get<Firearm>(firearm);
            MaxDistance = maxDistance;

            if (!Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance, HitscanHitregModuleBase.HitregMask))
                return;

            Distance = hitInfo.distance;
            Position = hitInfo.point;
            Damage = hitInfo.collider.TryGetComponent(out IDestructible component) ? instance.DamageAtDistance(hitInfo.distance) : 0f;
            Destructible = component;
            RaycastHit = hitInfo;

            if (component is HitboxIdentity identity)
            {
                Hitbox = identity;
                Target = Player.Get(Hitbox.TargetHub);
            }
        }

        /// <summary>
        /// Gets the player who shot.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the firearm used to shoot.
        /// </summary>
        public Firearm Firearm { get; }

        /// <inheritdoc/>
        public Item Item => Firearm;

        /// <summary>
        /// Gets the max distance of the shot.
        /// </summary>
        public float MaxDistance { get;  }

        /// <summary>
        /// Gets the shot distance. Can be <c>0.0f</c> if the raycast doesn't hit collider.
        /// </summary>
        public float Distance { get; }

        /// <summary>
        /// Gets the shot position. Can be <see langword="null"/> if the raycast doesn't hit collider.
        /// </summary>
        public Vector3 Position { get; }

        /// <summary>
        /// Gets the <see cref="IDestructible"/> component of the hit collider. Can be <see langword="null"/>.
        /// </summary>
        public IDestructible Destructible { get; }

        /// <summary>
        /// Gets the inflicted damage.
        /// </summary>
        public float Damage { get; }

        /// <summary>
        /// Gets the raycast result.
        /// </summary>
        public RaycastHit RaycastHit { get; }

        /// <summary>
        /// Gets the target of the shot. Can be <see langword="null"/>.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets the <see cref="HitboxIdentity"/> component of the hit collider. Can be <see langword="null"/>.
        /// </summary>
        public HitboxIdentity Hitbox { get; }
    }
}
