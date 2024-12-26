// -----------------------------------------------------------------------
// <copyright file="ChangingMicroHIDPickupStateEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Item
{
    using Exiled.API.Features.Pickups;

    using Interfaces;
    using InventorySystem.Items.MicroHID.Modules;
    using InventorySystem.Items.Pickups;

    /// <summary>
    /// Contains all information before MicroHID pickup state is changed.
    /// </summary>
    public class ChangingMicroHIDPickupStateEventArgs : IDeniableEvent, IPickupEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingMicroHIDPickupStateEventArgs" /> class.
        /// </summary>
        /// <param name="microHID">
        /// <inheritdoc cref="MicroHID" />
        /// </param>
        /// <param name="newPhase">
        /// <inheritdoc cref="NewPhase" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public ChangingMicroHIDPickupStateEventArgs(ItemPickupBase microHID, MicroHidPhase newPhase, bool isAllowed = true)
        {
            MicroHID = Pickup.Get<MicroHIDPickup>(microHID);
            NewPhase = newPhase;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the MicroHID instance.
        /// </summary>
        public MicroHIDPickup MicroHID { get; }

        /// <summary>
        /// Gets or sets the new MicroHID state.
        /// </summary>
        public MicroHidPhase NewPhase { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the MicroHID state can be changed.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <inheritdoc/>
        public Pickup Pickup => MicroHID;
    }
}