// -----------------------------------------------------------------------
// <copyright file="Interacted.cs" company="ExMod Team">
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
    using Interactables;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="InteractionCoordinator.ClientInteract" />.
    /// Adds the <see cref="Handlers.Player.Interacted" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Interacted))]
    [HarmonyPatch(typeof(InteractionCoordinator), nameof(InteractionCoordinator.ClientInteract))]
    internal static class Interacted
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // Handlers.Player.OnInteracted(new InteractedEventArgs(Player.Get(this.gameObject)));
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(InteractionCoordinator), nameof(InteractionCoordinator.gameObject))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InteractedEventArgs))[0]),
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnInteracted))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}