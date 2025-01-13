// -----------------------------------------------------------------------
// <copyright file="UsingMicroHIDEnergyEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using API.Features.Items;

    using Interfaces;

    using InventorySystem.Items.MicroHID;

    /// <summary>
    /// Contains all information before MicroHID energy is changed.
    /// </summary>
    public class UsingMicroHIDEnergyEventArgs : IDeniableEvent, IItemEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsingMicroHIDEnergyEventArgs" /> class.
        /// </summary>
        /// <param name="microHIDitem">
        /// <inheritdoc cref="MicroHID" />
        /// </param>
        /// <param name="newEnergy">
        /// <inheritdoc cref="Drain" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public UsingMicroHIDEnergyEventArgs(MicroHIDItem microHIDitem, float newEnergy, bool isAllowed = true)
        {
            MicroHID = Item.Get<MicroHid>(microHIDitem);
            Drain = MicroHID.Energy - newEnergy;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the MicroHID instance.
        /// </summary>
        public MicroHid MicroHID { get; }

        /// <inheritdoc/>
        public Item Item => MicroHID;

        /// <summary>
        /// Gets or sets the MicroHID energy drain.
        /// </summary>
        public float Drain { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the MicroHID energy can be changed.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <inheritdoc/>
        public Player Player => MicroHID.Owner;
    }
}