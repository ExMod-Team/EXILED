// -----------------------------------------------------------------------
// <copyright file="Status.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp1344
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp1344;
    using HarmonyLib;

    using InventorySystem.Items.Usables.Scp1344;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp1344Item.Status"/>.
    /// Adds the <see cref="Handlers.Scp1344.ChangingStatus" /> event and
    /// <see cref="Handlers.Scp1344.ChangedStatus" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp1344), nameof(Handlers.Scp1344.ChangingStatus))]
    [EventPatch(typeof(Handlers.Scp1344), nameof(Handlers.Scp1344.ChangedStatus))]
    [HarmonyPatch(typeof(Scp1344Item), nameof(Scp1344Item.Status), MethodType.Setter)]
    internal static class Status
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            // Declare local variable for ChangingStatusEventArgs
            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingStatusEventArgs));

            // Continue label for isAllowed check
            Label continueLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                    // this.Scp1344Item
                    new(OpCodes.Ldarg_0),

                    // value to be set
                    new(OpCodes.Ldarg_1),

                    // this._status
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp1344Item), nameof(Scp1344Item._status))),

                     // true (IsAllowed)
                    new(OpCodes.Ldc_I4_1),

                    // ChangingStatusEventArgs ev = new(this.Item, value, this._status, isallowed)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingStatusEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Scp1344.OnChangingStatus(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Scp1344), nameof(Handlers.Scp1344.OnChangingStatus))),

                    // if (!ev.IsAllowed) return;
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingStatusEventArgs), nameof(ChangingStatusEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueLabel),

                    // Return;
                    new(OpCodes.Ret),

                    // continue label
                    new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),

                    // value = ev.Scp1344StatusNew;
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingStatusEventArgs), nameof(ChangingStatusEventArgs.Scp1344StatusNew))),
                    new(OpCodes.Starg_S, 1),
            });

            // this.ServerChangeStatus(value) index
            MethodInfo changeStatusMethod = Method(typeof(Scp1344Item), nameof(Scp1344Item.ServerChangeStatus));
            int offset = 1;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Call && i.operand is MethodInfo method && method == changeStatusMethod) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                    // this.Scp1344Item
                    new(OpCodes.Ldarg_0),

                    // value to be set
                    new(OpCodes.Ldarg_1),

                    // ChangedStatusEventArgs ev = new(this.Item, value)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangedStatusEventArgs))[0]),

                    // Handlers.Scp1344.OnChangedStatus(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Scp1344), nameof(Handlers.Scp1344.OnChangedStatus))),
            });

            // Return the new instructions
            foreach (CodeInstruction newcode in newInstructions)
                yield return newcode;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
