// -----------------------------------------------------------------------
// <copyright file="CustomItemNameDetailData.cs" company="ExMod Team">
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
    /// Patch for storing item name from custom keycards.
    /// </summary>
    [HarmonyPatch(typeof(CustomItemNameDetail))]
    public class CustomItemNameDetailData
    {
        [HarmonyPatch(nameof(CustomItemNameDetail.WriteNewItem))]
        [HarmonyPrefix]
        private static void PrefixItem(KeycardItem item)
        {
            CustomKeycardItem.DataDict[item.ItemSerial].ItemName = CustomItemNameDetail._customText;
        }

        [HarmonyPatch(nameof(CustomItemNameDetail.WriteNewPickup))]
        [HarmonyPrefix]
        private static void PrefixPickup(KeycardPickup pickup)
        {
            CustomKeycardItem.DataDict[pickup.ItemId.SerialNumber].ItemName = CustomItemNameDetail._customText;
        }
    }
}