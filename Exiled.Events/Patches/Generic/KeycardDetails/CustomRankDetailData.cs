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
            if (!CustomKeycardItem.DataDict.TryGetValue(item.ItemSerial, out KeycardData data))
                CustomKeycardItem.DataDict[item.ItemSerial] = data = new KeycardData();
            data.Rank = (byte)(Mathf.Abs(CustomRankDetail._index) % __instance._options.Length);
        }

        [HarmonyPatch(nameof(CustomRankDetail.WriteNewPickup))]
        [HarmonyPrefix]
        private static void PrefixPickup(CustomRankDetail __instance, KeycardPickup pickup)
        {
            if (!CustomKeycardItem.DataDict.TryGetValue(pickup.ItemId.SerialNumber, out KeycardData data))
                CustomKeycardItem.DataDict[pickup.ItemId.SerialNumber] = data = new KeycardData();
            data.Rank = (byte)(Mathf.Abs(CustomRankDetail._index) % __instance._options.Length);
        }
    }
}