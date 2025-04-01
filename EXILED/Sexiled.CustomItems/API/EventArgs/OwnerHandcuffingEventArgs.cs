// -----------------------------------------------------------------------
// <copyright file="OwnerHandcuffingEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Sexiled.CustomItems.API.EventArgs
{
    using Sexiled.API.Features;
    using Sexiled.API.Features.Items;
    using Sexiled.CustomItems.API.Features;
    using Sexiled.Events.EventArgs.Player;

    /// <summary>
    /// Contains all information of a <see cref="CustomItem"/> before handcuffing a <see cref="Player"/>.
    /// </summary>
    public class OwnerHandcuffingEventArgs : HandcuffingEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerHandcuffingEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="ev">The <see cref="HandcuffingEventArgs"/> instance.</param>
        public OwnerHandcuffingEventArgs(Item item, HandcuffingEventArgs ev)
            : this(item, ev.Player, ev.Target, ev.IsAllowed)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerHandcuffingEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="cuffer"><inheritdoc cref="HandcuffingEventArgs.Player"/></param>
        /// <param name="target"><inheritdoc cref="HandcuffingEventArgs.Target"/></param>
        /// <param name="isAllowed"><inheritdoc cref="HandcuffingEventArgs.IsAllowed"/></param>
        public OwnerHandcuffingEventArgs(Item item, Player cuffer, Player target, bool isAllowed = true)
            : base(cuffer, target, isAllowed)
        {
            Item = item;
        }

        /// <summary>
        /// Gets the item in the player's inventory.
        /// </summary>
        public Item Item { get; }
    }
}