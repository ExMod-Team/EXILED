using Interactables.Interobjects.DoorUtils;
using System.Collections.Generic;
// -----------------------------------------------------------------------
// <copyright file="Door.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Doors
{
    internal static class DoorHelpers
    {

        /// <summary>
        /// A <see cref="Dictionary{TKey,TValue}"/> containing all known <see cref="DoorVariant"/>'s and their corresponding <see cref="Door"/>.
        /// </summary>
        internal static readonly Dictionary<DoorVariant, Door> DoorVariantToDoor = new(new ComponentsEqualityComparer());
    }
}