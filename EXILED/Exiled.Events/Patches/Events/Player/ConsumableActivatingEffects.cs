// -----------------------------------------------------------------------
// <copyright file="ConsumableActivatingEffects.cs" company="ExMod Team">
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
    /// Patches <see cref="Consumable.ActivateEffects"/> and adds <see cref="Handlers.Player.ConsumableActivatingEffects"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Consumable), nameof(Consumable.ActivateEffects))]
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ConsumableActivatingEffects))]
    internal static class ConsumableActivatingEffects
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();

            int secondOffset = 1;
            int secondIndex = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldarg_0) + secondOffset;

            int firstOffset = -1;
            int firstIndex = newInstructions.FindIndex(i => i.opcode == OpCodes.Callvirt) + firstOffset;
            newInstructions[firstIndex].WithLabels(continueLabel);
            newInstructions.InsertRange(firstIndex, new List<CodeInstruction>()
            {
                new(OpCodes.Ldarg_0),
            });

            newInstructions.InsertRange(secondIndex, new List<CodeInstruction>()
            {
                // this.Owner
                new(OpCodes.Callvirt, PropertyGetter(typeof(Consumable), nameof(Consumable.Owner))),

                // this
                new(OpCodes.Ldarg_0),

                // true
                new(OpCodes.Ldc_I4_1),

                // ConsumableActivatingEffectsEventArgs ev = new(this.Owner, this, true)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ConsumableActivatingEffectsEventArgs))[0]),
                new(OpCodes.Dup),

                // OnConsumableActivatingEffects(ev)
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnConsumableActivatingEffects))),

                // if (!ev.IsAllowed)
                //      return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(ConsumableActivatingEffectsEventArgs), nameof(ConsumableActivatingEffectsEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, continueLabel),
                new(OpCodes.Ret),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}