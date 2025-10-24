// -----------------------------------------------------------------------
// <copyright file="AttackingEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp1509
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.DamageHandlers;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;
    using InventorySystem.Items.Scp1509;

    /// <summary>
    /// Contains all information before SCP-1509 attack.
    /// </summary>
    public class AttackingEventArgs : IAttackerEvent, IItemEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttackingEventArgs"/> class.
        /// </summary>
        /// <param name="attacker"><inheritdoc cref="Attacker"/></param>
        /// <param name="destructible"><inheritdoc cref="Hitbox"/></param>
        /// <param name="damage"><inheritdoc cref="Damage"/></param>
        /// <param name="scp1509Item"><inheritdoc cref="Scp1509"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public AttackingEventArgs(Player attacker, IDestructible destructible, float damage, Scp1509Item scp1509Item, bool isAllowed = true)
        {
            Destructible = destructible;

            if (destructible is HitboxIdentity hitboxIdentity)
            {
                Player = Player.Get(hitboxIdentity.TargetHub);
                Hitbox = hitboxIdentity;
            }
            else
            {
                Player = null;
                Hitbox = null;
            }

            Attacker = attacker;
            Scp1509 = Item.Get<Scp1509>(scp1509Item);
            DamageHandler = new(Player, attacker, damage, DamageType.Scp1509);
            Damage = damage;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc />
        /// <remarks>May be <c>null</c>.</remarks>
        public Player Player { get; }

        /// <inheritdoc />
        public Player Attacker { get; }

        /// <inheritdoc />
        public CustomDamageHandler DamageHandler { get; set; }

        /// <inheritdoc />
        public Item Item => Scp1509;

        /// <summary>
        /// Gets the SCP-1509 instance.
        /// </summary>
        public Scp1509 Scp1509 { get; }

        /// <summary>
        /// Gets the attacked hitbox.
        /// </summary>
        /// <remarks>May be <c>null</c>.</remarks>
        public HitboxIdentity Hitbox { get; }

        /// <summary>
        /// Gets the <see cref="IDestructible"/> that's being attacked.
        /// </summary>
        public IDestructible Destructible { get; }

        /// <summary>
        /// Gets or sets the amount of damage to be dealt.
        /// </summary>
        public float Damage { get; set; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; }
    }
}