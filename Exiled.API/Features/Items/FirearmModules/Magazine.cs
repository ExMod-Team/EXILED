// -----------------------------------------------------------------------
// <copyright file="Magazine.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items.FirearmModules
{
    using System;

    using Exiled.API.Features.Items.FirearmModules.Barrel;
    using Exiled.API.Features.Items.FirearmModules.Primary;
    using InventorySystem.Items.Firearms.Modules;
    using InventorySystem.Items.Firearms.Modules.Scp127;
    using UnityEngine;

    /// <summary>
    /// Basic abstraction of <see cref="IAmmoContainerModule"/>.
    /// </summary>
    public abstract class Magazine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Magazine"/> class.
        /// </summary>
        /// <param name="module">target <see cref="IAmmoContainerModule"/>.</param>
        public Magazine(IAmmoContainerModule module)
        {
            AmmoContainerModule = module;
        }

        /// <summary>
        /// Gets an original <see cref="IAmmoContainerModule"/>.
        /// </summary>
        public IAmmoContainerModule AmmoContainerModule { get; }

        /// <summary>
        /// Gets or sets a count of current ammo in magazine.
        /// </summary>
        public abstract int Ammo { get; set; }

        /// <summary>
        /// Gets or sets a max avaible ammo count in magazine.
        /// </summary>
        public abstract int MaxAmmo { get; set; }

        /// <summary>
        /// Gets target <see cref="Exiled.API.Features.Items.Firearm"/> assotiated with this magazine.
        /// </summary>
        public abstract Firearm Firearm { get; }

        /// <summary>
        /// Gets wrapper to an <see cref="IAmmoContainerModule"/>.
        /// </summary>
        /// <param name="module">The target <see cref="IAmmoContainerModule"/>.</param>
        /// <returns>The wrapper for the given <see cref="IAmmoContainerModule"/>.</returns>
        public static Magazine Get(IAmmoContainerModule module)
        {
            if (module == null)
                return null;

            return module switch
            {
                AutomaticActionModule actomatic => new AutomaticBarrelMagazine(actomatic),
                PumpActionModule pump => new PumpBarrelMagazine(pump),
                IPrimaryAmmoContainerModule primary => primary switch
                {
                    CylinderAmmoModule cylinder => new CylinderMagazine(cylinder),
                    MagazineModule magazine => magazine switch
                    {
                        Scp127MagazineModule scp127MagazineModule => new Scp127Magazine(scp127MagazineModule),
                        _ => new NormalMagazine(magazine)
                    },
                    _ => null,
                },
                _ => null,
            };
        }

        /// <summary>
        /// Modifies stored ammo in magazine.
        /// </summary>
        /// <param name="delta">Ammo change value.</param>
        /// <param name="useBorders">Whether new ammo should be clamped in range of <see langword="0"/> and <see cref="MaxAmmo"/>.</param>
        /// <returns>Resultly changed ammos.</returns>
        /// <remarks>
        /// Just a variation of the <see cref="Ammo"/> setter.
        /// </remarks>
        public int ModifyAmmo(int delta, bool useBorders = true)
        {
            int oldAmmo = Ammo;
            if (useBorders)
            {
                Ammo = Mathf.Clamp(Ammo + delta, 0, MaxAmmo);
            }
            else
            {
                Ammo += delta;
            }

            return Ammo - oldAmmo;
        }

        /// <summary>
        /// Fills current <see cref="Ammo"/> to <see cref="MaxAmmo"/>.
        /// </summary>
        public void Fill() => Ammo = MaxAmmo;

        /// <summary>
        /// Resyncs a related values with a client.
        /// </summary>
        public abstract void Resync();
    }
}
