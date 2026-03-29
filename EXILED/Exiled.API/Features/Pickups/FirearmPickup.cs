// -----------------------------------------------------------------------
// <copyright file="FirearmPickup.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using System;

    using Exiled.API.Interfaces;
    using InventorySystem.Items;
    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.Attachments;
    using InventorySystem.Items.Firearms.Modules;

    using BaseFirearm = InventorySystem.Items.Firearms.FirearmPickup;

    /// <summary>
    /// A wrapper class for a Firearm pickup.
    /// </summary>
    public class FirearmPickup : Pickup, IWrapper<BaseFirearm>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirearmPickup"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="BaseFirearm"/> class.</param>
        internal FirearmPickup(BaseFirearm pickupBase)
            : base(pickupBase)
        {
            Base = pickupBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirearmPickup"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup.</param>
        internal FirearmPickup(ItemType type)
            : base(type)
        {
            Base = (BaseFirearm)((Pickup)this).Base;
            Ammo = MaxAmmo;
        }

        /// <summary>
        /// Gets the <see cref="BaseFirearm"/> that this class is encapsulating.
        /// </summary>
        public new BaseFirearm Base { get; }

        /// <summary>
        /// Gets a value indicating whether the pickup is already distributed.
        /// </summary>
        [Obsolete("Feature deprecated")]
        public bool IsDistributed { get; }

        /// <summary>
        /// Gets or sets a value indicating how much ammo can contain this <see cref="FirearmPickup"/>.
        /// </summary>
        public int MaxAmmo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how much ammo have this <see cref="FirearmPickup"/>.
        /// </summary>
        public int Ammo
        {
            get
            {
                if (!AttachmentPreview.TryGetOrAddInstance(Type, out Firearm baseFirearm))
                    return 0;

                Items.Firearm firearm = Items.Item.Get<Items.Firearm>(baseFirearm);

                ushort oldSerial = firearm.Serial;

                firearm.Serial = Serial;

                int ammo = firearm.PrimaryMagazine.Ammo;

                firearm.Serial = oldSerial;

                return ammo;
            }

            set
            {
                if (!AttachmentPreview.TryGetOrAddInstance(Type, out Firearm baseFirearm))
                    return;

                Items.Firearm firearm = Items.Item.Get<Items.Firearm>(baseFirearm);

                ushort oldSerial = firearm.Serial;

                firearm.Serial = Serial;

                firearm.PrimaryMagazine.Ammo = value;

                firearm.Serial = oldSerial;
            }
        }

        /// <summary>
        /// Gets or sets the ammo drain per shoot.
        /// </summary>
        /// <remarks>
        /// Always <see langword="1"/> by default.
        /// Applied on a high layer nether basegame ammo controllers.
        /// </remarks>
        public int AmmoDrain { get; set; } = 1;

        /// <summary>
        /// Gets or sets a value indicating whether the attachment code have this <see cref="FirearmPickup"/>.
        /// </summary>
        public uint Attachments
        {
            get => Base.Worldmodel.AttachmentCode;
            set => Base.Worldmodel.Setup(Base.CurId, Base.Worldmodel.WorldmodelType, value);
        }

        /// <summary>
        /// Initializes the item as if it was spawned naturally by map generation.
        /// </summary>
        public void Distribute() => Base.OnDistributed();

        /// <summary>
        /// Returns the FirearmPickup in a human-readable format.
        /// </summary>
        /// <returns>A string containing FirearmPickup related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}*";

        /// <inheritdoc/>
        internal override void ReadItemInfo(Items.Item item)
        {
            if (item is Items.Firearm firearm)
            {
                MaxAmmo = firearm.PrimaryMagazine.ConstantMaxAmmo;
                AmmoDrain = firearm.AmmoDrain;
            }

            base.ReadItemInfo(item);
        }

        /// <inheritdoc/>
        protected override void InitializeProperties(ItemBase itemBase)
        {
            base.InitializeProperties(itemBase);
            if (!(itemBase as Firearm).TryGetModule(out IPrimaryAmmoContainerModule magazine))
            {
                Log.Error($"firearm prefab {itemBase.ItemTypeId} doesnt have an primary magazine module(unexpected)");
                return;
            }

            MaxAmmo = magazine.AmmoMax;
        }
    }
}
