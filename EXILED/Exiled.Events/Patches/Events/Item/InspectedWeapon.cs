// -----------------------------------------------------------------------
// <copyright file="InspectedWeapon.cs" company="ExMod Team">
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
    using InventorySystem.Items.Firearms.Modules;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="SimpleInspectorModule.SetInspecting"/>
    /// to add <see cref="Handlers.Item.InspectedWeapon"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Item), nameof(Handlers.Item.InspectedWeapon))]
    [HarmonyPatch(typeof(SimpleInspectorModule), nameof(SimpleInspectorModule.SetInspecting))]
    internal class InspectedWeapon
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldarg_0);

            Label returnLabel = generator.DefineLabel();
            Label skipLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                // if (val)
                //   skip event
                new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Brtrue_S, skipLabel),

                // this.Firearm
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(SimpleInspectorModule), nameof(SimpleInspectorModule.Firearm))),

                // InspectedWeaponEventArgs ev = new(this.Firearm)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InspectedWeaponEventArgs))[0]),

                // Handlers.Item.OnInspectedWeapon(ev)
                new(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnInspectedWeapon))),

                // skip:
                new CodeInstruction(OpCodes.Nop).WithLabels(skipLabel),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}