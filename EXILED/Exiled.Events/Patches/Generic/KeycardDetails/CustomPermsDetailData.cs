// -----------------------------------------------------------------------
// <copyright file="CustomPermsDetailData.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic.KeycardDetails
{
    using Exiled.API.Features.Items.Keycards;
    using HarmonyLib;
    using InventorySystem.Items;
    using InventorySystem.Items.Keycards;

    /// <summary>
    /// Patch for storing permission colors from custom keycards.
    /// </summary>
    [HarmonyPatch(typeof(CustomPermsDetail), nameof(CustomPermsDetail.WriteCustom))]
    public class CustomPermsDetailData
    {
        private static void Prefix(IIdentifierProvider target)
        {
            if (!CustomKeycardItem.DataDict.TryGetValue(target.ItemId.SerialNumber, out KeycardData data))
                CustomKeycardItem.DataDict[target.ItemId.SerialNumber] = data = new KeycardData();
            data.PermissionsColor = CustomPermsDetail._customColor;
        }
    }
}