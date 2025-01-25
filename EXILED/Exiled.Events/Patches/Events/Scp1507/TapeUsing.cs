// -----------------------------------------------------------------------
// <copyright file="TapeUsing.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp1507
{
#pragma warning disable CS0618

    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp1507;
    using HarmonyLib;
    using InventorySystem.Items.FlamingoTapePlayer;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="TapeItem.ServerProcessCmd"/>
    /// to add <see cref="Handlers.Scp1507.UsingTape"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp1507), nameof(Handlers.Scp1507.UsingTape))]
    [HarmonyPatch(typeof(TapeItem), nameof(TapeItem.ServerProcessCmd))]
    public class TapeUsing
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = 1;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ret) + offset;

            Label retLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                // this;
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),

                // true
                new(OpCodes.Ldc_I4_1),

                // TapeUsingEventArgs ev = new(Player, true);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UsingTapeEventArgs))[0]),
                new(OpCodes.Dup),

                // Handlers.Scp1507.OnUsingTape(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Scp1507), nameof(Handlers.Scp1507.OnUsingTape))),

                // if (!ev.IsAllowed)
                //    goto retLabel;
                new(OpCodes.Callvirt, PropertyGetter(typeof(UsingTapeEventArgs), nameof(UsingTapeEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}