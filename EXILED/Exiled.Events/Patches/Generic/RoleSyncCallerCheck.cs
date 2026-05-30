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
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            yield return new CodeInstruction(OpCodes.Ldc_I4_1);
            yield return new CodeInstruction(OpCodes.Call, PropertySetter(typeof(Round), nameof(Round.SendingNewRoleInfo)));

            int z;
            for (z = 0; z < newInstructions.Count - 1; z++)
                yield return newInstructions[z];
            CodeInstruction ret = newInstructions[z];

            yield return new CodeInstruction(OpCodes.Ldc_I4_0).MoveLabelsFrom(ret);
            yield return new CodeInstruction(OpCodes.Call, PropertySetter(typeof(Round), nameof(Round.SendingNewRoleInfo)));

            yield return ret;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}