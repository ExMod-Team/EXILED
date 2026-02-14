// -----------------------------------------------------------------------
// <copyright file="EzFacilityLayout.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using Exiled.API.Features;

    /// <summary>
    /// Represents different layouts each zone in the facility can have.
    /// </summary>
    /// <remarks>Layout names come from https://steamcommunity.com/sharedfiles/filedetails/?id=2919451768, courtesy of EdgelordGreed.
    /// <para>Ordering comes from the order said layouts are stored in SL.</para>
    /// </remarks>
    public enum EzFacilityLayout
    {
        /// <summary>
        /// Represents an unknown layout. This value is only used if you try to access <see cref="Map.EzLayout"/> prematurely or if an error occured.
        /// </summary>
        Unknown,

        /// <summary>
        /// See <see cref="EzFacilityLayout"/> for details.
        /// </summary>
        Rectangles,

        /// <summary>
        /// See <see cref="EzFacilityLayout"/> for details.
        /// </summary>
        Handbag,

        /// <summary>
        /// See <see cref="EzFacilityLayout"/> for details.
        /// </summary>
        Fractured,

        /// <summary>
        /// See <see cref="EzFacilityLayout"/> for details.
        /// </summary>
        L,

        /// <summary>
        /// See <see cref="EzFacilityLayout"/> for details.
        /// </summary>
        Mogus,
    }
}