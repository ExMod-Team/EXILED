// -----------------------------------------------------------------------
// <copyright file="ChangingStatusEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp1344
{
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;
    using InventorySystem.Items;
    using InventorySystem.Items.Usables.Scp1344;

    /// <summary>
    /// Contains all information before SCP-1344 status changing.
    /// </summary>
    public class ChangingStatusEventArgs : IPlayerEvent, IScp1344Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingStatusEventArgs" /> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="scp1344StatusNew"><inheritdoc cref="Scp1344StatusNew"/></param>
        /// <param name="scp1344StatusOld"><inheritdoc cref="Scp1344StatusOld"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ChangingStatusEventArgs(ItemBase item, Scp1344Status scp1344StatusNew, Scp1344Status scp1344StatusOld, bool isAllowed = true)
        {
            Scp1344 = Item.Get<Scp1344>(item);
            Player = Scp1344.Owner;
            Scp1344StatusNew = scp1344StatusNew;
            Scp1344StatusOld = scp1344StatusOld;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the current status.
        /// </summary>
        public Scp1344Status Scp1344StatusOld { get; }

        /// <summary>
        /// Gets or sets the new state.
        /// </summary>
        public Scp1344Status Scp1344StatusNew { get; set; }

        /// <summary>
        /// Gets the item.
        /// </summary>
        public Item Item => Scp1344;

        /// <summary>
        /// Gets the player in owner of the item.
        /// </summary>
        public Exiled.API.Features.Player Player { get; }

        /// <summary>
        /// Gets Scp1344 item.
        /// </summary>
        public Scp1344 Scp1344 { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }
    }
}
