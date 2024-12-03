// -----------------------------------------------------------------------
// <copyright file="FirearmDistribution.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313
    using Exiled.API.Features.Pickups;
    using HarmonyLib;

    using BaseFirearm = InventorySystem.Items.Firearms.FirearmPickup;

    /// <summary>
    /// Patch to add <see cref="FirearmPickup.IsDistributed"/>.
    /// </summary>
    [HarmonyPatch(typeof(BaseFirearm), nameof(BaseFirearm.OnDistributed))]
    internal static class FirearmDistribution
    {
        private static void Postfix(BaseFirearm __instance)
        {
            Pickup.Get<FirearmPickup>(__instance).IsDistributed = true;
        }
    }
}