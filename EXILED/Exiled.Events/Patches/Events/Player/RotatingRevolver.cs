// -----------------------------------------------------------------------
// <copyright file="RotatingRevolver.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Extensions;
    using Exiled.API.Features.Pools;

    using Exiled.Events.Attributes;

    using Exiled.Events.EventArgs.Player;

    using Exiled.Events.Handlers;

    using HarmonyLib;
    using InventorySystem.Items.Firearms.Modules;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="CylinderAmmoModule.RotateCylinder(int)"/>
    /// to add <see cref="Player.RotatingRevolver"/> event.
    /// </summary>
    [EventPatch(typeof(Player), nameof(Player.RotatingRevolver))]
    [HarmonyPatch(typeof(CylinderAmmoModule), nameof(CylinderAmmoModule.RotateCylinder))]
    internal class RotatingRevolver
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = -2;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Rem) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // this.Firearm;
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(CylinderAmmoModule), nameof(CylinderAmmoModule.Firearm))),

                    // rotations
                    new(OpCodes.Ldarg_1),

                    // RotatingRevolverEventArgs ev = new(firearm, rotations);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RotatingRevolverEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnRotatingRevolver(ev);
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnRotatingRevolver))),

                    // rotations = ev.Rotations
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RotatingRevolverEventArgs), nameof(RotatingRevolverEventArgs.Rotations))),
                    new(OpCodes.Starg_S, 1),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}