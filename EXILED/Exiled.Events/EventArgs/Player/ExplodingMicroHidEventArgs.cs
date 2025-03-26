// -----------------------------------------------------------------------
// <copyright file="ExplodingMicroHidEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before the micro hid explode.
    /// </summary>
    public class ExplodingMicroHidEventArgs : IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExplodingMicroHidEventArgs" /> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        public ExplodingMicroHidEventArgs(Item item)
        {
            Item = item;
            MicroHid = item as MicroHid;
            Player = item.Owner;
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
        public MicroHid MicroHid { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the player can explode the micro HID.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}