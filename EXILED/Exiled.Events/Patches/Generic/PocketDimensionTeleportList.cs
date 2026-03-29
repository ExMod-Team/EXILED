// -----------------------------------------------------------------------
// <copyright file="PocketDimensionTeleportList.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313
    using API.Features;

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="PocketDimensionTeleport.Awake"/>.
    /// </summary>
    [HarmonyPatch]
    internal class PocketDimensionTeleportList
    {
        [HarmonyPatch(typeof(PocketDimensionTeleport), nameof(PocketDimensionTeleport.Awake))]
        [HarmonyPostfix]
        private static void Adding(PocketDimensionTeleport __instance) => Map.TeleportsValue.Add(__instance);

        [HarmonyPatch(typeof(PocketDimensionTeleport), nameof(PocketDimensionTeleport.OnDestroy))]
        [HarmonyPostfix]
        private static void Removing(PocketDimensionTeleport __instance) => Map.TeleportsValue.Remove(__instance);
    }
}