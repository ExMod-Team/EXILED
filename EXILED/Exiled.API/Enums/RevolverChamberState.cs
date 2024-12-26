// -----------------------------------------------------------------------
// <copyright file="RevolverChamberState.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using Exiled.API.Features.Items.FirearmModules.Primary;

    /// <summary>
    /// States for chamber in revolver cylindric magazine.
    /// </summary>
    /// <seealso cref="CylinderMagazine"/>
    public enum RevolverChamberState
    {
        /// <summary>
        /// State for empty chamber.
        /// </summary>
        Empty,

        /// <summary>
        /// State for chamber with a bullet.
        /// </summary>
        Live,

        /// <summary>
        /// State for discharged chamber.
        /// </summary>
        Discharged,
    }
}
