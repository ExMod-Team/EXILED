// -----------------------------------------------------------------------
// <copyright file="AttackingDoor.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp1507
{
#pragma warning disable CS0618
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp1507;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp1507;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp1507AttackAbility.TryAttackDoor"/>
    /// to add <see cref="Handlers.Scp1507.AttackingDoor"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp1507), nameof(Handlers.Scp1507.AttackingDoor))]
    [HarmonyPatch(typeof(Scp1507AttackAbility), nameof(Scp1507AttackAbility.TryAttackDoor))]
    internal class AttackingDoor
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = -4;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldloc_S) + offset;

            Label continueLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(this.Owner);
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp1507AttackAbility), nameof(Scp1507AttackAbility.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // target1
                    new(OpCodes.Ldloc_3),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // AttackingDoorEventArgs ev = new(Player, DoorVariant, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AttackingDoorEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Scp1507.OnAttackingDoor(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Scp1507), nameof(Handlers.Scp1507.OnAttackingDoor))),

                    // if (!ev.IsAllowed)
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AttackingDoorEventArgs), nameof(AttackingDoorEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueLabel),

                    // return false;
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Ret),

                    new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}