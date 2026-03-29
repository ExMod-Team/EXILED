// -----------------------------------------------------------------------
// <copyright file="DisruptorFiring.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Item
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Item;
    using Footprinting;
    using HarmonyLib;
    using InventorySystem.Items;
    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.Extensions;
    using InventorySystem.Items.Firearms.Modules;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="DisruptorWorldmodelActionExtension.ServerFire"/>. Adds the <see cref="DisruptorFiring"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Item), nameof(Handlers.Item.DisruptorFiring))]
    [HarmonyPatch(typeof(DisruptorWorldmodelActionExtension), nameof(DisruptorWorldmodelActionExtension.ServerFire))]
    public class DisruptorFiring
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(DisruptorFiringEventArgs));
            Label continueLabel = generator.DefineLabel();
            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // Pickup.Get(this._worldmodel.Identifier.SerialNumber)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(DisruptorWorldmodelActionExtension), nameof(DisruptorWorldmodelActionExtension._worldmodel))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(FirearmWorldmodel), nameof(FirearmWorldmodel.Identifier))),
                new(OpCodes.Ldfld, Field(typeof(ItemIdentifier), nameof(ItemIdentifier.SerialNumber))),
                new(OpCodes.Call, Method(typeof(Pickup), nameof(Pickup.Get), new[] { typeof(ushort) })),

                // Player.Get(this._scheduledAttackerFootprint)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(DisruptorWorldmodelActionExtension), nameof(DisruptorWorldmodelActionExtension._scheduledAttackerFootprint))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(Footprint) })),

                // this._scheduledFiringState
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(DisruptorWorldmodelActionExtension), nameof(DisruptorWorldmodelActionExtension._scheduledFiringState))),

                // true
                new(OpCodes.Ldc_I4_0),

                // DisruptorFiringEventArgs ev = new DisruptorFiringEventArgs(Pickup.Get(this._worldmodel.Identifier.SerialNumber), Player.Get(this._scheduledAttackerFootprint), this._scheduledFiringState, true)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DisruptorFiringEventArgs))[0]),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Item.OnDisruptorFiring(ev);
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnDisruptorFiring))),

                // if (!ev.IsAllowed) return;
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new (OpCodes.Callvirt, PropertyGetter(typeof(DisruptorFiringEventArgs), nameof(DisruptorFiringEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, continueLabel),
                new(OpCodes.Ret),

                // this._scheduledAttackerFootprint = Attacker.Footprint;
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(continueLabel),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(DisruptorFiringEventArgs), nameof(DisruptorFiringEventArgs.Attacker))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.Footprint))),
                new(OpCodes.Stfld, Field(typeof(DisruptorWorldmodelActionExtension), nameof(DisruptorWorldmodelActionExtension._scheduledAttackerFootprint))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}