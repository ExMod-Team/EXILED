// -----------------------------------------------------------------------
// <copyright file="RotatingRevolverEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using API.Features.Items;

    using Exiled.API.Features.Items.FirearmModules.Primary;

    using Interfaces;

    using InventorySystem.Items.Firearms.Modules;

    using FirearmBase = InventorySystem.Items.Firearms.Firearm;

    /// <summary>
    /// Contains all information when a player rotates revolver.
    /// </summary>
    public class RotatingRevolverEventArgs : IPlayerEvent, IFirearmEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RotatingRevolverEventArgs" /> class.
        /// </summary>
        /// <param name="firearm">
        /// <inheritdoc cref="Firearm" />
        /// </param>
        /// <param name="rotations">
        /// <inheritdoc cref="Rotations" />
        /// </param>
        public RotatingRevolverEventArgs(FirearmBase firearm, int rotations)
        {
            Firearm = Item.Get<Firearm>(firearm);
            Player = Firearm.Owner;
            Rotations = rotations;
        }

        /// <summary>
        /// Gets or sets a rotations count(per chamber).
        /// </summary>
        public int Rotations { get; set; }

        /// <summary>
        /// Gets a value indicating whether the rotation will have an effect.
        /// </summary>
        /// <remarks>
        /// checks rotations and chambers counts equality by mod of chambers counts. <code>Rotations % Chambers.Length == 0</code>
        /// </remarks>
        public bool HasEffect => Firearm.PrimaryMagazine.MaxAmmo % Rotations == 0;

        /// <inheritdoc/>
        public Firearm Firearm { get; }

        /// <inheritdoc/>
        public Item Item => Firearm;

        /// <inheritdoc/>
        public Player Player { get; }
    }
}