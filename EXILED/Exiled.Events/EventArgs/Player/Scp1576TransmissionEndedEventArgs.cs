// -----------------------------------------------------------------------
// <copyright file="Scp1576TransmissionEndedEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using API.Features.Items;
    using Interfaces;
    using InventorySystem.Items.Usables.Scp1576;

    /// <summary>
    /// Contains all information after a SCP-1576 transmission has ended.
    /// </summary>
    public class Scp1576TransmissionEndedEventArgs : IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp1576TransmissionEndedEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="scp1576Item"><inheritdoc cref="Scp1576"/></param>
        public Scp1576TransmissionEndedEventArgs(Player player, Scp1576Item scp1576Item)
        {
            Player = player;
            Scp1576 = Item.Get<Scp1576>(scp1576Item);
        }

        /// <summary>
        /// Gets the <see cref="Exiled.API.Features.Player"/> that the transmission ended for.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="Exiled.API.Features.Items.Scp1576"/> instance.
        /// </summary>
        public Exiled.API.Features.Items.Scp1576 Scp1576 { get; }
    }
}