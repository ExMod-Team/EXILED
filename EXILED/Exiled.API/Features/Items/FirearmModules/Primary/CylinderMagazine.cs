// -----------------------------------------------------------------------
// <copyright file="CylinderMagazine.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items.FirearmModules.Primary
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;

    using InventorySystem.Items.Firearms.Modules;

    /// <summary>
    /// Basic realization of <see cref="CylinderAmmoModule"/>.
    /// </summary>
    public class CylinderMagazine : PrimaryMagazine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CylinderMagazine"/> class.
        /// </summary>
        /// <param name="magazine">target <see cref="CylinderAmmoModule"/>.</param>
        public CylinderMagazine(CylinderAmmoModule magazine)
            : base(magazine)
        {
            CylinderModule = magazine;
        }

        /// <summary>
        /// Gets an original <see cref="IPrimaryAmmoContainerModule"/>.
        /// </summary>
        public CylinderAmmoModule CylinderModule { get; }

        /// <inheritdoc/>
        public override Firearm Firearm => Item.Get<Firearm>(CylinderModule.Firearm);

        /// <inheritdoc/>
        public override int MaxAmmo
        {
            set
            {
                CylinderModule._defaultCapacity = value;
                Resync();
            }
        }

        /// <inheritdoc/>
        public override int ConstantMaxAmmo => CylinderModule._defaultCapacity;

        /// <summary>
        /// Gets or sets an used <see cref="Exiled.API.Enums.AmmoType"/> for this magazine.
        /// </summary>
        public override AmmoType AmmoType
        {
            get => Magazine.AmmoType.GetAmmoType();
            set => CylinderModule.AmmoType = value.GetItemType();
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of chambers in cylindric magazine.
        /// </summary>
        public IEnumerable<Chamber> Chambers => CylinderAmmoModule.GetChambersArrayForSerial(CylinderModule.ItemSerial, MaxAmmo).Select(baseChamber => new Chamber(baseChamber));

        /// <inheritdoc/>
        public override void Resync() => CylinderModule._needsResyncing = true;

        /// <summary>
        /// Rotates cylindric magazine by fixed rotatins.
        /// </summary>
        /// <param name="rotations">Rotations count.</param>
        public void Rotate(int rotations) => CylinderModule.RotateCylinder(rotations);

        /// <summary>
        /// A basic wrapper for chamber in cylinder magazine.
        /// </summary>
        public class Chamber
        {
            private CylinderAmmoModule.Chamber baseChamber;

            /// <summary>
            /// Initializes a new instance of the <see cref="Chamber"/> class.
            /// </summary>
            /// <param name="baseChamber">Basic <see cref="CylinderAmmoModule.Chamber"/> class.</param>
            internal Chamber(CylinderAmmoModule.Chamber baseChamber)
            {
                this.baseChamber = baseChamber;
            }

            /// <summary>
            /// Gets or sets an state for current chamber.
            /// </summary>
            public RevolverChamberState State
            {
                get => (RevolverChamberState)baseChamber.ContextState;
                set => baseChamber.ContextState = (CylinderAmmoModule.ChamberState)value;
            }
        }
    }
}
