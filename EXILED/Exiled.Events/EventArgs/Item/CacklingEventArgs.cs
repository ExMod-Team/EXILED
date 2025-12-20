// -----------------------------------------------------------------------
// <copyright file="CacklingEventArgs.cs" company="ExMod Team">
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
    public class CacklingEventArgs : IMarshmallowEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacklingEventArgs"/> class.
        /// </summary>
        /// <param name="player">The Player cackling.</param>
        /// <param name="marshmallow">The marshmallow item of the player cackling.</param>
        /// <param name="isAllowed">Whether the player is allowed to cackle.</param>
        public CacklingEventArgs(Player player, MarshmallowItem marshmallow, bool isAllowed = true)
        {
            Player = player;
            Marshmallow = Item.Get<Marshmallow>(marshmallow);
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player cackling.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc />
        public API.Features.Items.Item Item => Marshmallow;

        /// <summary>
        /// Gets the marshmallow item of the player cackling.
        /// </summary>
        public Marshmallow Marshmallow { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the player is allowed to cackle.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}