// -----------------------------------------------------------------------
// <copyright file="InspectingWeapon.cs" company="ExMod Team">
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
    /// Patches <see cref="SimpleInspectorModule.ServerProcessCmd"/>
    /// to add <see cref="Handlers.Item.InspectingWeapon"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Item), nameof(Handlers.Item.InspectingWeapon))]
    [HarmonyPatch(typeof(SimpleInspectorModule), nameof(SimpleInspectorModule.ServerProcessCmd))]
    internal class InspectingWeapon
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldarg_0);

            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                // this.Firearm
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Callvirt, PropertyGetter(typeof(SimpleInspectorModule), nameof(SimpleInspectorModule.Firearm))),

                // InspectingWeaponEventArgs ev = new(this.Firearm)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InspectingWeaponEventArgs))[0]),

                // Handlers.Item.OnInspectingWeapon(ev)
                new(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnInspectingWeapon))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}