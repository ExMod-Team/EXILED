// -----------------------------------------------------------------------
// <copyright file="ServerHub330Fix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
#pragma warning disable SA1313
    using API.Features.Items;
    using Exiled.API.Features;

    using HarmonyLib;

    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.Usables.Scp330;

    /// <summary>
    /// Patches <see cref="Scp330Bag.OnAdded(ItemPickupBase)"/> to fix fake adding/removing <see cref="Scp330"/> for <see cref="Item.Create(ItemType, API.Features.Player)"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp330Bag), nameof(Scp330Bag.OnAdded))]
    internal static class ServerHub330Fix
    {
        private static bool Prefix(Scp330Bag __instance, ItemPickupBase pickup)
        {
            return __instance.Owner != Server.Host.ReferenceHub;
        }
    }
}
