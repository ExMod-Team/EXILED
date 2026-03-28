// -----------------------------------------------------------------------
// <copyright file="FoundPosition.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp2536
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Christmas.Scp2536;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp2536;
    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp2536Controller.ServerFindTarget"/>
    /// to add <see cref="Handlers.Scp2536.FoundPosition"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp2536), nameof(Handlers.Scp2536.FoundPosition))]
    [HarmonyPatch(typeof(Scp2536Controller), nameof(Scp2536Controller.ServerFindTarget))]
    public class FoundPosition
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();

            int index = newInstructions.Count - 1;

            newInstructions[index].WithLabels(retLabel);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // dupe the result (true / false if found)
                new(OpCodes.Dup),

                // return w result if failed
                new(OpCodes.Brfalse_S, retLabel),
                new(OpCodes.Pop),

                // load ADDRESS of parameter "out Scp2536Spawnpoint spawnpoint" onto stack (so not actual value but address)
                // this is used at end of transpiler
                new(OpCodes.Ldarg_2),

                // load address of parameter "out ReferenceHub target"
                new(OpCodes.Ldarg_1),

                // load value at address
                new(OpCodes.Ldind_Ref),

                // player = Player.Get(target);
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // load address of parameter "out Scp2536Spawnpoint spawnpoint"
                new(OpCodes.Ldarg_2),

                // load value at address
                new(OpCodes.Ldind_Ref),

                // ev = new FoundPositionEventArgs(player, spawnpoint);
                new(OpCodes.Newobj, Constructor(typeof(FoundPositionEventArgs), new[] { typeof(Player), typeof(Scp2536Spawnpoint) })),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.Scp2536), nameof(Handlers.Scp2536.OnFoundPosition))),

                // spawnpoint = ev.Spawnpoint;
                new(OpCodes.Callvirt, PropertyGetter(typeof(FoundPositionEventArgs), nameof(FoundPositionEventArgs.Spawnpoint))),

                // this uses the address already on stack (see 3rd comment) with the value from the event args
                new(OpCodes.Stind_Ref),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}