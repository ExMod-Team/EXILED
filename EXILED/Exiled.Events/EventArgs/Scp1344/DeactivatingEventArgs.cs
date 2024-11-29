// -----------------------------------------------------------------------
// <copyright file="DeactivatingEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp1344
{
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before deactivating.
    /// </summary>
    public class DeactivatingEventArgs : IPlayerEvent, IScp1344Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeactivatingEventArgs" /> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public DeactivatingEventArgs(Item item, bool isAllowed = true)
        {
            Item = item;
            Scp1344 = item as Scp1344;
            Player = item.Owner;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        public Item Item { get; }

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
