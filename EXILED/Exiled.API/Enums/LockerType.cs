// -----------------------------------------------------------------------
// <copyright file="LockerType.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// Unique identifier for different types of <see cref="Features.Lockers.Locker"/>s.
    /// </summary>
    public enum LockerType
    {
        /// <summary>
        /// The pedestal used by SCP items.
        /// </summary>
        Pedestal,

        /// <summary>
        /// Large weapon locker.
        /// </summary>
        LargeGun,

        /// <summary>
        /// Locker for rifles, known as a rifle rack.
        /// </summary>
        RifleRack,

        /// <summary>
        /// Miscellaneous locker for various items.
        /// </summary>
        Misc,

        /// <summary>
        /// Locker that contains medkits.
        /// </summary>
        Medkit,

        /// <summary>
        /// Locker that contains adrenaline.
        /// </summary>
        Adrenaline,

        /// <summary>
        /// Unknow type of locker.
        /// </summary>
        Unknow,
    }
}
