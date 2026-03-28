// -----------------------------------------------------------------------
// <copyright file="AdminToyList.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.Internal
{
    /// <summary>
    /// Handles adding and removing from <see cref="Exiled.API.Features.Toys.AdminToy.List"/>.
    /// </summary>
    internal static class AdminToyList
    {
        /// <summary>
        /// Called after a ragdoll is spawned. Hooked to <see cref="AdminToys.AdminToyBase.OnAdded"/>.
        /// </summary>
        /// <param name="adminToy">The spawned ragdoll.</param>
        public static void OnAddedAdminToys(AdminToys.AdminToyBase adminToy) => API.Features.Toys.AdminToy.Get(adminToy);

        /// <summary>
        /// Called before a ragdoll is destroyed. Hooked to <see cref="AdminToys.AdminToyBase.OnRemoved"/>.
        /// </summary>
        /// <param name="adminToy">The destroyed ragdoll.</param>
        public static void OnRemovedAdminToys(AdminToys.AdminToyBase adminToy) => API.Features.Toys.AdminToy.BaseToAdminToy.Remove(adminToy);
    }
}
