// -----------------------------------------------------------------------
// <copyright file="WearableArmorPatch.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;

    using HarmonyLib;

    using PlayerRoles.FirstPersonControl.Thirdperson.Subcontrollers.Wearables;

    /// <summary>
    /// Patches <see cref="WearableArmor.ServerCurArmor"/>.
    /// </summary>
    [HarmonyPatch(typeof(WearableSync), nameof(WearableSync.OnHubAdded))]
    internal static class WearableArmorPatch
    {
        private static bool Prefix(ref ReferenceHub hub)
        {
            foreach (KeyValuePair<uint, WearableSyncMessage> item in WearableSync.Database)
            {
                hub.connectionToClient.Send(item.Value);
            }

            return false;
        }
    }
}