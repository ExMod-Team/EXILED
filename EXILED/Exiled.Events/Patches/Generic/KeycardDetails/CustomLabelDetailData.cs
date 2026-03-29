// -----------------------------------------------------------------------
// <copyright file="CustomLabelDetailData.cs" company="ExMod Team">
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
    /// Patch for storing label and label color from custom keycards.
    /// </summary>
    [HarmonyPatch(typeof(CustomLabelDetail))]
    public class CustomLabelDetailData
    {
        [HarmonyPatch(nameof(CustomLabelDetail.WriteNewItem))]
        [HarmonyPrefix]
        private static void Prefix(KeycardItem item)
        {
            if (!CustomKeycardItem.DataDict.TryGetValue(item.ItemSerial, out KeycardData data))
                CustomKeycardItem.DataDict[item.ItemSerial] = data = new KeycardData();
            data.Label = CustomLabelDetail._customText;
            data.LabelColor = CustomLabelDetail._customColor;
        }

        [HarmonyPatch(nameof(CustomLabelDetail.WriteNewPickup))]
        [HarmonyPrefix]
        private static void Prefix(KeycardPickup pickup)
        {
            if (!CustomKeycardItem.DataDict.TryGetValue(pickup.ItemId.SerialNumber, out KeycardData data))
                CustomKeycardItem.DataDict[pickup.ItemId.SerialNumber] = data = new KeycardData();
            data.Label = CustomLabelDetail._customText;
            data.LabelColor = CustomLabelDetail._customColor;
        }
    }
}