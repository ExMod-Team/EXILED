// -----------------------------------------------------------------------
// <copyright file="InspectingAndTriggeringAttack.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp1509
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp1509;
    using HarmonyLib;
    using InventorySystem.Items.Scp1509;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp1509Item.ServerProcessCmd"/>
    /// to add <see cref="Handlers.Scp1509.Inspecting"/> and <see cref="Handlers.Scp1509.TriggeringAttack"/> events.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp1509), nameof(Handlers.Scp1509.Inspecting))]
    [EventPatch(typeof(Handlers.Scp1509), nameof(Handlers.Scp1509.TriggeringAttack))]
    [HarmonyPatch(typeof(Scp1509Item), nameof(Scp1509Item.ServerProcessCmd))]
    internal class InspectingAndTriggeringAttack
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = -2;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Stfld) + offset;

            Label retLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                // this
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),

                // true
                new(OpCodes.Ldc_I4_1),

                // TriggeringAttackEventArgs ev = new(this, true);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(TriggeringAttackEventArgs))[0]),
                new(OpCodes.Dup),

                // Handlers.Scp1509.OnTriggeringAttack(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Scp1509), nameof(Handlers.Scp1509.OnTriggeringAttack))),

                // if (!ev.IsAllowed)
                //     return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(TriggeringAttackEventArgs), nameof(TriggeringAttackEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),
            });

            index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldarg_0);

            newInstructions.InsertRange(index, new[]
            {
                // this
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),

                // true
                new(OpCodes.Ldc_I4_1),

                // InspectingEventArgs ev = new(this, true);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InspectingEventArgs))[0]),
                new(OpCodes.Dup),

                // Handlers.Scp1509.OnInspecting(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Scp1509), nameof(Handlers.Scp1509.OnInspecting))),

                // if (!ev.IsAllowed)
                //     return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(InspectingEventArgs), nameof(InspectingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}