// -----------------------------------------------------------------------
// <copyright file="ServerDropEverythingFix.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;

    using HarmonyLib;
    using InventorySystem;
    using InventorySystem.Items;
    using NorthwoodLib.Pools;

    /// <summary>
    /// Patches <see cref="InventoryExtensions.ServerDropEverything(Inventory)"/>.
    /// Prevents from trying to drop destroyed items.
    /// </summary>
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerDropEverything))]
    public static class ServerDropEverythingFix
    {
        [HarmonyPrefix]
        private static bool Prefix(Inventory inv)
        {
            if (inv == null || inv.UserInventory == null || inv.UserInventory.Items.Count == 0)
                return true;

            List<ushort> toRemove = ListPool<ushort>.Shared.Rent();

            foreach (KeyValuePair<ushort, ItemBase> kvp in inv.UserInventory.Items)
            {
                if (kvp.Value == null)
                    toRemove.Add(kvp.Key);
            }

            foreach (ushort serial in toRemove)
                inv.UserInventory.Items.Remove(serial);

            ListPool<ushort>.Shared.Return(toRemove);
            return true;
        }
    }
}