// -----------------------------------------------------------------------
// <copyright file="UpgradedPickupEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp914
{
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Interfaces;
    using global::Scp914;
    using InventorySystem.Items.Pickups;
    using UnityEngine;

    /// <summary>
    /// Contains all information before SCP-914 upgrades an item.
    /// </summary>
    public class UpgradedPickupEventArgs : IPickupEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpgradedPickupEventArgs" /> class.
        /// </summary>
        /// <param name="item">
        /// <inheritdoc cref="Pickup" />
        /// </param>
        /// <param name="newPos">
        /// <inheritdoc cref="OutputPosition" />
        /// </param>
        /// <param name="knobSetting">
        /// <inheritdoc cref="KnobSetting" />
        /// </param>
        /// <param name="result">
        /// <inheritdoc cref="Result" />
        /// </param>
        public UpgradedPickupEventArgs(ItemPickupBase item, Vector3 newPos, Scp914KnobSetting knobSetting, ItemPickupBase[] result)
        {
            Pickup = Pickup.Get(item);
            OutputPosition = newPos;
            KnobSetting = knobSetting;
            Result = result;
        }

        /// <summary>
        /// Gets a list of items to be upgraded inside SCP-914.
        /// </summary>
        public Pickup Pickup { get; }

        /// <summary>
        /// Gets the position the item will be output to.
        /// </summary>
        public Vector3 OutputPosition { get; }

        /// <summary>
        /// Gets SCP-914 working knob setting.
        /// </summary>
        public Scp914KnobSetting KnobSetting { get; }

        /// <summary>
        /// Gets the array of items created as a result of SCP-914 upgraded.
        /// </summary>
        public ItemPickupBase[] Result { get; }
    }
}