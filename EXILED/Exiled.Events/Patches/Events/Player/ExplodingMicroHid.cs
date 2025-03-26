// -----------------------------------------------------------------------
// <copyright file="ExplodingMicroHid.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

namespace Exiled.Events.Patches.Events.Player
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using InventorySystem.Items.MicroHID;

    /// <summary>
    /// Patches <see cref="InventorySystem.Items.MicroHID.Modules.ChargeFireModeModule.ServerExplode"/>.
    /// Adds the <see cref="ExplodingMicroHid" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ExplodingMicroHid))]
    [HarmonyPatch(typeof(InventorySystem.Items.MicroHID.Modules.ChargeFireModeModule), nameof(InventorySystem.Items.MicroHID.Modules.ChargeFireModeModule.ServerExplode))]
    internal static class ExplodingMicroHid
    {
        private static bool Prefix(ref MicroHIDItem __instance)
        {
            ExplodingMicroHidEventArgs ev = new(Item.Get(__instance));
            Exiled.Events.Handlers.Player.OnExplodingMicroHid(ev);

            if (!ev.IsAllowed)
            {
                return false;
            }

            return true;
        }
    }
}