// -----------------------------------------------------------------------
// <copyright file="Scp1509.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using Exiled.API.Interfaces;
    using InventorySystem.Items.Scp1509;

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
