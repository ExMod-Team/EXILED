// -----------------------------------------------------------------------
// <copyright file="InteractingScp330EventArgs.cs" company="ExMod Team">
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
    /// Contains all information before a player interacts with SCP-330.
    /// </summary>
    public class InteractingScp330EventArgs : IPlayerEvent, IScp330Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InteractingScp330EventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="usage">
        /// <inheritdoc cref="UsageCount" />
        /// </param>
        public InteractingScp330EventArgs(Player player, int usage)
        {
            Player = player;
            UsageCount = usage;
            ShouldSever = usage >= 2;
            ShouldPlaySound = true;
            IsAllowed = Player.IsHuman;
            Candy = Scp330Candies.GetRandom();

            if (Scp330Bag.TryGetBag(player.ReferenceHub, out Scp330Bag scp330Bag))
            {
                Scp330 = (Scp330)Item.Get(scp330Bag);
            }
            else
            {
                Scp330 = (Scp330)Item.Create(ItemType.SCP330, player);
                Scp330.RemoveAllCandy();
                player.AddItem(Scp330);
            }
        }

        /// <summary>
        /// Gets a value indicating how many times this player has interacted with SCP-330.
        /// </summary>
        public int UsageCount { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the player's hands should get severed.
        /// </summary>
        public bool ShouldSever { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the sound should be played.
        /// </summary>
        /// <remarks>It won't work if <see cref="IsAllowed"/> = <see langword="false"/>.</remarks>
        public bool ShouldPlaySound { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the type of candy that will be received from this interaction.
        /// </summary>
        public CandyKindID Candy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player is allowed to interact with SCP-330.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the <see cref="API.Features.Player" /> triggering the event.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp330 Scp330 { get; }

        /// <inheritdoc/>
        public Item Item => Scp330;
    }
}