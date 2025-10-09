// -----------------------------------------------------------------------
// <copyright file="JailbirdState.cs" company="ExMod Team">
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
    internal static class JailbirdState
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder oldState = generator.DeclareLocal(typeof(JailbirdWearState));
            LocalBuilder evChanging = generator.DeclareLocal(typeof(JailbirdChangingStateEventArgs));

            Label skipChangedEventLabel = generator.DefineLabel();
            Label retLabel = generator.DefineLabel();

            int originalStloc2Index = newInstructions.FindIndex(x => x.opcode == OpCodes.Stloc_2);
            if (originalStloc2Index == -1)
            {
                for (int i = 0; i < newInstructions.Count; i++)
                    yield return newInstructions[i];

                ListPool<CodeInstruction>.Pool.Return(newInstructions);
                yield break;
            }

            CodeInstruction[] prefixInstructions = new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, PropertyGetter(typeof(JailbirdDeteriorationTracker), nameof(JailbirdDeteriorationTracker.WearState))),
                new(OpCodes.Stloc_S, oldState.LocalIndex),
            };

            newInstructions.InsertRange(0, prefixInstructions);
            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            int targetStloc2Index = originalStloc2Index + prefixInstructions.Length;

            newInstructions.InsertRange(
                targetStloc2Index + 1,
                new[]
                {
                    // if (oldWearState == newWearState)
                    //    skipChangedEventLabel;
                    new CodeInstruction(OpCodes.Ldloc_S, oldState.LocalIndex),
                    new(OpCodes.Ldloc_2),
                    new(OpCodes.Beq_S, skipChangedEventLabel),

                    // this._jailbird.Owner
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(JailbirdDeteriorationTracker), "_jailbird")),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(JailbirdItem), nameof(JailbirdItem.Owner))),

                    // this._jailbird
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(JailbirdDeteriorationTracker), "_jailbird")),

                    // newWearState
                    new(OpCodes.Ldloc_2),

                    // oldWearState
                    new(OpCodes.Ldloc_S, oldState.LocalIndex),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // JailbirdChangingStateEventArgs ev = new(...)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(JailbirdChangingStateEventArgs))[0]),
                    new(OpCodes.Stloc_S, evChanging.LocalIndex),

                    // Handlers.Item.OnJailbirdStateChanging(ev)
                    new(OpCodes.Ldloc_S, evChanging.LocalIndex),
                    new(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnJailbirdStateChanging))),

                    // if (!IsAllowed)
                    //    return;
                    new(OpCodes.Ldloc_S, evChanging.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(JailbirdChangingStateEventArgs), nameof(JailbirdChangingStateEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, retLabel),

                    // Update local 2
                    new(OpCodes.Ldloc_S, evChanging.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(JailbirdChangingStateEventArgs), nameof(JailbirdChangingStateEventArgs.NewWearState))),
                    new(OpCodes.Stloc_2),
                });

            int dictSetIndex = newInstructions.FindIndex(x => x.Calls(Method(typeof(Dictionary<ushort, JailbirdWearState>), "set_Item")));
            if (dictSetIndex == -1)
            {
                for (int i = 0; i < newInstructions.Count; i++)
                    yield return newInstructions[i];

                ListPool<CodeInstruction>.Pool.Return(newInstructions);
                yield break;
            }

            int insertAfterDictSet = dictSetIndex + 1;

            newInstructions.InsertRange(
                insertAfterDictSet,
                new[]
                {
                    new CodeInstruction(OpCodes.Nop).WithLabels(skipChangedEventLabel),

                    // if (oldWearState == newWearState)
                    //    return;
                    new(OpCodes.Ldloc_S, oldState.LocalIndex),
                    new(OpCodes.Ldloc_2),
                    new(OpCodes.Beq_S, retLabel),

                    // this._jailbird.Owner
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(JailbirdDeteriorationTracker), "_jailbird")),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(JailbirdItem), nameof(JailbirdItem.Owner))),

                    // this._jailbird
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(JailbirdDeteriorationTracker), "_jailbird")),

                    // newWearState
                    new(OpCodes.Ldloc_2),

                    // oldWearState
                    new(OpCodes.Ldloc_S, oldState.LocalIndex),

                    // JailbirdChangedStateEventArgs ev = new(...)
                    // Handlers.Item.OnJailbirdStateChanged(ev)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(JailbirdChangedStateEventArgs))[0]),
                    new(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnJailbirdStateChanged))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}