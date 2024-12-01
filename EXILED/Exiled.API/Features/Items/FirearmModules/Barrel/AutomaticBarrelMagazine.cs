// -----------------------------------------------------------------------
// <copyright file="AutomaticBarrelMagazine.cs" company="ExMod Team">
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
    /// Basic realization of <see cref="AutomaticActionModule"/> barrel.
    /// </summary>
    public class AutomaticBarrelMagazine : BarrelMagazine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutomaticBarrelMagazine"/> class.
        /// </summary>
        /// <param name="automaticModule">Target <see cref="AutomaticActionModule"/>.</param>
        public AutomaticBarrelMagazine(AutomaticActionModule automaticModule)
            : base(automaticModule)
        {
            AutomaticBarrel = automaticModule;
        }

        /// <summary>
        /// Gets an original <see cref="IAmmoContainerModule"/>.
        /// </summary>
        public AutomaticActionModule AutomaticBarrel { get; }

        /// <inheritdoc/>
        public override int Ammo
        {
            get
            {
                return AutomaticBarrel.AmmoStored;
            }

            set
            {
                AutomaticBarrel.AmmoStored = Mathf.Max(value, 0);

                Resync();
            }
        }

        /// <inheritdoc/>
        public override int MaxAmmo
        {
            get
            {
                return AutomaticBarrel.ChamberSize;
            }

            set
            {
                AutomaticBarrel.ChamberSize = Mathf.Clamp(value, 0, 16);
            }
        }

        /// <inheritdoc/>
        public override bool IsCocked
        {
            get
            {
                return AutomaticBarrel.Cocked;
            }

            set
            {
                AutomaticBarrel.Cocked = value;
                Resync();
            }
        }

        /// <summary>
        /// Gets a value indicating whether barrel magazine has open bolt or not.
        /// </summary>
        public bool IsOpenBolted =>
            AutomaticBarrel.OpenBolt;

        /// <summary>
        /// Gets or sets a value indicating whether barrel bolt is currently locked.
        /// </summary>
        public bool BoltLocked
        {
            get
            {
                return AutomaticBarrel.BoltLocked;
            }

            set
            {
                AutomaticBarrel.BoltLocked = value;
                Resync();
            }
        }

        /// <inheritdoc/>
        public override void Resync()
        {
            AutomaticBarrel.ServerResync();
        }
    }
}
