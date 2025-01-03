// -----------------------------------------------------------------------
// <copyright file="RoomLightControllersList.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using Exiled.API.Features;
#pragma warning disable SA1313

    using HarmonyLib;

    /// <summary>
    /// Patch for adding <see cref="RoomLightController"/> to list.
    /// </summary>
    [HarmonyPatch(typeof(RoomLightController), nameof(RoomLightController.Start))]
    internal class RoomLightControllersList
    {
        private static void Postfix(RoomLightController __instance)
        {
            Room.Get(__instance.Room).RoomLightControllersValue.Add(__instance);
        }
    }
}