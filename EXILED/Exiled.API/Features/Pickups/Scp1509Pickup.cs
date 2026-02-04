// -----------------------------------------------------------------------
// <copyright file="Scp1509Pickup.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using Exiled.API.Features.Items;
    using Exiled.API.Interfaces;
    using InventorySystem.Items;
    using InventorySystem.Items.Scp1509;

    using BaseScp1509 = InventorySystem.Items.Scp1509.Scp1509Pickup;

    /// <summary>
    /// A wrapper class for a Scp1509 pickup.
    /// </summary>
    public class Scp1509Pickup : Pickup, IWrapper<BaseScp1509>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp1509Pickup"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="BaseScp1509"/> class.</param>
        internal Scp1509Pickup(BaseScp1509 pickupBase)
            : base(pickupBase)
        {
            Base = pickupBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp1509Pickup"/> class.
        /// </summary>
        internal Scp1509Pickup()
            : base(ItemType.SCP1509)
        {
            Base = (BaseScp1509)((Pickup)this).Base;
        }

        /// <summary>
        /// Gets the <see cref="BaseScp1509"/> that this class is encapsulating.
        /// </summary>
        public new BaseScp1509 Base { get; }

        /// <summary>
        /// Gets or sets the shield regeneration rate.
        /// </summary>
        public float ShieldRegenRate { get; set; }

        /// <summary>
        /// Gets or sets the shield decay rate.
        /// </summary>
        public float ShieldDecayRate { get; set; }

        /// <summary>
        /// Gets or sets the shield time pause when player get damage.
        /// </summary>
        public float ShieldOnDamagePause { get; set; }

        /// <summary>
        /// Gets or sets the delay after the decay start.
        /// </summary>
        public float UnequipDecayDelay { get; set; }

        /// <summary>
        /// Returns the Scp1509Pickup in a human readable format.
        /// </summary>
        /// <returns>A string containing Scp1509Pickup related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{ShieldRegenRate}| -{ShieldDecayRate}- /{ShieldOnDamagePause}/ ^{UnequipDecayDelay}^";

        /// <inheritdoc/>
        internal override void ReadItemInfo(Item item)
        {
            base.ReadItemInfo(item);
            if (item is Scp1509 scp1509Item)
            {
                ShieldRegenRate = scp1509Item.ShieldRegenRate;
                ShieldDecayRate = scp1509Item.ShieldDecayRate;
                ShieldOnDamagePause = scp1509Item.ShieldOnDamagePause;
                UnequipDecayDelay = scp1509Item.UnequipDecayDelay;
            }
        }

        /// <inheritdoc/>
        protected override void InitializeProperties(ItemBase itemBase)
        {
            base.InitializeProperties(itemBase);
            if (itemBase is Scp1509Item scp1509Item)
            {
                ShieldRegenRate = scp1509Item.ShieldRegenRate;
                ShieldDecayRate = scp1509Item.ShieldDecayRate;
                ShieldOnDamagePause = scp1509Item.ShieldOnDamagePause;
                UnequipDecayDelay = scp1509Item.UnequipDecayDelay;
            }
        }
    }
}
