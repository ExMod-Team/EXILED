// -----------------------------------------------------------------------
// <copyright file="LiftList.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    using API.Features;

    using HarmonyLib;
    using Interactables.Interobjects;

    /// <summary>
    /// Patches to control <see cref="Lift.List"/>.
    /// </summary>
    [HarmonyPatch]
    internal class LiftList
    {
        [HarmonyPatch(typeof(ElevatorChamber), nameof(ElevatorChamber.Start))]
        [HarmonyPostfix]
        private static void Adding(ElevatorChamber __instance) => _ = new Lift(__instance);

        [HarmonyPatch(typeof(ElevatorChamber), nameof(ElevatorChamber.OnDestroy))]
        [HarmonyPostfix]
        private static void Removing(ElevatorChamber __instance) => Lift.ElevatorChamberToLift.Remove(__instance);
    }
}