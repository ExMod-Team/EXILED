// -----------------------------------------------------------------------
// <copyright file="ElevatorSequencesUpdated.cs" company="ExMod Team">
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
    /// Adds the <see cref="Handlers.Map.ElevatorSequencesUpdated" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Map), nameof(Handlers.Map.ElevatorSequencesUpdated))]
    [HarmonyPatch(typeof(ElevatorChamber), nameof(ElevatorChamber.CurSequence), MethodType.Setter)]
    internal class ElevatorSequencesUpdated
    {
        private static void Postfix(ElevatorChamber __instance)
        {
            ElevatorSequencesUpdatedEventArgs ev = new(__instance, __instance.CurSequence);
            Handlers.Map.OnElevatorSequencesUpdated(ev);
        }
    }
}