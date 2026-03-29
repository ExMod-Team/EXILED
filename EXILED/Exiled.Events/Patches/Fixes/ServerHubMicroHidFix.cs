// -----------------------------------------------------------------------
// <copyright file="ServerHubMicroHidFix.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
#pragma warning disable SA1313
    using API.Features.Items;
    using Exiled.API.Features;

    using HarmonyLib;

    using InventorySystem.Items.Autosync;
    using InventorySystem.Items.MicroHID.Modules;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.Usables.Scp330;

    /// <summary>
    /// Patches <see cref="CycleSyncModule.Update()"/> to fix phantom <see cref="MicroHid"/> for <see cref="Item.Create(ItemType, API.Features.Player)"/>.
    /// </summary>
    [HarmonyPatch(typeof(CycleSyncModule), nameof(CycleSyncModule.Update))]
    internal static class ServerHubMicroHidFix
    {
        private static bool Prefix(CycleSyncModule __instance)
        {
            return __instance.MicroHid.InstantiationStatus == AutosyncInstantiationStatus.InventoryInstance;
        }
    }
}
