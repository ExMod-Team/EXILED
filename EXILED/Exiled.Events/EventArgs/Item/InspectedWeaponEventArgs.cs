// -----------------------------------------------------------------------
// <copyright file="InspectedWeaponEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Item
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;
    using InventorySystem.Items;

    /// <summary>
    /// Contains all information before weapon is inspected.
    /// </summary>
    public class InspectedWeaponEventArgs : IItemEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InspectedWeaponEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        public InspectedWeaponEventArgs(ItemBase item)
        {
            Firearm = Item.Get<Firearm>(item);
        }

        /// <inheritdoc/>
        public Player Player => Firearm.Owner;

        /// <inheritdoc/>
        public Item Item => Firearm;

        /// <summary>
        /// Gets the firearm that is being inspected.
        /// </summary>
        public Firearm Firearm { get; }
    }
}