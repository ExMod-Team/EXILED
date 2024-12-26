// -----------------------------------------------------------------------
// <copyright file="WarheadConfigLockGateFix.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Footprinting;
    using HarmonyLib;
    using Interactables.Interobjects.DoorUtils;
    using InventorySystem;
    using InventorySystem.Items.Firearms.Ammo;
    using InventorySystem.Items.Pickups;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="DoorEventOpenerExtension.Trigger"/> delegate.
    /// Fix than NW config "lock_gates_on_countdown"
    /// reported https://git.scpslgame.com/northwood-qa/scpsl-bug-reporting/-/issues/316.
    /// </summary>
    [HarmonyPatch(typeof(DoorEventOpenerExtension), nameof(DoorEventOpenerExtension.Trigger))]
    internal class WarheadConfigLockGateFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            // replace Contains with StartWith
            int index = newInstructions.FindIndex(x => x.operand == (object)Method(typeof(string), nameof(string.Contains), new[] { typeof(string) }));
            newInstructions[index].operand = Method(typeof(string), nameof(string.StartsWith), new System.Type[] { typeof(string) });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
