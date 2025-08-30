// -----------------------------------------------------------------------
// <copyright file="NameTagDetailData.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic.KeycardDetails
{
    using Exiled.API.Features.Items.Keycards;
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
        [HarmonyPrefix]
        private static void PrefixItem(KeycardItem item)
        {
            if (CustomKeycardItem.DataDict.TryGetValue(item.ItemSerial, out KeycardData data))
                data.NameTag = NametagDetail._customNametag;
        }

        [HarmonyPatch(nameof(NametagDetail.WriteNewPickup))]
        [HarmonyPrefix]
        private static void PrefixPickup(KeycardPickup pickup)
        {
            if (CustomKeycardItem.DataDict.TryGetValue(pickup.ItemId.SerialNumber, out KeycardData data))
                data.NameTag = NametagDetail._customNametag;
        }
    }
}