// -----------------------------------------------------------------------
// <copyright file="EatingScp330EventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp330
{
    using API.Features;
    using Exiled.API.Features.Items;
    using Interfaces;

    using InventorySystem.Items.Usables.Scp330;

    /// <summary>
    /// Contains all information before a player eats SCP-330.
    /// </summary>
    public class EatingScp330EventArgs : IPlayerEvent, IScp330Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EatingScp330EventArgs" /> class.
        /// </summary>
        /// <param name="player"><see cref="Player" />.</param>
        /// <param name="scp330"><inheritdoc cref="Scp330" />.</param>
        /// <param name="candy"><see cref="Candy" />.</param>
        /// <param name="isAllowed"><see cref="IsAllowed" />.</param>
        public EatingScp330EventArgs(Player player, Scp330Bag scp330, ICandy candy, bool isAllowed = true)
        {
            Player = player;
            Scp330 = (Scp330)Item.Get(scp330);
            Candy = candy;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="ICandy" /> that is being eaten by the player.
        /// </summary>
        public ICandy Candy { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the player can eat SCP-330.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the player who's eating SCP-330.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp330 Scp330 { get; }

        /// <inheritdoc/>
        public Item Item => Scp330;
    }
}