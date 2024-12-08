// -----------------------------------------------------------------------
// <copyright file="FirearmPickup.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using System;

    using Interfaces;
    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.Attachments;
    using InventorySystem.Items.Firearms.Modules;
    using UnityEngine;

    using BaseFirearm = InventorySystem.Items.Firearms.FirearmPickup;

    /// <summary>
    /// A wrapper class for a Firearm pickup.
    /// </summary>
    public class FirearmPickup : Pickup, IWrapper<BaseFirearm>
    {
        private IPrimaryAmmoContainerModule module;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirearmPickup"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="BaseFirearm"/> class.</param>
        internal FirearmPickup(BaseFirearm pickupBase)
            : base(pickupBase)
        {
            Base = pickupBase;
            module = AttachmentPreview.TryGet(Base.CurId, false, out Firearm firearm) ? firearm.TryGetModule(out CylinderAmmoModule cylinder) ? cylinder : firearm.TryGetModule(out MagazineModule magazine) ? magazine : null : null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirearmPickup"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup.</param>
        internal FirearmPickup(ItemType type)
            : base(type)
        {
            Base = (BaseFirearm)((Pickup)this).Base;
            module = AttachmentPreview.TryGet(Base.CurId, false, out Firearm firearm) ? firearm.TryGetModule(out CylinderAmmoModule cylinder) ? cylinder : firearm.TryGetModule(out MagazineModule magazine) ? magazine : null : null;
        }

        /// <summary>
        /// Gets the <see cref="BaseFirearm"/> that this class is encapsulating.
        /// </summary>
        public new BaseFirearm Base { get; }

        /// <summary>
        /// Gets or sets a value indicating how many ammo have this <see cref="FirearmPickup"/>.
        /// </summary>
        /// <remarks>This will be updated only when item will be picked up.</remarks>
        public int Ammo
        {
            get => module?.AmmoStored ?? 0;
            set
            {
                if (module is null)
                    throw new InvalidOperationException("Cannot set ammo for non-ammo using weapons.");
                module.ServerModifyAmmo(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the attachment code have this <see cref="FirearmPickup"/>.
        /// </summary>
        public uint Attachments
        {
            get => Base.Worldmodel.AttachmentCode;
            set => Base.Worldmodel.Setup(Base.CurId, Base.Worldmodel.WorldmodelType, value);
        }

        /// <summary>
        /// Gets a value indicating whether the item has been distributed.
        /// </summary>
        public bool IsDistributed { get; internal set; }

        /// <summary>
        /// Returns the FirearmPickup in a human readable format.
        /// </summary>
        /// <returns>A string containing FirearmPickup related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* -{/*Ammo*/0}-";

        /// <inheritdoc />
        public override void Spawn()
        {
            base.Spawn();
            Base.OnDistributed();
        }

        /// <inheritdoc />
        public override Pickup Spawn(Vector3 position, Quaternion? rotation = null, Player previousOwner = null)
        {
            Pickup pickup = base.Spawn(position, rotation, previousOwner);
            Base.OnDistributed();
            return pickup;
        }

        /// <inheritdoc />
        public override void UnSpawn()
        {
            base.UnSpawn();
            IsDistributed = false;
        }
    }
}
