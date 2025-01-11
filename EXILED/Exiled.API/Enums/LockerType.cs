// -----------------------------------------------------------------------
// <copyright file="LockerType.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using System;

    /// <summary>
    /// Unique identifier for different types of <see cref="Features.Lockers.Locker"/>s.
    /// </summary>
    public enum LockerType
    {
        /// <summary>
        /// The pedestal used by SCP items.
        /// </summary>
        [Obsolete("This value is not used.")]
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

        /// <summary>
        /// MircoHid pedestal.
        /// </summary>
        MicroHid,

        /// <summary>
        /// Experimental weapon locker.
        /// </summary>
        ExperimentalWeapon,

        /// <summary>
        /// SCP-500 pedestal.
        /// </summary>
        Scp500Pedestal,

        /// <summary>
        /// SCP-207? (Anti SCP-207) pedestal.
        /// </summary>
        AntiScp207Pedestal,

        /// <summary>
        /// SCP-207 pedestal.
        /// </summary>
        Scp207Pedestal,

        /// <summary>
        /// SCP-268 pedestal.
        /// </summary>
        Scp268Pedestal,

        /// <summary>
        /// SCP-1344 pedestal.
        /// </summary>
        Scp1344Pedestal,

        /// <summary>
        /// SCP-018 pedestal.
        /// </summary>
        Scp018Pedestal,

        /// <summary>
        /// SCP-1576 pedestal.
        /// </summary>
        Scp1576Pedestal,

        /// <summary>
        /// SCP-244 pedestal.
        /// </summary>
        Scp244Pedestal,

        /// <summary>
        /// SCP-2176 pedestal.
        /// </summary>
        Scp2176Pedestal,

        /// <summary>
        /// SCP-1853 pedestal.
        /// </summary>
        Scp1853Pedestal,
    }
}
