// -----------------------------------------------------------------------
// <copyright file="CustomWearDetailData.cs" company="ExMod Team">
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
    /// Patch for storing wear from custom keycards.
    /// </summary>
    [HarmonyPatch(typeof(CustomWearDetail))]
    public class CustomWearDetailData
    {
        [HarmonyPatch(nameof(CustomWearDetail.WriteNewItem))]
        [HarmonyPrefix]
        private static void PrefixItem(KeycardItem item)
        {
            CustomKeycard.WearDict[item.ItemSerial] = CustomWearDetail._customWearLevel;
        }

        [HarmonyPatch(nameof(CustomWearDetail.WriteNewPickup))]
        [HarmonyPrefix]
        private static void PrefixPickup(KeycardPickup pickup)
        {
            CustomKeycard.WearDict[pickup.ItemId.SerialNumber] = CustomWearDetail._customWearLevel;
        }
    }
}