// -----------------------------------------------------------------------
// <copyright file="MicroHIDPickup.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features.Items;
using InventorySystem.Items.MicroHID.Modules;

namespace Exiled.API.Features.Pickups
{
    using Exiled.API.Interfaces;

    using BaseMicroHID = InventorySystem.Items.MicroHID.MicroHIDPickup;

    /// <summary>
    /// A wrapper class for a MicroHID pickup.
    /// </summary>
    public class MicroHIDPickup : Pickup, IWrapper<BaseMicroHID>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroHIDPickup"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="BaseMicroHID"/> class.</param>
        internal MicroHIDPickup(BaseMicroHID pickupBase)
            : base(pickupBase)
        {
            Base = pickupBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroHIDPickup"/> class.
        /// </summary>
        internal MicroHIDPickup()
            : base(ItemType.MicroHID)
        {
            Base = (BaseMicroHID)((Pickup)this).Base;
        }

        /// <summary>
        /// Gets the <see cref="BaseMicroHID"/> that this class is encapsulating.
        /// </summary>
        public new BaseMicroHID Base { get; }

        /// <summary>
        /// Gets the <see cref="InventorySystem.Items.MicroHID.Modules.CycleController"/> of this <see cref="MicroHIDPickup"/>.
        /// </summary>
        public CycleController CycleController => Base._cycleController;

        /// <summary>
        /// Gets or sets the MicroHID Energy Level.
        /// </summary>
        public float Energy { get; set; }

        /// <summary>
        /// Returns the MicroHIDPickup in a human readable format.
        /// </summary>
        /// <returns>A string containing MicroHIDPickup related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Energy}|";

        internal override void ReadItemInfo(Item item)
        {
            base.ReadItemInfo(item);

            if (item.Is(out MicroHid microHid))
            {
                Energy = microHid.Energy;
            }
        }
    }
}
