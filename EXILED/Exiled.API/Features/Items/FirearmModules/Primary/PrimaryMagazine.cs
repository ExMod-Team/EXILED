// -----------------------------------------------------------------------
// <copyright file="PrimaryMagazine.cs" company="ExMod Team">
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
    /// Basic abstraction of <see cref="IPrimaryAmmoContainerModule"/> whose are logically used to be a primary magazines.
    /// </summary>
    public abstract class PrimaryMagazine : Magazine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrimaryMagazine"/> class.
        /// </summary>
        /// <param name="magazine">target <see cref="IPrimaryAmmoContainerModule"/>.</param>
        public PrimaryMagazine(IPrimaryAmmoContainerModule magazine)
            : base(magazine)
        {
            Magazine = magazine;
        }

        /// <summary>
        /// Gets an original <see cref="IPrimaryAmmoContainerModule"/>.
        /// </summary>
        public IPrimaryAmmoContainerModule Magazine { get; }

        /// <inheritdoc/>
        public override int MaxAmmo => Magazine.AmmoMax;

        /// <summary>
        /// Gets a max avaible ammo count in magazine without attachments.
        /// </summary>
        public abstract int ConstantMaxAmmo { get; }

        /// <inheritdoc/>
        public override int Ammo
        {
            get => Magazine.AmmoStored;

            set
            {
                int modifyCount = Math.Max(0, value) - Ammo;
                Magazine.ServerModifyAmmo(modifyCount);
                Resync();
            }
        }

        /// <summary>
        /// Gets or sets an used <see cref="Exiled.API.Enums.AmmoType"/> for this magazine.
        /// </summary>
        public abstract AmmoType AmmoType { get; set; }
    }
}
