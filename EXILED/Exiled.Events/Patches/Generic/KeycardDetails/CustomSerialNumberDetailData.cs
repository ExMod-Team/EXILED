// -----------------------------------------------------------------------
// <copyright file="CustomSerialNumberDetailData.cs" company="ExMod Team">
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
    /// Patch for storing serial numbers from custom keycards.
    /// </summary>
    [HarmonyPatch(typeof(CustomSerialNumberDetail))]
    public class CustomSerialNumberDetailData
    {
        [HarmonyPatch(nameof(CustomSerialNumberDetail.WriteNewItem))]
        [HarmonyPrefix]
        private static void PrefixItem(KeycardItem item)
        {
            CustomKeycard.DataDict[item.ItemSerial].SerialNumber = CustomSerialNumberDetail._customVal;
        }

        [HarmonyPatch(nameof(CustomSerialNumberDetail.WriteNewPickup))]
        [HarmonyPrefix]
        private static void PrefixPickup(KeycardPickup pickup)
        {
            CustomKeycard.DataDict[pickup.ItemId.SerialNumber].SerialNumber = CustomSerialNumberDetail._customVal;
        }
    }
}