// -----------------------------------------------------------------------
// <copyright file="NameTagDetailData.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic.KeycardDetails
{
    using Exiled.API.Features.Items.Keycards;
    using Exiled.Events.Patches.Fixes;
    using HarmonyLib;
    using InventorySystem.Items.Keycards;

    /// <summary>
    /// Patch for storing name tags from custom keycards.
    /// </summary>
    /// <remarks>There's a fix for <see cref="NametagDetail.WriteNewPickup"/>.</remarks>
    [HarmonyPatch(typeof(NametagDetail))]
    public class NameTagDetailData
    {
        [HarmonyPatch(nameof(NametagDetail.WriteNewItem))]
        [HarmonyTranspiler]
        private static void PrefixItem(KeycardItem item)
        {
            CustomKeycard.NameTagDict[item.ItemSerial] = NametagDetail._customNametag;
        }

        [HarmonyPatch(nameof(NametagDetail.WriteNewPickup))]
        [HarmonyTranspiler]
        private static void PrefixPickup(KeycardPickup pickup)
        {
            CustomKeycard.NameTagDict[pickup.ItemId.SerialNumber] = NametagDetail._customNametag;
        }
    }
}