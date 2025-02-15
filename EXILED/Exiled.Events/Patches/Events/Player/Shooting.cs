// -----------------------------------------------------------------------
// <copyright file="Shooting.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
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

            /*
            [] <= Here
            IL_0078: ldarg.2      // processingMethod
            IL_0079: ldarg.0      // this
            IL_007a: ldfld        class ReferenceHub InventorySystem.Items.Firearms.Modules.Misc.ShotBacktrackData::PrimaryTargetHub
            IL_007f: callvirt     instance void class [mscorlib]System.Action`1<class ReferenceHub>::Invoke(!0/*class ReferenceHub* /)
             */
            int hasTargetIndex = newInstructions.FindIndex(instruction => instruction.IsLdarg(2));

            /*
            [] <= Here
            IL_0092: ldarg.2      // processingMethod
            IL_0093: ldnull       // null
            IL_0094: callvirt     instance void class [mscorlib]System.Action`1<class ReferenceHub>::Invoke(!0/*class ReferenceHub* /)
             */
            int noTargetIndex = newInstructions.FindIndex(hasTargetIndex + 1, instruction => instruction.IsLdarg(2));
            List<Label> noTargetLabels = newInstructions[noTargetIndex].ExtractLabels();

            ConstructorInfo constructorInfo = Constructor(
                typeof(ShootingEventArgs),
                new[] { typeof(InventorySystem.Items.Firearms.Firearm), typeof(ShotBacktrackData).MakeByRefType() });

            Label continueLabel1 = generator.DefineLabel();

            CodeInstruction[] noTargetInstructions =
            {
                // ShootingEventArgs ev = new(firearm, this)
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Newobj, constructorInfo),

                // Handlers.Player.OnShooting(ev)
                new(OpCodes.Dup), // Dup to keep ev on the stack
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnShooting))),

                // if (!ev.IsAllowed) return
                new(OpCodes.Callvirt, PropertyGetter(typeof(ShootingEventArgs), nameof(ShootingEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, continueLabel1),

                new(OpCodes.Leave_S, returnLabel),
                new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel1),
            };

            Label continueLabel2 = generator.DefineLabel();

            CodeInstruction[] hasTargetInstructions =
            {
                // ShootingEventArgs ev = new(firearm, this)
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Newobj, constructorInfo),

                // Handlers.Player.OnShooting(ev)
                new(OpCodes.Dup), // Dup to keep ev on the stack
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnShooting))),

                // if (!ev.IsAllowed) return
                new(OpCodes.Callvirt, PropertyGetter(typeof(ShootingEventArgs), nameof(ShootingEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, continueLabel2),

                // Dispose target FpcBacktracker
                new(OpCodes.Ldloc_S, 5),
                new(OpCodes.Callvirt, Method(typeof(IDisposable), nameof(IDisposable.Dispose))),

                new(OpCodes.Leave_S, returnLabel),
                new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel2),
            };

            newInstructions.InsertRange( // noTargetIndex goes first because it's higher then hasTargetIndex so it won't mess it up
                noTargetIndex,
                noTargetInstructions);
            newInstructions[noTargetIndex].WithLabels(noTargetLabels);

            newInstructions.InsertRange(
                hasTargetIndex,
                hasTargetInstructions);

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}