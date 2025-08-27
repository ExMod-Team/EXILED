// -----------------------------------------------------------------------
// <copyright file="CustomRankDetailData.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic.KeycardDetails
{
    using Exiled.API.Features.Items.Keycards;
    using HarmonyLib;
    using InventorySystem.Items.Keycards;
    using UnityEngine;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

    /// <summary>
    /// Patch for storing rank from custom keycards.
    /// </summary>
    [HarmonyPatch(typeof(CustomRankDetail))]
    public class CustomRankDetailData
    {
        [HarmonyPatch(nameof(CustomRankDetail.WriteNewItem))]
        [HarmonyPrefix]
        private static void PrefixItem(CustomRankDetail __instance, KeycardItem item)
        {
            CustomKeycard.DataDict[item.ItemSerial].Rank = (byte)(Mathf.Abs(CustomRankDetail._index) % __instance._options.Length);
        }

        [HarmonyPatch(nameof(CustomRankDetail.WriteNewPickup))]
        [HarmonyPrefix]
        private static void PrefixPickup(CustomRankDetail __instance, KeycardPickup pickup)
        {
            CustomKeycard.DataDict[pickup.ItemId.SerialNumber].Rank = (byte)(Mathf.Abs(CustomRankDetail._index) % __instance._options.Length);
        }
    }
}