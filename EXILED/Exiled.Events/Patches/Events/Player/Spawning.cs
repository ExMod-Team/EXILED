// -----------------------------------------------------------------------
// <copyright file="Spawning.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;

    using PlayerRoles.FirstPersonControl.Spawnpoints;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="RoleSpawnpointManager"/> delegate.
    /// Adds the <see cref="Handlers.Player.Spawning"/> event.
    /// Fix for spawning in void.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Spawning))]
    [HarmonyPatch(typeof(RoleSpawnpointManager), nameof(RoleSpawnpointManager.SetPosition))]
    internal static class Spawning
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = -1;

            // Locate the call to `Transform.position` setter to determine where to insert new instructions.
            int index = newInstructions.FindIndex(instr => instr.Calls(PropertySetter(typeof(Transform), nameof(Transform.position)))) + offset;

            // Declare the `SpawningEventArgs` local variable.
            LocalBuilder ev = generator.DeclareLocal(typeof(SpawningEventArgs));

            newInstructions.InsertRange(
            index,
            new[]
            {
                // Load `ReferenceHub` (argument 0) and get `Player`.
                new CodeInstruction(OpCodes.Ldarg_0), // Load `hub` (first argument passed to the method).
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })), // Call Player.Get(hub) to get the Player instance.

                // Load `position` (local variable 2).
                new CodeInstruction(OpCodes.Ldloc_2),

                // Load `rotation` (local variable 3).
                new CodeInstruction(OpCodes.Ldloc_3),

                // Load `newRole` (argument 1).
                new CodeInstruction(OpCodes.Ldarg_1), // Load `newRole` from argument 1.

                // Create a new instance of `SpawningEventArgs`.
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawningEventArgs))[0]),

                // Duplicate the object to store it and pass it around.
                new CodeInstruction(OpCodes.Dup), // Duplicate the `SpawningEventArgs` object.
                new CodeInstruction(OpCodes.Stloc, ev.LocalIndex), // Store the duplicated object in a local variable.

                // Call `Handlers.Player.OnSpawning`.
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnSpawning))),

                // Modify `position` from `ev.Position`.
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex), // Load the `SpawningEventArgs` object stored in the local variable.
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(SpawningEventArgs), nameof(SpawningEventArgs.Position))), // Get the `Position` property from `SpawningEventArgs`.
                new CodeInstruction(OpCodes.Stloc_2), // Store the position value back in the local variable 2 (`position`).

                // Modify `rotation` from `ev.HorizontalRotation`.
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex), // Load the `SpawningEventArgs` object again.
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(SpawningEventArgs), nameof(SpawningEventArgs.HorizontalRotation))), // Get the `HorizontalRotation` property from `SpawningEventArgs`.
                new CodeInstruction(OpCodes.Stloc_3), // Store the rotation value back in the local variable 3 (`rotation`).
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
