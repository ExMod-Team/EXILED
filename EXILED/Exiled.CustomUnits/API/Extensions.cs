// -----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomUnits.API
{
    using System.Collections.Generic;

    using Exiled.API.Features;
    using Exiled.CustomUnits.API.Features;

    /// <summary>
    /// A class containing extension methods for <see cref="CustomUnit"/> API.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Gets all custom units from player.
        /// </summary>
        /// <param name="player">Target player.</param>
        /// <returns>A collection of custom units.</returns>
        public static CustomUnit? GetCustomUnit(this Player player)
        {
            foreach (CustomUnit? customUnit in CustomUnit.Registered)
            {
                if (customUnit?.Check(player) ?? false)
                    return customUnit;
            }

            return null;
        }

        /// <summary>
        /// Registers a custom unit.
        /// </summary>
        /// <param name="customUnit"><see cref="CustomUnit"/> instance.</param>
        /// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
        public static bool Register(this CustomUnit customUnit) => customUnit.TryRegister();

        /// <summary>
        /// Unregisters a custom unit.
        /// </summary>
        /// <param name="customUnit">A <see cref="CustomUnit"/> instance.</param>
        /// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
        public static bool Unregister(this CustomUnit customUnit) => customUnit.TryUnregister();
    }
}