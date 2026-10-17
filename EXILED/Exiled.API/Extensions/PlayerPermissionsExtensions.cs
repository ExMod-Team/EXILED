// -----------------------------------------------------------------------
// <copyright file="PlayerPermissionsExtensions.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System.Collections.Generic;

    /// <summary>
    /// A set of extensions for <see cref="PlayerPermissions"/>.
    /// </summary>
    public static class PlayerPermissionsExtensions
    {
        /// <summary>
        /// Checks whether the current permissions contain any of the permissions specified in the mask.
        /// </summary>
        /// <param name="playerPermissions">The current permissions to check.</param>
        /// <param name="mask">The mask of permissions to test against.</param>
        /// <returns><see langword="true"/> if the current permissions contain at least one permission from the mask; otherwise, <see langword="false"/>.</returns>
        public static bool HasAnyPermission(this PlayerPermissions playerPermissions, PlayerPermissions mask)
        {
            return (playerPermissions & mask) != 0;
        }

        /// <summary>
        /// Checks whether the current permissions contain any of the permissions specified in the collection.
        /// </summary>
        /// <param name="playerPermissions">The current permissions to check.</param>
        /// <param name="collectionPlayerPermissions">The collection of permissions to test against.</param>
        /// <returns><see langword="true"/> if the current permissions contain at least one permission from the collection; otherwise, <see langword="false"/>.</returns>
        public static bool HasAnyPermission(this PlayerPermissions playerPermissions, IEnumerable<PlayerPermissions> collectionPlayerPermissions)
        {
            if (collectionPlayerPermissions is null)
                return false;

            foreach (PlayerPermissions perm in collectionPlayerPermissions)
            {
                if (playerPermissions.HasAnyPermission(perm))
                    return true;
            }

            return false;
        }
    }
}
