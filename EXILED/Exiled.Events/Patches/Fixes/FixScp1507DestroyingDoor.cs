// -----------------------------------------------------------------------
// <copyright file="FixScp1507DestroyingDoor.cs" company="ExMod Team">
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
    using PlayerRoles.PlayableScps.Scp1507;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp1507AttackAbility.TryAttackDoor()"/> delegate.
    /// Fix than NW don't set the footprint argument <see cref="IDamageableDoor.ServerDamage(float, DoorDamageType, Footprint)"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp1507AttackAbility), nameof(Scp1507AttackAbility.TryAttackDoor))]
    internal class FixScp1507DestroyingDoor
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = -1;
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Initobj) + offset;

            newInstructions.RemoveRange(index, 3);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp1507AttackAbility), nameof(Scp1507AttackAbility.Owner))),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(Footprint))[0]),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
