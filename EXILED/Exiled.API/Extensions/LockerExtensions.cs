// -----------------------------------------------------------------------
// <copyright file="LockerExtensions.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;

    using Exiled.API.Enums;
    using MapGeneration.Distributors;

    /// <summary>
    /// A set of extensions for <see cref="Enums.LockerType"/>.
    /// </summary>
    public static class LockerExtensions
    {
        /// <summary>
        /// Gets the <see cref="LockerType"/> from the given <see cref="Locker"/> object.
        /// </summary>
        /// <param name="locker">The <see cref="Locker"/> to check.</param>
        /// <returns>The corresponding <see cref="LockerType"/>.</returns>
        public static LockerType GetLockerType(this Locker locker) => locker.name.GetLockerTypeByName();

        /// <summary>
        /// Gets the <see cref="LockerType"/> by name.
        /// </summary>
        /// <param name="name">The name to check.</param>
        /// <returns>The corresponding <see cref="LockerType"/>.</returns>
        public static LockerType GetLockerTypeByName(this string name) => name.Replace("(Clone)", string.Empty) switch
        {
            "Scp500PedestalStructure Variant" => LockerType.Pedestal,
            "LargeGunLockerStructure" => LockerType.LargeGun,
            "RifleRackStructure" => LockerType.RifleRack,
            "MiscLocker" => LockerType.Misc,
            "RegularMedkitStructure" => LockerType.Medkit,
            "AdrenalineMedkitStructure" => LockerType.Adrenaline,
            _ => LockerType.Unknow,
        };
    }
}
