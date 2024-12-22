// -----------------------------------------------------------------------
// <copyright file="ChangingMicroHIDState.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using API.Features.Items;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using InventorySystem.Items.MicroHID;
    using InventorySystem.Items.MicroHID.Modules;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="CycleController.Phase"/>.
    /// Adds the <see cref="Handlers.Player.ChangingMicroHIDState"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ChangingMicroHIDState))]
    [HarmonyPatch(typeof(CycleController), nameof(CycleController.Phase), MethodType.Setter)]
    internal static class ChangingMicroHIDState
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingMicroHIDStateEventArgs));

            int offset = 1;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ret) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // Item.Get(this.Serial);
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Ldfld, Field(typeof(CycleController), nameof(CycleController.Serial))),
                new(OpCodes.Call, GetDeclaredMethods(typeof(Item)).Find(x => !x.IsGenericMethod && x.IsStatic && x.GetParameters().FirstOrDefault()?.ParameterType == typeof(ushort))),

                // value
                new(OpCodes.Ldarg_1),

                // true
                new(OpCodes.Ldc_I4_1),

                // ChangerMicroHIDStateEventArgs ev = new(Item.Get(this.Serial), value, true);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingMicroHIDStateEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Player.OnChangingMicroHIDState(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnChangingMicroHIDState))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingMicroHIDStateEventArgs), nameof(ChangingMicroHIDStateEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),

                // value = ev.NewPhase;
                new(OpCodes.Ldarg_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingMicroHIDStateEventArgs), nameof(ChangingMicroHIDStateEventArgs.NewPhase))),
                new(OpCodes.Starg_S, 1),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}