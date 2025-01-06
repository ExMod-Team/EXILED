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
        public static LockerType GetLockerTypeByName(this string name) => name.Split('(')[0].Trim() switch
        {
            "Scp500PedestalStructure Variant" => LockerType.Scp500Pedestal,
            "AntiScp207PedestalStructure Variant" => LockerType.AntiScp207Pedestal,
            "Scp207PedestalStructure Variant" => LockerType.Scp207Pedestal,
            "Experimental Weapon Locker" => LockerType.ExperimentalWeapon,
            "Scp1344PedestalStructure Variant" => LockerType.Scp1344Pedestal,
            "Scp1576PedestalStructure Variant" => LockerType.Scp1576Pedestal,
            "Scp2176PedestalStructure Variant" => LockerType.Scp2176Pedestal,
            "Scp1853PedestalStructure Variant" => LockerType.Scp1853Pedestal,
            "Scp268PedestalStructure Variant" => LockerType.Scp268Pedestal,
            "Scp244PedestalStructure Variant" => LockerType.Scp244Pedestal,
            "Scp018PedestalStructure Variant" => LockerType.Scp018Pedestal,
            "LargeGunLockerStructure" => LockerType.LargeGun,
            "RifleRackStructure" => LockerType.RifleRack,
            "MiscLocker" => LockerType.Misc,
            "RegularMedkitStructure" => LockerType.Medkit,
            "AdrenalineMedkitStructure" => LockerType.Adrenaline,
            "MicroHIDpedestal" => LockerType.MicroHid,
            _ => LockerType.Unknow,
        };
    }
}
