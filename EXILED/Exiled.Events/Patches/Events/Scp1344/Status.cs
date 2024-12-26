// -----------------------------------------------------------------------
// <copyright file="Status.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

namespace Exiled.Events.Patches.Events.Scp1344
{
    using Exiled.API.Features.Items;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp1344;
    using HarmonyLib;
    using InventorySystem.Items.Usables.Scp1344;

    /// <summary>
    /// Patches <see cref="Scp1344Item.Status"/>.
    /// Adds the <see cref="Handlers.Scp1344.ChangingStatus" /> event and
    /// <see cref="Handlers.Scp1344.ChangedStatus" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp1344), nameof(Handlers.Scp1344.ChangingStatus))]
    [EventPatch(typeof(Handlers.Scp1344), nameof(Handlers.Scp1344.ChangedStatus))]
    [HarmonyPatch(typeof(Scp1344Item), nameof(Scp1344Item.Status), MethodType.Setter)]
    internal static class Status
    {
        private static bool Prefix(Scp1344Item __instance, ref Scp1344Status value)
        {
            ChangingStatusEventArgs ev = new(Item.Get(__instance), value, __instance._status);
            Handlers.Scp1344.OnChangingStatus(ev);
            value = ev.Scp1344StatusNew;
            return ev.IsAllowed;
        }

        private static void Postfix(Scp1344Item __instance, ref Scp1344Status value)
        {
            ChangedStatusEventArgs ev = new(Item.Get(__instance), value);
            Handlers.Scp1344.OnChangedStatus(ev);
        }
    }
}