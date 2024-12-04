// -----------------------------------------------------------------------
// <copyright file="Shooting.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;

    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.BasicMessages;
    using InventorySystem.Items.Firearms.Modules;
    using InventorySystem.Items.Firearms.Modules.Misc;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Player" />.
    /// Adds the <see cref="Handlers.Player.Shooting" /> events.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Shooting))]

    [HarmonyPatch(typeof(ShotBacktrackData), nameof(ShotBacktrackData.ProcessShot))]
    internal static class Shooting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_2) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(firearm.Owner)
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Firearm), nameof(Firearm.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // firearm
                    new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(newInstructions[index]),

                    // ShootingEventArgs ev = new(Player, firearm)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ShootingEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnShooting(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnShooting))),

                    // if (ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ShootingEventArgs), nameof(ShootingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}