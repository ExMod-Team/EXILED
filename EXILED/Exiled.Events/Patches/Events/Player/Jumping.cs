// -----------------------------------------------------------------------
// <copyright file="Jumping.cs" company="ExMod Team">
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

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="FpcMotor.UpdateGrounded(ref bool, float)" />
    /// Adds the <see cref="Player.Jumping" /> event.
    /// </summary>
    [EventPatch(typeof(Player), nameof(Player.Jumping))]
    [HarmonyPatch(typeof(FpcMotor), nameof(FpcMotor.UpdateGrounded))]
    internal static class Jumping
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(JumpingEventArgs));
            LocalBuilder jumping = generator.DeclareLocal(typeof(bool));

            Label cont = generator.DefineLabel();
            Label cancel = generator.DefineLabel();

            // fun fact, the "int num = ProcessJump() ? 1 : 0;" in target method doesn't actually store anything, the bool result from ProcessJump is simply kept on stack.
            // our patch needs to know when player is jumping so we modify method to store that.
            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(FpcJumpController), nameof(FpcJumpController.ProcessJump)))) + offset;

            // after ProcessJump, store its result
            newInstructions.Insert(index, new CodeInstruction(OpCodes.Stloc, jumping));

            offset = 1;
            index = newInstructions.FindIndex(instruction => instruction.StoresField(Field(typeof(FpcMotor), nameof(FpcMotor._maxFallSpeed)))) + offset;

            // make br_false use stored value
            newInstructions.Insert(index, new CodeInstruction(OpCodes.Ldloc, jumping));

            // The FindIndex finds when storing the field inside the moveDirection vector, so our patch occurs right before "this.MoveDirection = moveDirection;"
            offset = 1;
            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stfld) + offset;

            newInstructions[index].WithLabels(cont);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // if not jumping, skip Jumping event
                    new(OpCodes.Ldloc, jumping),
                    new(OpCodes.Brfalse, cont),

                    // Player.Get(this.Hub)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(FpcMotor), nameof(FpcMotor.Hub))),
                    new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                    // moveDir
                    new(OpCodes.Ldloc_0),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // JumpingEventArgs ev = new(Player, Vector3, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(JumpingEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev),

                    // Player.OnJumping(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnJumping))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(JumpingEventArgs), nameof(JumpingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, cancel),

                    // moveDir = ev.Direction
                    new(OpCodes.Ldloc, ev),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(JumpingEventArgs), nameof(JumpingEventArgs.Direction))),
                    new(OpCodes.Stloc_0),
                    new(OpCodes.Br, cont),

                    new CodeInstruction(OpCodes.Ldc_I4_0).WithLabels(cancel),
                    new(OpCodes.Stloc, jumping),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}