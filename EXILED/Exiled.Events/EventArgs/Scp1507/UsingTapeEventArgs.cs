// -----------------------------------------------------------------------
// <copyright file="UsingTapeEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp1507
{
    using System;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;
    using InventorySystem.Items;

    /// <summary>
    /// Contains all information before SCP-1507 screams.
    /// </summary>
    // [Obsolete("Only availaible for Christmas and AprilFools.")]
    public class UsingTapeEventArgs : IPlayerEvent, IItemEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsingTapeEventArgs"/> class.
        /// </summary>
        /// <param name="itemBase"><inheritdoc cref="Item"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public UsingTapeEventArgs(ItemBase itemBase, bool isAllowed = true)
        {
            Item = Item.Get(itemBase);
            Player = Item.Owner;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <inheritdoc/>
        public Item Item { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }
    }
}