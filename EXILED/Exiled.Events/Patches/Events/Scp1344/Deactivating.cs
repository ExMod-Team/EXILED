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

    using static PlayerList;

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
                TryingDeactivatingEventArgs ev = new(Item.Get(__instance));
                Handlers.Scp1344.OnTryingDeactivating(ev);

                if (!ev.IsAllowed)
                {
                    return StopDeactivation(__instance, Scp1344Status.Active);
                }
            }

            if (__instance._useTime + Time.deltaTime >= Scp1344Item.DeactivationTime)
            {
                DeactivatingEventArgs deactivating = new(Item.Get(__instance));
                Handlers.Scp1344.OnDeactivating(deactivating);

                if (!deactivating.IsAllowed)
                {
                    return StopDeactivation(__instance, deactivating.NewStatus);
                }

                __instance.ActivateFinalEffects();
                __instance.ServerDropItem(__instance);

                DeactivatedEventArgs ev = new(Item.Get(__instance));
                Handlers.Scp1344.OnDeactivated(ev);
                return false;
            }

            return true;
        }

        private static bool StopDeactivation(Scp1344Item instance, Scp1344Status newStatus)
        {
            instance.Status = newStatus;
            instance.ServerSetStatus(newStatus);

            if (newStatus == Scp1344Status.Idle)
            {
                instance._useTime = 0f;
                instance._savedIntensity = 0;
                instance._cancelationTime = 0f;
                instance.OwnerInventory?.ServerSelectItem(0);
            }

            return false;
        }
    }
}
