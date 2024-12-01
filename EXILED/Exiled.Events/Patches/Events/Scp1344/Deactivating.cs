// -----------------------------------------------------------------------
// <copyright file="Deactivating.cs" company="ExMod Team">
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
    using InventorySystem.Items.Usables.Scp244;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="Scp1344Item.ServerUpdateDeactivating"/>.
    /// Adds the <see cref="Handlers.Scp1344.TryingDeactivating" /> event,
    /// <see cref="Handlers.Scp1344.Deactivating" /> event and
    /// <see cref="Handlers.Scp1344.Deactivated" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp1344), nameof(Handlers.Scp1344.TryingDeactivating))]
    [EventPatch(typeof(Handlers.Scp1344), nameof(Handlers.Scp1344.Deactivating))]
    [EventPatch(typeof(Handlers.Scp1344), nameof(Handlers.Scp1344.Deactivated))]
    [HarmonyPatch(typeof(Scp1344Item), nameof(Scp1344Item.ServerUpdateDeactivating))]
    internal static class Deactivating
    {
        private static bool Prefix(ref Scp1344Item __instance)
        {
            if (__instance._useTime == 0)
            {
                var ev = new TryingDeactivatingEventArgs(Item.Get(__instance));
                Exiled.Events.Handlers.Scp1344.OnTryingDeactivating(ev);

                if (!ev.IsAllowed)
                {
                    return StopDeactivation(__instance);
                }
            }

            if (__instance._useTime + Time.deltaTime >= 5.1f)
            {
                var deactivating = new DeactivatingEventArgs(Item.Get(__instance));
                Exiled.Events.Handlers.Scp1344.OnDeactivating(deactivating);

                if (!deactivating.IsAllowed)
                {
                    return StopDeactivation(__instance);
                }

                __instance.ActivateFinalEffects();
                __instance.ServerDropItem(__instance);

                var ev = new DeactivatedEventArgs(Item.Get(__instance));
                Exiled.Events.Handlers.Scp1344.OnDeactivated(ev);
                return false;
            }

            return true;
        }

        private static bool StopDeactivation(Scp1344Item instance)
        {
            instance.Status = Scp1344Status.Active;
            instance.ServerSetStatus(Scp1344Status.Active);
            return false;
        }
    }
}
