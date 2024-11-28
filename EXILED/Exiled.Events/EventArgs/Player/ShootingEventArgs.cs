// -----------------------------------------------------------------------
// <copyright file="ShootingEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Exiled.API.Features.Items;
    using Interfaces;
    using InventorySystem.Items.Firearms.Modules.Misc;

    using BaseFirearm = InventorySystem.Items.Firearms.Firearm;

    /// <summary>
    /// Contains all information before a player fires a weapon.
    /// </summary>
    public class ShootingEventArgs : IPlayerEvent, IDeniableEvent, IFirearmEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShootingEventArgs" /> class.
        /// </summary>
        /// <param name="firearm">
        /// The <see cref="BaseFirearm"/> that is being fired.
        /// </param>
        /// <param name="shotBacktrackData">
        /// <see cref="ShotBacktrackData"/> sent by the client.
        /// </param>
        public ShootingEventArgs(BaseFirearm firearm, ShotBacktrackData shotBacktrackData)
        {
            Firearm = (Firearm)Item.Get(firearm);
            Player = Firearm.Owner;
            ShotBacktrackData = shotBacktrackData;
        }

        /// <summary>
        /// Gets the player who is shooting.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the target that client claims it hit. This value is controlled by the client and should not be trusted. Can be null.
        /// </summary>
        public Player ClaimedTarget => ShotBacktrackData.HasPrimaryTarget ? Player.Get(ShotBacktrackData.PrimaryTargetHub) : null;

        /// <summary>
        /// Gets the <see cref="ShotBacktrackData" />. This object contains the data sent by the client to the server. Values should not be trusted.
        /// </summary>
        public ShotBacktrackData ShotBacktrackData { get; }

        /// <summary>
        /// Gets the target <see cref="API.Features.Items.Firearm" />.
        /// </summary>
        public Firearm Firearm { get; }

        /// <inheritdoc />
        public Item Item => Firearm;

        /// <summary>
        /// Gets or sets a value indicating whether the shot can be fired.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}