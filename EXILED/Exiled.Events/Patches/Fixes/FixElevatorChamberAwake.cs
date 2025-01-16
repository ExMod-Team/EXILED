// -----------------------------------------------------------------------
// <copyright file="FixElevatorChamberAwake.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using Exiled.Events.Attributes;
    using HarmonyLib;
    using Interactables.Interobjects;

    /// <summary>
    /// Patches <see cref="ElevatorChamber.Awake" />'s setter.
    /// Fix for the <see cref="Handlers.Map.ElevatorSequencesUpdated" /> event having its Lift value being null.
    /// </summary>
    [HarmonyPatch(typeof(ElevatorChamber), nameof(ElevatorChamber.Awake))]
    internal class FixElevatorChamberAwake
    {
#pragma warning disable SA1313
        private static void Postfix(ElevatorChamber __instance)
#pragma warning restore SA1313
        {
            __instance._floorDoors = ElevatorDoor.GetDoorsForGroup(__instance.AssignedGroup);
        }
    }
}