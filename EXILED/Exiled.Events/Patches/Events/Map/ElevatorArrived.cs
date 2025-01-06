// -----------------------------------------------------------------------
// <copyright file="ElevatorArrived.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using Attributes;
    using Exiled.Events.EventArgs.Map;
    using HarmonyLib;
    using Interactables.Interobjects;

    using Map = Exiled.Events.Handlers.Map;

    /// <summary>
    /// Patches <see cref="ElevatorChamber.CurSequence" />'s setter.
    /// Adds the <see cref="Map.ElevatorArrived" /> event.
    /// </summary>
    [EventPatch(typeof(Map), nameof(Map.ElevatorArrived))]
    [HarmonyPatch(typeof(ElevatorChamber), nameof(ElevatorChamber.CurSequence), MethodType.Setter)]
    internal static class ElevatorArrived
    {
#pragma warning disable SA1313
        // ReSharper disable once InconsistentNaming
        private static void Postfix(ElevatorChamber __instance)
#pragma warning restore SA1313
        {
            // ReSharper disable once InvertIf
            if (__instance.CurSequence == ElevatorChamber.ElevatorSequence.DoorOpening)
            {
                ElevatorArrivedEventArgs ev = new(__instance, __instance.DestinationLevel);
                Map.OnElevatorArrived(ev);
            }
        }
    }
}