// -----------------------------------------------------------------------
// <copyright file="Consumable.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using Exiled.API.Extensions;
    using Exiled.API.Interfaces;
    using InventorySystem.Items.Usables;

    using BaseConsumable = InventorySystem.Items.Usables.Consumable;

    /// <summary>
    /// A wrapper class for <see cref="BaseConsumable"/>.
    /// </summary>
    public class Consumable : Usable, IWrapper<BaseConsumable>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Consumable"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="BaseConsumable"/> class.</param>
        public Consumable(BaseConsumable itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Consumable"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the usable item.</param>
        internal Consumable(ItemType type)
            : this((BaseConsumable)Server.Host.Inventory.CreateItemInstance(new(type, 0), false))
        {
        }

        /// <summary>
        /// Gets the <see cref="BaseConsumable"/> that this class is encapsulating.
        /// </summary>
        public new BaseConsumable Base { get; }

        /// <inheritdoc/>
        public override void Use(Player owner = null)
        {
            Player oldOwner = Owner;
            owner ??= Owner;

            if (owner is null)
                throw new System.InvalidOperationException("The Owner of the item cannot be null.");

            Base.Owner = owner.ReferenceHub;
            Base.ActivateEffects();

            typeof(UsableItemsController).InvokeStaticEvent(nameof(UsableItemsController.ServerOnUsingCompleted), new object[] { owner.ReferenceHub, Base });

            Base.Owner = oldOwner.ReferenceHub;
        }

        /// <inheritdoc/>
        internal override void ChangeOwner(Player oldOwner, Player newOwner)
        {
            if (oldOwner != Server.Host)
                Base.OnRemoved(null);

            Base.Owner = newOwner.ReferenceHub;
        }
    }
}
