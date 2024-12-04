// -----------------------------------------------------------------------
// <copyright file="BeingObserved.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp173
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp173;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp173;
    using PlayerRoles.Subroutines;
    using PluginAPI.Events;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp173ObserversTracker.IsObservedBy" />.
    /// Adds the <see cref="Handlers.Scp173.BeingObserved" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp173), nameof(Handlers.Scp173.BeingObserved))]
    [HarmonyPatch(typeof(Scp173ObserversTracker), nameof(Scp173ObserversTracker.IsObservedBy), typeof(ReferenceHub), typeof(float))]
    internal static class BeingObserved
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();

            const int offset = -4;
            MethodInfo scp173ExecuteEvent = typeof(EventManager)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m => m.Name == nameof(EventManager.ExecuteEvent)
                                     && !m.IsGenericMethod);
            int index = newInstructions.FindLastIndex(i => i.Calls(scp173ExecuteEvent)) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
            {
                // Player.Get(target)
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // Player.Get(base.Owner)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, PropertyGetter(typeof(StandardSubroutine<Scp173Role>), nameof(StandardSubroutine<Scp173Role>.Owner))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // true
                new(OpCodes.Ldc_I4_1),

                // BeingObservedEventArgs ev = new(Player, Player, bool)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(BeingObservedEventArgs))[0]),
                new(OpCodes.Dup),

                // Handlers.Scp173.OnBeingObserved(ev)
                new(OpCodes.Call, Method(typeof(Handlers.Scp173), nameof(Handlers.Scp173.OnBeingObserved))),

                // if (ev.IsAllowed)
                //   goto continueLabel
                new(OpCodes.Callvirt, PropertyGetter(typeof(BeingObservedEventArgs), nameof(BeingObservedEventArgs.IsAllowed))),
                new(OpCodes.Brtrue, continueLabel),

                // return false
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Ret),

                // continueLabel:
                new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
