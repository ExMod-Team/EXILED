// -----------------------------------------------------------------------
// <copyright file="Punching.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Item
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Item;
    using HarmonyLib;
    using InventorySystem.Items.MarshmallowMan;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch the <see cref="MarshmallowItem.ServerProcessCmd" />.
    /// Adds the <see cref="Handlers.Item.Punching" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Item), nameof(Handlers.Item.Punching))]
    [HarmonyPatch(typeof(MarshmallowItem), nameof(MarshmallowItem.ServerProcessCmd))]
    public class Punching
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) + offset;

            newInstructions[^1].WithLabels(retLabel);

            newInstructions.InsertRange(index, new[]
            {
                // this;
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),

                // true
                new(OpCodes.Ldc_I4_1),

                // ev = new PunchingEventArgs(MarshmallowItem, bool);
                new(OpCodes.Newobj, Constructor(typeof(PunchingEventArgs), new[] { typeof(MarshmallowItem), typeof(bool) })),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnPunching))),

                // if (!ev.IsAllowed) return
                new(OpCodes.Callvirt, PropertyGetter(typeof(PunchingEventArgs), nameof(PunchingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}