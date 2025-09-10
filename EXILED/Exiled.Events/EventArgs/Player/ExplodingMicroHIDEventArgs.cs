// -----------------------------------------------------------------------
// <copyright file="ExplodingMicroHIDEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;
    using InventorySystem.Items.MicroHID;

    /// <summary>
    /// Contains all information before the micro hid explode.
    /// </summary>
    public class ExplodingMicroHIDEventArgs : IDeniableEvent, IMicroHIDEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExplodingMicroHIDEventArgs" /> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="isAllowed">Whether the Micro HID can explode the player or not.</param>
        public ExplodingMicroHIDEventArgs(MicroHIDItem item, bool isAllowed = true)
        {
            MicroHID = Item.Get<MicroHid>(item);
            Player = MicroHID.Owner;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        public Item Item => MicroHID;

        /// <summary>
        /// Gets the player in owner of the item.
        /// </summary>
        public Exiled.API.Features.Player Player { get; }

        /// <summary>
        /// Gets MicroHid item.
        /// </summary>
        public MicroHid MicroHID { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the player can explode the micro HID.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}