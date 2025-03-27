// -----------------------------------------------------------------------
// <copyright file="MicroHIDOpeningDoor.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

namespace Exiled.Events.Patches.Events.Player
{
    using Attributes;

    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using Interactables;
    using Interactables.Interobjects;
    using InventorySystem.Items.MicroHID.Modules;

    /// <summary>
    /// Patches <see cref="InventorySystem.Items.MicroHID.Modules.ChargeFireModeModule.HandlePotentialDoor"/>.
    /// Adds the <see cref="MicroHIDOpeningDoor" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.MicroHIDOpeningDoor))]
    [HarmonyPatch(typeof(ChargeFireModeModule), nameof(ChargeFireModeModule.HandlePotentialDoor))]
    internal static class MicroHIDOpeningDoor
    {
        private static void Prefix(ref ChargeFireModeModule __instance, InteractableCollider interactable)
        {
            BreakableDoor breakableDoor = interactable.Target as BreakableDoor;
            if(breakableDoor == null)
            {
                return;
            }

            if (breakableDoor.TargetState)
            {
                return;
            }
//
            if (breakableDoor.AllowInteracting(__instance.Item.Owner, interactable.ColliderId))
            {
                return;
            }

            MicroHIDOpeningDoorEventArgs ev = new(__instance.MicroHid);
            Exiled.Events.Handlers.Player.OnMicroHIDOpeningDoor(ev);

            if (!ev.IsAllowed)
            {
                return;
            }

            if ((breakableDoor.ActiveLocks & (ushort)(~(ushort)ChargeFireModeModule.BypassableLocks)) == 0)
            {
                breakableDoor.NetworkTargetState = true;
            }
        }
    }
}
