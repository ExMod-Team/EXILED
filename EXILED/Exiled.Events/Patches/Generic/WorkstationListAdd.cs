// -----------------------------------------------------------------------
// <copyright file="WorkstationListAdd.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313
#pragma warning disable SA1402

    using HarmonyLib;
    using InventorySystem.Items.Firearms.Attachments;

    /// <summary>
    /// Patch for adding <see cref="API.Features.Workstation"/> to list.
    /// </summary>
    [HarmonyPatch(typeof(WorkstationController), nameof(WorkstationController.Start))]
    internal class WorkstationListAdd
    {
        private static void Postfix(WorkstationController __instance)
        {
            API.Features.Workstation.Get(__instance);
        }
    }

    /// <summary>
    /// Patch for removing <see cref="API.Features.Workstation"/> to list.
    /// </summary>
    [HarmonyPatch(typeof(WorkstationController), nameof(WorkstationController.OnDestroy))]
    internal class WorkstationListRemove
    {
        private static void Postfix(WorkstationController __instance)
        {
            API.Features.Workstation.WorkstationControllerToWorkstation.Remove(__instance);
        }
    }
}