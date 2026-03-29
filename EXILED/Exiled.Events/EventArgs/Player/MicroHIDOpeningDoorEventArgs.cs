// -----------------------------------------------------------------------
// <copyright file="MicroHIDOpeningDoorEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features.Doors;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;

    using Interactables.Interobjects.DoorUtils;
    using InventorySystem.Items.MicroHID;

    /// <summary>
    /// Contains all information before the micro opens a doors.
    /// </summary>
    public class MicroHIDOpeningDoorEventArgs : IDeniableEvent, IDoorEvent, IMicroHIDEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroHIDOpeningDoorEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="isAllowed">Whether the Micro HID can open the door or not.</param>
        /// <param name="door"><inheritdoc cref="Door"/></param>
        public MicroHIDOpeningDoorEventArgs(MicroHIDItem item, DoorVariant door, bool isAllowed = true)
        {
            MicroHID = Item.Get<MicroHid>(item);
            Player = MicroHID.Owner;
            IsAllowed = !isAllowed;
            Door = Door.Get(door);
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
        /// Gets the <see cref="API.Features.Doors.Door"/> instance.
        /// </summary>
        public Door Door { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the player can explode the micro HID.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}