// -----------------------------------------------------------------------
// <copyright file="KeycardPickup.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups.Keycards
{
    using Exiled.API.Extensions;

    using InventorySystem.Items.Pickups;

    /// <summary>
    /// A base class for all keycard pickups.
    /// </summary>
    public class KeycardPickup : Pickup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeycardPickup"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="ItemPickupBase"/> class.</param>
        public KeycardPickup(ItemPickupBase pickupBase)
            : base(pickupBase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeycardPickup"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup.</param>
        internal KeycardPickup(ItemType type)
            : base(type)
        {
        }

        /// <summary>
        /// Resyncs all properties of the keycard.
        /// Gets called by all setters by default.
        /// </summary>
        public void Resync()
        {
            MirrorExtensions.ResyncKeycardPickup(this);
        }
    }
}