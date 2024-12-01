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
        /// Initializes a new instance of the <see cref="ShotEventArgs"/> class.
        /// </summary>
        /// <param name="hitregModule">Hitreg module that calculated the shot.</param>
        /// <param name="hitInfo">Raycast hit info.</param>
        /// <param name="firearm">The firearm used.</param>
        /// <param name="destructible">The IDestructible that was hit. Can be null.</param>
        public ShotEventArgs(HitscanHitregModuleBase hitregModule, RaycastHit hitInfo, InventorySystem.Items.Firearms.Firearm firearm, IDestructible destructible)
        {
            Player = Player.Get(firearm.Owner);
            Firearm = Item.Get<Firearm>(firearm);

            Damage = hitInfo.collider.TryGetComponent(out IDestructible component) ? hitregModule.DamageAtDistance(hitInfo.distance) : 0f;
            Destructible = component;
            RaycastHit = hitInfo;

            if (Destructible is HitboxIdentity hitboxIdentity)
            {
                Hitbox = hitboxIdentity;
                Target = Player.Get(Hitbox.TargetHub);
            }
        }

        /// <summary>
        /// Gets the player who fired the shot.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the firearm used to shoot.
        /// </summary>
        public Firearm Firearm { get; }

        /// <inheritdoc/>
        public Item Item => Firearm;

        /// <summary>
        /// Gets the bullet travel distance.
        /// </summary>
        public float Distance => RaycastHit.distance;

        /// <summary>
        /// Gets the position of the hit.
        /// </summary>
        public Vector3 Position => RaycastHit.point;

        /// <summary>
        /// Gets the <see cref="IDestructible"/> component of the hit collider. Can be null.
        /// </summary>
        public IDestructible Destructible { get; }

        /// <summary>
        /// Gets the firearm damage at the hit distance. Actual inflicted damage may vary.
        /// </summary>
        public float Damage { get; }

        /// <summary>
        /// Gets the raycast info.
        /// </summary>
        public RaycastHit RaycastHit { get; }

        /// <summary>
        /// Gets the target player. Can be null.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets the <see cref="HitboxIdentity"/> component of the target player. Can be null.
        /// </summary>
        public HitboxIdentity Hitbox { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the shot can hurt the target.
        /// </summary>
        public bool CanHurt { get; set; } = true;
    }
}
