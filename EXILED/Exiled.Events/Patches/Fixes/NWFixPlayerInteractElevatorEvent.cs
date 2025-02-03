// -----------------------------------------------------------------------
// <copyright file="NWFixPlayerInteractElevatorEvent.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1402 // File may only contain a single type
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using API.Features.Pools;

    using HarmonyLib;

    using Interactables.Interobjects;
    using PluginAPI.Events;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ElevatorDoor.ServerInteract" />.
    /// Triggers the <see cref="PlayerInteractElevatorEvent" /> event.
    /// Bug reported to NW (https://git.scpslgame.com/northwood-qa/scpsl-bug-reporting/-/issues/895).
    /// <remarks>This event only get call when interact on the outside button.</remarks>
    /// </summary>
    [HarmonyPatch(typeof(ElevatorDoor), nameof(ElevatorDoor.ServerInteract))]
    internal class NWFixPlayerInteractElevatorEvent
    {
        /** Transpiler patch **/
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            // PlayerInteractElevatorEvent ev = new(ReferenceHub, ElevatorChamber)
            // if (!ExecuteEvent(ev)) return;
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            MethodInfo executeEventMethod = typeof(EventManager)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m => m.Name == nameof(EventManager.ExecuteEvent)
                        && m.GetParameters().Length == 1
                        && m.ReturnType == typeof(bool));

            newInstructions.InsertRange(newInstructions.Count - 1, new CodeInstruction[]
                {
                // ReferenceHub
                new(OpCodes.Ldarg_1),

                // this.Chamber
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(ElevatorDoor), nameof(ElevatorDoor.Chamber))),

                // PlayerInteractElevatorEvent ev = new(ReferenceHub, ElevatorChamber)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PlayerInteractElevatorEvent))[0]),

                // if (!ExecuteEvent(ev)) return;
                new(OpCodes.Call, executeEventMethod),
                new(OpCodes.Brfalse_S, returnLabel),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="ElevatorChamber.ServerInteract" />.
    /// Adds the <see cref="PlayerInteractElevatorEvent" /> event.
    /// Bug reported to NW (https://git.scpslgame.com/northwood-qa/scpsl-bug-reporting/-/issues/895).
    /// <remarks>This event only get call when interact on the inside button.</remarks>
    /// </summary>
    [HarmonyPatch(typeof(ElevatorChamber), nameof(ElevatorChamber.ServerInteract))]
    internal class PlayerInteractElevatorEventFix2
    {
        /** Transpiler patch **/
        // PlayerInteractElevatorEvent ev = new(ReferenceHub, ElevatorChamber)
        // if (!ExecuteEvent(ev)) return;
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            MethodInfo executeEventMethod = typeof(EventManager)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m => m.Name == nameof(EventManager.ExecuteEvent)
                        && m.GetParameters().Length == 1
                        && m.ReturnType == typeof(bool));

            newInstructions.InsertRange(newInstructions.Count - 1, new CodeInstruction[]
                {
                    // ReferenceHub
                    new(OpCodes.Ldarg_1),

                    // elevatorChamber
                    new(OpCodes.Ldarg_0),

                    // PlayerInteractElevatorEvent ev = new(ReferenceHub, ElevatorChamber)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PlayerInteractElevatorEvent))[0]),

                    // if (!ExecuteEvent(ev)) return;
                    new(OpCodes.Call, executeEventMethod),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}