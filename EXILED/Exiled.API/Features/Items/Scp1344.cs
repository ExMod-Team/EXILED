// -----------------------------------------------------------------------
// <copyright file="Scp1344.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using Exiled.API.Interfaces;
    using InventorySystem.Items.Usables.Scp1344;

    /// <summary>
    /// A wrapper class for SCP-1344.
    /// </summary>
    public class Scp1344 : Usable, IWrapper<Scp1344Item>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp1344"/> class.
        /// </summary>
        /// <param name="itemBase">The item base to wrap.</param>
        public Scp1344(Scp1344Item itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp1344"/> class.
        /// </summary>
        internal Scp1344()
            : this((Scp1344Item)Server.Host.Inventory.CreateItemInstance(new(ItemType.SCP1344, 0), false))
        {
        }

        /// <summary>
        /// Gets the <see cref="Scp1344"/> base.
        /// </summary>
        public new Scp1344Item Base { get; }

        /// <summary>
        /// Gets a value indicating whether the item is worn.
        /// </summary>
        public bool IsWorn => Base.IsWorn;

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public Scp1344Status Status
        {
            get => Base.Status;
            set => Base.Status = value;
        }

        /// <summary>
        /// Gets a value indicating whether it can be started to use.
        /// </summary>
        public bool CanStartUsing => Base.CanStartUsing;
    }
}