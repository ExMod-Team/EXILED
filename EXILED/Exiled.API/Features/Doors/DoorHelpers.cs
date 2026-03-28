// -----------------------------------------------------------------------
// <copyright file="DoorHelpers.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Doors
{
    using System.Collections.Generic;

    using Interactables.Interobjects.DoorUtils;

    /// <summary>
    /// Helper class for door-related utilities and door variant mappings.
    /// </summary>
    internal static class DoorHelpers
    {
        /// <summary>
        /// A <see cref="Dictionary{TKey,TValue}"/> containing all known <see cref="DoorVariant"/>'s and their corresponding <see cref="Door"/>.
        /// </summary>
        internal static readonly Dictionary<DoorVariant, Door> DoorVariantToDoor = new(new ComponentsEqualityComparer());
    }
}