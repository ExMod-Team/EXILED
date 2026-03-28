// -----------------------------------------------------------------------
// <copyright file="JailbirdWearState.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Item
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Item;
    using HarmonyLib;
    using InventorySystem.Items.Jailbird;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="JailbirdDeteriorationTracker.RecheckUsage"/>
    /// to add <see cref="Handlers.Item.JailbirdChangingWearState"/> and <see cref="Handlers.Item.JailbirdChangedWearState"/> events.
    /// </summary>
    [EventPatch(typeof(Handlers.Item), nameof(Handlers.Item.JailbirdChangingWearState))]
    [EventPatch(typeof(Handlers.Item), nameof(Handlers.Item.JailbirdChangedWearState))]
    [HarmonyPatch(typeof(JailbirdDeteriorationTracker), nameof(JailbirdDeteriorationTracker.RecheckUsage))]
    internal static class JailbirdWearState
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder oldState = generator.DeclareLocal(typeof(InventorySystem.Items.Jailbird.JailbirdWearState));
            LocalBuilder evChanging = generator.DeclareLocal(typeof(JailbirdChangingWearStateEventArgs));

            Label retLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // oldWearState = this.WearState
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, PropertyGetter(typeof(JailbirdDeteriorationTracker), nameof(JailbirdDeteriorationTracker.WearState))),
                new(OpCodes.Stloc_S, oldState.LocalIndex),
            });

            int offset = 1;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Stloc_2) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // if (oldWearState == newWearState)
                //    return;
                new(OpCodes.Ldloc_S, oldState.LocalIndex),
                new(OpCodes.Ldloc_2),
                new(OpCodes.Beq, retLabel),

                // this._jailbird
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(JailbirdDeteriorationTracker), nameof(JailbirdDeteriorationTracker._jailbird))),

                // newWearState
                new(OpCodes.Ldloc_2),

                // oldWearState
                new(OpCodes.Ldloc_S, oldState.LocalIndex),

                // JailbirdChangingWearStateEventArgs ev = new(InventorySystem.Items.ItemBase, JailbirdWearState, JailbirdWearState)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(JailbirdChangingWearStateEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, evChanging.LocalIndex),

                // Handlers.Item.OnJailbirdStateChanging(ev)
                new(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnJailbirdStateChanging))),

                // if (!IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(JailbirdChangingWearStateEventArgs), nameof(JailbirdChangingWearStateEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),

                // newWearState = ev.NewWearState;
                new(OpCodes.Ldloc_S, evChanging.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(JailbirdChangingWearStateEventArgs), nameof(JailbirdChangingWearStateEventArgs.NewWearState))),
                new(OpCodes.Stloc_2),
            });

            offset = 1;
            index = newInstructions.FindIndex(x => x.Calls(Method(typeof(Dictionary<ushort, InventorySystem.Items.Jailbird.JailbirdWearState>), "set_Item")));

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // this._jailbird
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(JailbirdDeteriorationTracker), nameof(JailbirdDeteriorationTracker._jailbird))),

                // newWearState
                new(OpCodes.Ldloc_2),

                // oldWearState
                new(OpCodes.Ldloc_S, oldState.LocalIndex),

                // JailbirdChangedWearStateEventArgs ev = new(InventorySystem.Items.ItemBase, JailbirdWearState, JailbirdWearState)
                // Handlers.Item.OnJailbirdStateChanged(ev)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(JailbirdChangedWearStateEventArgs))[0]),
                new(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnJailbirdStateChanged))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}