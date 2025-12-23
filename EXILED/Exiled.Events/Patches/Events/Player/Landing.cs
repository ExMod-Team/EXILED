// -----------------------------------------------------------------------
// <copyright file="Landing.cs" company="ExMod Team">
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
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.FirstPersonControl.Thirdperson;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="FirstPersonMovementModule.IsGrounded" />
    /// Adds the <see cref="Player.Landing" /> event.
    /// </summary>
    [EventPatch(typeof(Player), nameof(Player.Landing))]
    [HarmonyPatch(typeof(FirstPersonMovementModule), nameof(FirstPersonMovementModule.IsGrounded), MethodType.Setter)]
    internal static class Landing
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = 2;
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldarg_1) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(this.Hub)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(FirstPersonMovementModule), nameof(FirstPersonMovementModule.Hub))),
                    new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                    // LandingEventArgs ev = new(Player)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(LandingEventArgs))[0]),

                    // Player.OnLanding(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnLanding))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}