// -----------------------------------------------------------------------
// <copyright file="DroppingScp330EventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp330
{
    using API.Features;
    using API.Features.Items;

    using Interfaces;

    using InventorySystem.Items.Usables.Scp330;

    /// <summary>
    /// Contains all information before a player drops a SCP-330 candy.
    /// </summary>
    public class DroppingScp330EventArgs : IPlayerEvent, IScp330Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DroppingScp330EventArgs" /> class.
        /// </summary>
        /// <param name="scp330">
        /// <inheritdoc cref="Scp330" />
        /// </param>
        /// <param name="candy">
        /// <inheritdoc cref="Candy" />
        /// </param>
        public DroppingScp330EventArgs(Scp330Bag scp330, CandyKindID candy)
        {
            Scp330 = Item.Get<Scp330>(scp330);
            Player = Scp330.Owner;
            Candy = candy;
        }

        /// <summary>
        /// Gets a value representing the <see cref="API.Features.Items.Item" /> being picked up.
        /// </summary>
        public Scp330 Scp330 { get; }

        /// <inheritdoc/>
        public Item Item => Scp330;

        /// <summary>
        /// Gets or sets a value indicating whether the type of candy drop.
        /// </summary>
        public CandyKindID Candy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player can interact with SCP-330.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        /// Gets the player who's interacting with SCP-330.
        /// </summary>
        public Player Player { get; }
    }
}