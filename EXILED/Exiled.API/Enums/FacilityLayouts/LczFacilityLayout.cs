// -----------------------------------------------------------------------
// <copyright file="LczFacilityLayout.cs" company="ExMod Team">
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
    public enum LczFacilityLayout
    {
        /// <summary>
        /// Represents an unknown layout. This value is only used if you try to access <see cref="Map.LczLayout"/> prematurely or if an error occured.
        /// </summary>
        Unknown,

        /// <summary>
        /// See <see cref="LczFacilityLayout"/> for details.
        /// </summary>
        Clothes,

        /// <summary>
        /// See <see cref="LczFacilityLayout"/> for details.
        /// </summary>
        Stool,

        /// <summary>
        /// See <see cref="LczFacilityLayout"/> for details.
        /// </summary>
        Controller,

        /// <summary>
        /// See <see cref="LczFacilityLayout"/> for details.
        /// </summary>
        Brain,

        /// <summary>
        /// See <see cref="LczFacilityLayout"/> for details.
        /// </summary>
        Skull,
    }
}