// -----------------------------------------------------------------------
// <copyright file="Spawning.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using MEC;

    using PlayerRoles.FirstPersonControl;
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
        private static void ApplyRotation(IFpcRole fpcRole, SpawningEventArgs ev)
        {
            if (!ev.IsRotationModified || fpcRole == null || fpcRole.FpcModule == null)
                return;

            Timing.RunCoroutine(WaitForMouseLookAndApply(fpcRole.FpcModule, ev.SpawningRotation), fpcRole.FpcModule.gameObject);
        }

        private static IEnumerator<float> WaitForMouseLookAndApply(FirstPersonMovementModule fpcModule, Vector2 rotation)
        {
            yield return Timing.WaitUntilFalse(() => fpcModule != null && fpcModule.MouseLook == null);

            if (fpcModule == null)
                yield break;

            yield return Timing.WaitForOneFrame;
            fpcModule.ServerOverrideRotation(rotation);
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(SpawningEventArgs));

            int offset = -1;
            int index = newInstructions.FindIndex(instr => instr.Calls(PropertySetter(typeof(Transform), nameof(Transform.position)))) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Player.Get(hub);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // spawnLocation
                new(OpCodes.Ldloc_2),

                // horizontalRotation
                new(OpCodes.Ldloc_3),

                // newRole
                new(OpCodes.Ldarg_1),

                // SpawningEventArg ev = new SpawningEventArgs(player, spawnLocation, horizontalRotation, newRole);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawningEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, ev.LocalIndex),

                // Handlers.Player.OnSpawning(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnSpawning))),

                // spawnLocation = ev.Position;
                new(OpCodes.Ldloc, ev.LocalIndex),
                new(OpCodes.Call, PropertyGetter(typeof(SpawningEventArgs), nameof(SpawningEventArgs.Position))),
                new(OpCodes.Stloc_2),

                // horizontalRotation = ev.SpawningRotation.y;
                new(OpCodes.Ldloc, ev.LocalIndex),
                new(OpCodes.Call, PropertyGetter(typeof(SpawningEventArgs), nameof(SpawningEventArgs.SpawningRotation))),
                new(OpCodes.Ldfld, Field(typeof(Vector2), nameof(Vector2.y))),
                new(OpCodes.Stloc_3),

                // ApplyRotation(fpcRole, ev.SpawningRotation)
                new(OpCodes.Ldloc_0),
                new(OpCodes.Ldloc, ev.LocalIndex),
                new(OpCodes.Call, Method(typeof(Spawning), nameof(ApplyRotation))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
