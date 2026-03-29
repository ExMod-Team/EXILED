// -----------------------------------------------------------------------
// <copyright file="InspectingItemEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Item
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;
    using InventorySystem.Items;

    /// <summary>
    /// Contains all information before weapon is inspected.
    /// </summary>
    public class InspectingItemEventArgs : IItemEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InspectingItemEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public InspectingItemEventArgs(ItemBase item, bool isAllowed = true)
        {
            Item = Item.Get(item);
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public Player Player => Item.Owner;

        /// <inheritdoc/>
        public Item Item { get; }

        /// <inheritdoc/>
        /// <remarks>Setter will not work if inspected <see cref="Item"/> is a <see cref="Firearm"/> or a <see cref="Keycard"/>.</remarks>
        public bool IsAllowed { get; set; }
    }
}