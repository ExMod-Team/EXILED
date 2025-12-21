// -----------------------------------------------------------------------
// <copyright file="Cackling.cs" company="ExMod Team">
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
    /// Adds the <see cref="Handlers.Item.Cackling" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Item), nameof(Handlers.Item.Cackling))]
    [HarmonyPatch(typeof(MarshmallowItem), nameof(MarshmallowItem.ServerProcessCackle))]
    public class Cackling
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // this;
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),

                // true
                new(OpCodes.Ldc_I4_1),

                // ev = new CacklingEventArgs(MarshmallowItem, bool);
                new(OpCodes.Newobj, Constructor(typeof(CacklingEventArgs), new[] { typeof(MarshmallowItem), typeof(bool) })),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnCackling))),

                // if (!ev.IsAllowed) return
                new(OpCodes.Callvirt, PropertyGetter(typeof(CacklingEventArgs), nameof(CacklingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}