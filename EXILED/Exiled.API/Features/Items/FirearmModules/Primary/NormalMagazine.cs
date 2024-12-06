// -----------------------------------------------------------------------
// <copyright file="NormalMagazine.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items.FirearmModules.Primary
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;

    using InventorySystem.Items.Firearms.Modules;

    /// <summary>
    /// Basic realization of <see cref="InventorySystem.Items.Firearms.Modules.MagazineModule"/>.
    /// </summary>
    public class NormalMagazine : PrimaryMagazine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NormalMagazine"/> class.
        /// </summary>
        /// <param name="magazine">target <see cref="IPrimaryAmmoContainerModule"/>.</param>
        public NormalMagazine(MagazineModule magazine)
            : base(magazine)
        {
            MagazineModule = magazine;
            Firearm = (Firearm)Item.Get(MagazineModule.Firearm);
        }

        /// <summary>
        /// Gets an original <see cref="MagazineModule"/>.
        /// </summary>
        public MagazineModule MagazineModule { get; }

        /// <inheritdoc/>
        public override Firearm Firearm { get; }

        /// <inheritdoc/>
        public override int MaxAmmo
        {
            set
            {
                MagazineModule._defaultCapacity = value;
            }
        }

        /// <inheritdoc/>
        public override AmmoType AmmoType
        {
            get
            {
                return Magazine.AmmoType.GetAmmoType();
            }

            set
            {
                MagazineModule._ammoType = value.GetItemType();
            }
        }

        /// <inheritdoc/>
        public override bool MagazineInserted
        {
            get
            {
                return MagazineModule.MagazineInserted;
            }

            set
            {
                MagazineModule.MagazineInserted = value;
                Resync();
            }
        }

        /// <summary>
        /// Removes magazine from current <see cref="Exiled.API.Features.Items.Firearm"/>.
        /// </summary>
        /// <remarks>
        /// Affects on actual ammo count.
        /// Removes all ammo from magazine.
        /// </remarks>
        public void RemoveMagazine()
        {
            MagazineModule.ServerRemoveMagazine();
        }

        /// <summary>
        /// Inserts current magazine from current <see cref="Exiled.API.Features.Items.Firearm"/>.
        /// </summary>
        public void InsertMagazine()
        {
            MagazineModule.ServerInsertEmptyMagazine();
        }

        /// <inheritdoc/>
        public override void Resync()
        {
            MagazineModule.ServerResyncData();
        }
    }
}
