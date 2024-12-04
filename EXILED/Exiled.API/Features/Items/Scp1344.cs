// -----------------------------------------------------------------------
// <copyright file="Scp1344.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using Exiled.API.Interfaces;
    using InventorySystem.Items.Usables;
    using InventorySystem.Items.Usables.Scp1344;
    using PlayerRoles.FirstPersonControl.Thirdperson.Subcontrollers;

    /// <summary>
    /// A wrapper class for <see cref="Scp1344Item"/>.
    /// </summary>
    public class Scp1344 : Usable, IWrapper<Scp1344Item>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp1344"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="Scp1344Item"/> class.</param>
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
        /// Gets the <see cref="UsableItem"/> that this class is encapsulating.
        /// </summary>
        public new Scp1344Item Base { get; }

        /// <summary>
        /// Gets a value indicating whether it can be started to use.
        /// </summary>
        public bool CanStartUsing => Base.CanStartUsing;

        /// <summary>
        /// Gets or sets the status of Scp1344.
        /// </summary>
        public Scp1344Status Status
        {
            get => Base.Status;
            set => Base.Status = value;
        }

        /// <summary>
        /// Forcefully Deactivate SCP-1344.
        /// </summary>
        /// <param name="dropItem">Drop or not the item.</param>
        public void Deactivate(bool dropItem = false)
        {
            if (Status is not(Scp1344Status.Active or Scp1344Status.Stabbing or Scp1344Status.Dropping))
            {
                return;
            }

            Base.Owner.DisableWearables(WearableElements.Scp1344Goggles);
            Base.ActivateFinalEffects();
            Status = Scp1344Status.Idle;

            if (dropItem)
            {
                Base.ServerDropItem(true);
            }
        }

        /// <summary>
        /// Forcefully activated SCP-1344.
        /// </summary>
        public void Actived() => Status = Scp1344Status.Stabbing;
    }
}
