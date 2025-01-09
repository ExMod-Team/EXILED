// -----------------------------------------------------------------------
// <copyright file="ElevatorSequences.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;
    using HarmonyLib;
    using Interactables.Interobjects;

    /// <summary>
    /// Patches <see cref="ElevatorChamber.CurSequence" />'s setter.
    /// Adds the <see cref="Handlers.Map.ElevatorArrived" /> event.
    /// Adds the <see cref="Handlers.Map.ElevatorSequencesUpdated" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Map), nameof(Handlers.Map.ElevatorSequencesUpdated))]
    [HarmonyPatch(typeof(ElevatorChamber), nameof(ElevatorChamber.CurSequence), MethodType.Setter)]
    internal class ElevatorSequences
    {
        // ReSharper disable once InconsistentNaming
#pragma warning disable SA1313
        private static void Postfix(ElevatorChamber __instance)
#pragma warning restore SA1313
        {
            ElevatorSequencesUpdatedEventArgs ev = new(__instance, __instance.CurSequence);
            Handlers.Map.OnElevatorSequencesUpdated(ev);
            if (__instance.CurSequence != ElevatorChamber.ElevatorSequence.DoorOpening)
                return;
            ElevatorArrivedEventArgs ev2 = new(__instance, __instance.DestinationLevel);
            Handlers.Map.OnElevatorArrived(ev2);
        }
    }
}