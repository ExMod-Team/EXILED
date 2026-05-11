// -----------------------------------------------------------------------
// <copyright file="RoleChangedPatch.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using EventArgs.Player;

    using Exiled.API.Features.Items;

    using HarmonyLib;

    using InventorySystem;

    /// <summary>
    /// Patches <see cref="InventoryItemProvider.RoleChanged"/> to help override in <see cref="ChangingRoleEventArgs.Items"/> and <see cref="Ammo"/>.
    /// </summary>
    [HarmonyPatch(typeof(InventoryItemProvider), nameof(InventoryItemProvider.RoleChanged))]
    internal static class RoleChangedPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            _ = instructions;
            yield return new CodeInstruction(OpCodes.Ret);
        }
    }
}