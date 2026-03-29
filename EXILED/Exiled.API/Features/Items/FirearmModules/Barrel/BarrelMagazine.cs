// -----------------------------------------------------------------------
// <copyright file="BarrelMagazine.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items.FirearmModules.Barrel
{
    using InventorySystem.Items.Firearms.Modules;

    /// <summary>
    /// Basic abstraction of <see cref="IAmmoContainerModule"/> whose are logically used to be a barrels magazines.
    /// </summary>
    public abstract class BarrelMagazine : Magazine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarrelMagazine"/> class.
        /// </summary>
        /// <param name="module">target <see cref="IAmmoContainerModule"/>.</param>
        public BarrelMagazine(IAmmoContainerModule module)
            : base(module)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether barrel is cocked.
        /// </summary>
        public abstract bool IsCocked { get; set; }
    }
}
