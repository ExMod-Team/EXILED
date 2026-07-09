// -----------------------------------------------------------------------
// <copyright file="ConsumingItem.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem.Items.Usables;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Consumable.ActivateEffects" />.
    /// Adds the <see cref="Handlers.Player.ConsumingItem" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ConsumingItem))]
    [HarmonyPatch(typeof(Consumable), nameof(Consumable.ActivateEffects))]
    internal static class ConsumingItem
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label skip = generator.DefineLabel();

            int offset = -1;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(Consumable), nameof(Consumable.OnEffectsActivated)))) + offset;

            List<Label> mainLabels = newInstructions[index].ExtractLabels();
            newInstructions[index].WithLabels(skip);

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // this.Owner;
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(mainLabels),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Consumable), nameof(Consumable.Owner))),

                // this;
                new(OpCodes.Ldarg_0),

                // ConsumingItemEventArgs ev = new(this.Owner, this);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ConsumingItemEventArgs))[0]),
                new(OpCodes.Dup),

                // Player.OnConsumingItem(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnConsumingItem))),

                // if (!ev.IsAllowed)
                //   this._alreadyActivated = true;
                //   return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(ConsumingItemEventArgs), nameof(ConsumingItemEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, skip),

                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Stfld, Field(typeof(Consumable), nameof(Consumable._alreadyActivated))),

                new(OpCodes.Ret),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
