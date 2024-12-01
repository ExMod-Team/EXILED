// -----------------------------------------------------------------------
// <copyright file="Shot.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using InventorySystem.Items.Firearms.Modules;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="HitscanHitregModuleBase.ServerPerformHitscan" />.
    /// Adds the <see cref="Handlers.Player.Shot" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Shot))]
    [HarmonyPatch(typeof(HitscanHitregModuleBase), nameof(HitscanHitregModuleBase.ServerPerformHitscan))]
    internal static class Shot
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            int offset = 3;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldarg_2) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // instance
                    new(OpCodes.Ldarg_0),

                    // this.Firearm
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(HitscanHitregModuleBase), nameof(HitscanHitregModuleBase.Firearm))),

                    // ray
                    new(OpCodes.Ldarg_1),

                    // maxDistance
                    new(OpCodes.Ldloc_0),

                    // ShotEventArgs ev = new(HitscanHitregModuleBase, Firearm, Ray, float)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ShotEventArgs))[0]),

                    // Handlers.Player.OnShot(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnShot))),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
