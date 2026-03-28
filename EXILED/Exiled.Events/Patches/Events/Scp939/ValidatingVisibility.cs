// -----------------------------------------------------------------------
// <copyright file="ValidatingVisibility.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp939
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;

    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp939;
    using HarmonyLib;
    using InventorySystem.Items;
    using Mirror;

    using PlayerRoles.PlayableScps.Scp939;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp939VisibilityController.ValidateVisibility(ReferenceHub)" />
    ///     to add the <see cref="ValidatingVisibility" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp939), nameof(Handlers.Scp939.ValidatingVisibility))]
    [HarmonyPatch(typeof(Scp939VisibilityController), nameof(Scp939VisibilityController.ValidateVisibility))]
    internal class ValidatingVisibility
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(ValidatingVisibilityEventArgs));

            Label ret = generator.DefineLabel();
            Label end = generator.DefineLabel();

            int offset = 0;
            int index = newInstructions.FindIndex(i => i.LoadsConstant(0)) + offset;

            newInstructions[index].labels.Add(ret);

            newInstructions.InsertRange(index, StaticCallEvent(generator, ev, ret, newInstructions[index], Scp939VisibilityState.None, false));

            offset = 0;
            index = newInstructions.FindIndex(i => i.LoadsConstant(1)) + offset;

            newInstructions.InsertRange(index, StaticCallEvent(generator, ev, ret, newInstructions[index], Scp939VisibilityState.SeenAsScp));

            offset = 2;
            index = newInstructions.FindIndex(i => i.Calls(PropertyGetter(typeof(AlphaWarheadController), nameof(AlphaWarheadController.Detonated)))) + offset;

            newInstructions.InsertRange(index, StaticCallEvent(generator, ev, ret, newInstructions[index], Scp939VisibilityState.SeenByDetonation));

            offset = 0;
            index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ldloc_3) + offset;

            // just pre-check for SeenByLastTime or NotSeen VisibilityState, and then il inject
            newInstructions.InsertRange(index, Enumerable.Concat(
                new CodeInstruction[]
                {
                    new CodeInstruction(OpCodes.Ldc_I4, (int)Scp939VisibilityState.NotSeen).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Ldloc_3),
                    new(OpCodes.Brfalse_S, end),
                    new(OpCodes.Pop),
                    new(OpCodes.Ldc_I4, (int)Scp939VisibilityState.SeenByLastTime),
                    new CodeInstruction(OpCodes.Nop).WithLabels(end),
                },
                CallEvent(generator, ev, ret)));

            offset = 0;
            index = newInstructions.FindLastIndex(i => i.LoadsField(Field(typeof(Scp939VisibilityController), nameof(Scp939VisibilityController.LastSeen)))) + offset;

            newInstructions.InsertRange(index, StaticCallEvent(generator, ev, ret, newInstructions[index], Scp939VisibilityState.SeenByRange));

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        // helper method for injecting instructions
        private static IEnumerable<CodeInstruction> StaticCallEvent(ILGenerator generator, LocalBuilder ev, Label ret, CodeInstruction insertInstuction, Scp939VisibilityState state, bool setLabel = true)
        {
            CodeInstruction first = new CodeInstruction(OpCodes.Ldc_I4, (int)state);

            if (setLabel)
            {
                first.labels.AddRange(insertInstuction.ExtractLabels());
            }

            yield return first;

            foreach (CodeInstruction z in CallEvent(generator, ev, ret))
            {
                yield return z;
            }
        }

        // mail il logic
        private static IEnumerable<CodeInstruction> CallEvent(ILGenerator generator, LocalBuilder ev, Label ret)
        {
            Label cnt = generator.DefineLabel();

            // ...VisibilityState loaded in stack
            // ValidatingVisibilityEventArgs ev = new(state, scp939, target)
            yield return new(OpCodes.Ldarg_0);
            yield return new(OpCodes.Callvirt, PropertyGetter(typeof(Scp939VisibilityController), nameof(Scp939VisibilityController.Owner)));
            yield return new(OpCodes.Ldarg_1);
            yield return new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ValidatingVisibilityEventArgs))[0]);
            yield return new(OpCodes.Dup);
            yield return new(OpCodes.Dup);
            yield return new(OpCodes.Stloc_S, ev.LocalIndex);

            // Scp939.OnValidatingVisibility(ev)
            // if (!ev.IsAllowed)
            //     return false;
            yield return new(OpCodes.Call, Method(typeof(Handlers.Scp939), nameof(Handlers.Scp939.OnValidatingVisibility)));
            yield return new(OpCodes.Callvirt, PropertyGetter(typeof(ValidatingVisibilityEventArgs), nameof(ValidatingVisibilityEventArgs.IsAllowed)));
            yield return new(OpCodes.Brfalse_S, ret);

            // if (IsLateSeen)
            //     ValidatingVisibility.SetToLastSeen(target);
            // return true;
            yield return new(OpCodes.Ldloc_S, ev.LocalIndex);
            yield return new(OpCodes.Callvirt, PropertyGetter(typeof(ValidatingVisibilityEventArgs), nameof(ValidatingVisibilityEventArgs.IsLateSeen)));
            yield return new(OpCodes.Brfalse_S, cnt);

            yield return new(OpCodes.Ldarg_1);
            yield return new(OpCodes.Call, Method(typeof(ValidatingVisibility), nameof(ValidatingVisibility.SetToLastSeen)));

            yield return new CodeInstruction(OpCodes.Ldc_I4_1).WithLabels(cnt);
            yield return new(OpCodes.Ret);
        }

        private static void SetToLastSeen(ReferenceHub target)
        {
            Scp939VisibilityController.LastSeen[target.netId] = new Scp939VisibilityController.LastSeenInfo()
            {
                Time = NetworkTime.time,
            };
        }
    }
}
