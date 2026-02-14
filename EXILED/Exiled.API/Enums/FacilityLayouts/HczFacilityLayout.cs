// -----------------------------------------------------------------------
// <copyright file="HczFacilityLayout.cs" company="ExMod Team">
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
    public enum HczFacilityLayout
    {
        /// <summary>
        /// Represents an unknown layout. This value is only used if you try to access <see cref="Map.HczLayout"/> prematurely or if an error occured.
        /// </summary>
        Unknown,

        /// <summary>
        /// See <see cref="HczFacilityLayout"/> for details.
        /// </summary>
        CCross,

        /// <summary>
        /// See <see cref="HczFacilityLayout"/> for details.
        /// </summary>
        Storm,

        /// <summary>
        /// See <see cref="HczFacilityLayout"/> for details.
        /// </summary>
        TopSquares,

        /// <summary>
        /// See <see cref="HczFacilityLayout"/> for details.
        /// </summary>
        Inkblot,

        /// <summary>
        /// See <see cref="HczFacilityLayout"/> for details.
        /// </summary>
        RottenHeart,

        /// <summary>
        /// See <see cref="HczFacilityLayout"/> for details.
        /// </summary>
        NewTall,

        /// <summary>
        /// See <see cref="HczFacilityLayout"/> for details.
        /// </summary>
        Split,

        /// <summary>
        /// See <see cref="HczFacilityLayout"/> for details.
        /// </summary>
        NewCircuit,

        /// <summary>
        /// See <see cref="HczFacilityLayout"/> for details.
        /// </summary>
        Grasp,

        /// <summary>
        /// See <see cref="HczFacilityLayout"/> for details.
        /// </summary>
        Help,
    }
}