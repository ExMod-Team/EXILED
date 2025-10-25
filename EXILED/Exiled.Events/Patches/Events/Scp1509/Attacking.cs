// -----------------------------------------------------------------------
// <copyright file="Attacking.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp1509
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp1509;
    using HarmonyLib;
    using InventorySystem.Items.Scp1509;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp1509Hitreg.ServerAttack"/>
    /// to add <see cref="Handlers.Scp1509.Attacking"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp1509), nameof(Handlers.Scp1509.Attacking))]
    [HarmonyPatch(typeof(Scp1509Hitreg), nameof(Scp1509Hitreg.ServerAttack))]
    internal class Attacking
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = 1;
            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Stloc_1) + offset;

            LocalBuilder ev = generator.DeclareLocal(typeof(AttackingEventArgs));

            Label skipLabel = generator.DefineLabel();

            newInstructions[index].labels.Add(skipLabel);

            offset = -1;
            index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldloc_S && x.operand is LocalBuilder { LocalIndex: 12 }) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(owner)
                new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // detectedDestructible
                new(OpCodes.Ldloc_S, 11),

                // damage
                new(OpCodes.Ldloc_S, 12),

                // this._item
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Scp1509Hitreg), nameof(Scp1509Hitreg._item))),

                // true
                new(OpCodes.Ldc_I4_1),

                // AttackingEventArgs ev = new(Player.Get(owner), detectedDestructible, damage, this, true);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AttackingEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Scp1509.OnAttacking(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Scp1509), nameof(Handlers.Scp1509.OnAttacking))),

                // if (!ev.IsAllowed)
                //    continue;
                new(OpCodes.Callvirt, PropertyGetter(typeof(AttackingEventArgs), nameof(AttackingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, skipLabel),

                // damage = ev.Damage;
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(AttackingEventArgs), nameof(AttackingEventArgs.Damage))),
                new(OpCodes.Stloc_S, 12),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}