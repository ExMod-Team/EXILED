// -----------------------------------------------------------------------
// <copyright file="EzFacilityLayout.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// Represents different layouts each zone in the facility can have.
    /// </summary>
    /// <remarks>Layout names come from https://steamcommunity.com/sharedfiles/filedetails/?id=2919451768, courtesy of EdgelordGreed.
    /// <para>Ordering comes from the order said layouts are stored in SL.</para>
    /// </remarks>
    public enum EzFacilityLayout
    {
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