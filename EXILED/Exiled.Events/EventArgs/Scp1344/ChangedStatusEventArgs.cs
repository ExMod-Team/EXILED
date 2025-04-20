// -----------------------------------------------------------------------
// <copyright file="ChangedStatusEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp1344
{
    using System;

    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;

    using InventorySystem.Items;
    using InventorySystem.Items.Usables.Scp1344;

    /// <summary>
    /// Contains all information after SCP-1344 status changing.
    /// </summary>
    public class ChangedStatusEventArgs : IScp1344Event, IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangedStatusEventArgs" /> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="scp1344Status"><inheritdoc cref="InventorySystem.Items.Usables.Scp1344.Scp1344Status"/></param>
        public ChangedStatusEventArgs(ItemBase item, Scp1344Status scp1344Status)
        {
            Scp1344 = Item.Get<Scp1344>(item);
            Player = Scp1344.Owner;
            Scp1344Status = scp1344Status;
        }

        /// <summary>
        /// Gets the new state.
        /// </summary>
        public Scp1344Status Scp1344Status { get; }

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
        [Obsolete("Please use ChangingStatusEventArgs::IsAllowed instead of this", true)]
        public bool IsAllowed { get; set; }
    }
}
