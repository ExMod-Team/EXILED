// -----------------------------------------------------------------------
// <copyright file="PumpBarrelMagazine.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items.FirearmModules.Barrel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using InventorySystem.Items.Firearms.Modules;

    using UnityEngine;

    /// <summary>
    /// Basic realization of <see cref="PumpActionModule"/> barrel.
    /// </summary>
    public class PumpBarrelMagazine : BarrelMagazine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PumpBarrelMagazine"/> class.
        /// </summary>
        /// <param name="pumpModule">Target <see cref="PumpActionModule"/>.</param>
        public PumpBarrelMagazine(PumpActionModule pumpModule)
            : base(pumpModule)
        {
            PumpBarrel = pumpModule;
            Firearm = (Firearm)Item.Get(PumpBarrel.Firearm);
        }

        /// <summary>
        /// Gets an original <see cref="IAmmoContainerModule"/>.
        /// </summary>
        public PumpActionModule PumpBarrel { get; }

        /// <inheritdoc/>
        public override Firearm Firearm { get; }

        /// <inheritdoc/>
        public override int Ammo
        {
            get
            {
                return PumpBarrel.SyncChambered;
            }

            set
            {
                PumpBarrel.SyncChambered = Mathf.Max(value, 0);

                Resync();
            }
        }

        /// <summary>
        /// Gets or sets an amount of bullets, that pump module will try to shot.
        /// </summary>
        public int CockedAmmo
        {
            get
            {
                return PumpBarrel.SyncCocked;
            }

            set
            {
                PumpBarrel.SyncCocked = Mathf.Max(value, 0);

                Resync();
            }
        }

        /// <inheritdoc/>
        public override int MaxAmmo
        {
            get
            {
                return PumpBarrel._numberOfBarrels;
            }

            set
            {
                PumpBarrel._numberOfBarrels = Mathf.Max(value, 0);
            }
        }

        /// <inheritdoc/>
        public override bool IsCocked
        {
            get
            {
                return PumpBarrel.SyncCocked > 0;
            }

            set
            {
                PumpBarrel.SyncCocked = MaxAmmo;
                Resync();
            }
        }

        /// <inheritdoc/>
        public override void Resync()
        {
            PumpBarrel.ServerResync();
        }
    }
}
