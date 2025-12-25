// -----------------------------------------------------------------------
// <copyright file="PunchingEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Item
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;
    using InventorySystem.Items.MarshmallowMan;

    /// <summary>
    /// Contains all information before a marshmallow man punches.
    /// </summary>
    public class PunchingEventArgs : IMarshmallowEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PunchingEventArgs"/> class.
        /// </summary>
        /// <param name="marshmallow">The marshmallow item of the player attacking.</param>
        /// <param name="isAllowed">Whether the player is allowed to punch.</param>
        public PunchingEventArgs(MarshmallowItem marshmallow, bool isAllowed = true)
        {
            Marshmallow = Item.Get<Marshmallow>(marshmallow);
            Player = Marshmallow.Owner;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player punching.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc />
        public Item Item => Marshmallow;

        /// <summary>
        /// Gets the marshmallow item of the player punching.
        /// </summary>
        public Marshmallow Marshmallow { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the punch is allowed.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}