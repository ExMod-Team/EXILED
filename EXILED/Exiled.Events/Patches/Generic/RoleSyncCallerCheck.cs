// -----------------------------------------------------------------------
// <copyright file="RoleSyncCallerCheck.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Handlers.Internal;
    using HarmonyLib;
    using PlayerRoles;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PlayerRoleManager.SendNewRoleInfo"/> to check if we can skip writing all the data for a fake role inside <see cref="Round.SendingNewRoleInfo"/> without looking inside the stack trace.
    /// </summary>
    [HarmonyPatch(typeof(PlayerRoleManager), nameof(PlayerRoleManager.SendNewRoleInfo))]
    internal class RoleSyncCallerCheck
    {
        private static void Prefix() => Round.SendingNewRoleInfo = true;

        private static void Postfix() => Round.SendingNewRoleInfo = false;
    }
}