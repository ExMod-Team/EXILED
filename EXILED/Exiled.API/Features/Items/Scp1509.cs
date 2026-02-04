// -----------------------------------------------------------------------
// <copyright file="Scp1509.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Interfaces;
    using InventorySystem.Items.Scp1509;
    using PlayerRoles;

    /// <summary>
    /// A wrapper class for <see cref="Scp1509Item"/>.
    /// </summary>
    public class Scp1509 : Item, IWrapper<Scp1509Item>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp1509"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="Scp1509Item"/> class.</param>
        public Scp1509(Scp1509Item itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp1509"/> class.
        /// </summary>
        internal Scp1509()
            : this((Scp1509Item)Server.Host.Inventory.CreateItemInstance(new(ItemType.SCP1509, 0), false))
        {
        }

        /// <summary>
        /// Gets the <see cref="Scp1509Item"/> that this class is encapsulating.
        /// </summary>
        public new Scp1509Item Base { get; }

        /// <summary>
        /// Gets the <see cref="Scp1509RespawnEligibility"/> instance.
        /// </summary>
        public Scp1509RespawnEligibility RespawnEligibility => Base._respawnEligibility;

        /// <summary>
        /// Gets or sets the shield regeneration rate.
        /// </summary>
        public float ShieldRegenRate
        {
            get => Base.ShieldRegenRate;
            set => Base.ShieldRegenRate = value;
        }

        /// <summary>
        /// Gets or sets the shield decay rate.
        /// </summary>
        public float ShieldDecayRate
        {
            get => Base.ShieldDecayRate;
            set => Base.ShieldDecayRate = value;
        }

        /// <summary>
        /// Gets or sets the shield time pause when player get damage.
        /// </summary>
        public float ShieldOnDamagePause
        {
            get => Base.ShieldOnDamagePause;
            set => Base.ShieldOnDamagePause = value;
        }

        /// <summary>
        /// Gets or sets the delay after the decay start.
        /// </summary>
        public float UnequipDecayDelay
        {
            get => Base.UnequipDecayDelay;
            set => Base.UnequipDecayDelay = value;
        }

        /// <summary>
        /// Gets or sets the time when resurrection ability will be available again.
        /// </summary>
        public double NextResurrectTime
        {
            get => Base._nextResurrectTime;
            set => Base._nextResurrectTime = value;
        }

        /// <summary>
        /// Gets or sets the cooldown for a melee attack.
        /// </summary>
        public float MeleeCooldown
        {
            get => Base._meleeCooldown;
            set => Base._meleeCooldown = value; // TODO not syned with clients, tests required
        }

        /// <summary>
        /// Gets or sets the amount of AHP bonus that all revived players are receiving.
        /// </summary>
        public float RevivedAhpBonus
        {
            get => Base._revivedPlayerAOEBonusAHP;
            set => Base._revivedPlayerAOEBonusAHP = value;
        }

        /// <summary>
        /// Gets or sets the distance in which all revived players will receive AHP bonus.
        /// </summary>
        public float RevivedAhpBonusDistance
        {
            get => Base._revivedPlayerAOEBonusAHPDistance;
            set => Base._revivedPlayerAOEBonusAHPDistance = value;
        }

        /// <summary>
        /// Gets or sets the max amount of HumeShield that can owner receive.
        /// </summary>
        public float MaxHs
        {
            get => Base._equippedHS;
            set => Base._equippedHS = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="EffectType.Blurred"/> duration for a revived player.
        /// </summary>
        public float RevivedBlurTime
        {
            get => Base._revivedPlayerBlurTime;
            set => Base._revivedPlayerBlurTime = value;
        }

        /// <summary>
        /// Gets or sets all revived players.
        /// </summary>
        public IEnumerable<Player> RevivedPlayers
        {
            get => Base._revivedPlayers.Select(Player.Get);
            set => Base._revivedPlayers = value.Select(x => x.ReferenceHub).ToList();
        }

        /// <summary>
        /// Gets a player that is eligible for respawn as a <paramref name="roleTypeId"/>.
        /// </summary>
        /// <param name="roleTypeId">Role to respawn.</param>
        /// <returns>Found player or <c>null</c>.</returns>
        public Player GetEligibleSpectator(RoleTypeId roleTypeId) => Player.Get(RespawnEligibility.GetEligibleSpectator(roleTypeId));

        /// <summary>
        /// Checks if there is any eligible spectator for spawn.
        /// </summary>
        /// <returns><c>true</c> if any spectator is found. Otherwise, <c>false</c>.</returns>
        public bool IsAnyEligibleSpectators() => RespawnEligibility.IsAnyEligibleSpectators();

        /// <summary>
        /// Clones current <see cref="Scp1509"/> object.
        /// </summary>
        /// <returns> New <see cref="Scp1509"/> object. </returns>
        public override Item Clone() => new Scp1509()
        {
            ShieldRegenRate = ShieldRegenRate,
            ShieldDecayRate = ShieldDecayRate,
            ShieldOnDamagePause = ShieldOnDamagePause,
            UnequipDecayDelay = UnequipDecayDelay,
        };
    }
}
