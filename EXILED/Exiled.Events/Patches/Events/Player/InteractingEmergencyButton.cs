// -----------------------------------------------------------------------
// <copyright file="InteractingEmergencyButton.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Doors;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using Interactables.Interobjects;
    using Interactables.Interobjects.DoorUtils;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="EmergencyDoorRelease.ServerInteract"/>
    /// to add <see cref="Handlers.Player.InteractingEmergencyButton"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.InteractingEmergencyButton))]
    [HarmonyPatch(typeof(EmergencyDoorRelease), nameof(EmergencyDoorRelease.ServerInteract), typeof(ReferenceHub), typeof(byte))]
    internal class InteractingEmergencyButton
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label transpilerLabel = generator.DefineLabel();

            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Brtrue_S) + 2;
            Label continueLabel = (Label)newInstructions[index - 2].operand;
            newInstructions[index - 2].operand = transpilerLabel;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(transpilerLabel),

                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(EmergencyDoorRelease), nameof(EmergencyDoorRelease._controlledDoor))),
                new(OpCodes.Call, typeof(Door).GetMethods().Single(method => method.Name is "Get" && !method.IsGenericMethod && method.GetParameters().All(parameter => parameter.ParameterType == typeof(DoorVariant)))),

                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InteractingEmergencyButtonEventArgs))[0]),
                new(OpCodes.Dup),

                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnInteractingEmergencyButton))),

                new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingEmergencyButtonEventArgs), nameof(InteractingEmergencyButtonEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, continueLabel),
                new(OpCodes.Ret),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}