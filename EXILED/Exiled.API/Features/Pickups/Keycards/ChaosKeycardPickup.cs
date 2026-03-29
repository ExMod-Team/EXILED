// -----------------------------------------------------------------------
// <copyright file="ChaosKeycardPickup.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups.Keycards
{
    using InventorySystem.Items.Keycards;
    using InventorySystem.Items.Keycards.Snake;
    using InventorySystem.Items.Pickups;

    /// <summary>
    /// A base class for all keycard pickups.
    /// </summary>
    public class ChaosKeycardPickup : Pickups.KeycardPickup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChaosKeycardPickup"/> class.
        /// </summary>
        /// <param name="pickupBase">The <see cref="ItemPickupBase"/> to encapsulate.</param>
        internal ChaosKeycardPickup(KeycardPickup pickupBase)
            : base(pickupBase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChaosKeycardPickup"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup to create.</param>
        internal ChaosKeycardPickup(ItemType type)
            : base(type)
        {
        }

        /// <summary>
        /// Gets the <see cref="InventorySystem.Items.Keycards.Snake.SnakeEngine"/> this encapsulates.
        /// </summary>
        public SnakeEngine SnakeEngine => ChaosKeycardItem.GetEngineForSerial(Serial);

        /// <summary>
        /// Returns the Keycard in a human readable format.
        /// </summary>
        /// <returns>A string containing Keycard-related data.</returns>
        public override string ToString() => $"{Type} == ({Serial}) [{Weight}] *{Scale}* |{Permissions}|";
    }
}