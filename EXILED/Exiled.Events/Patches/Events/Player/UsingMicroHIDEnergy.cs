// -----------------------------------------------------------------------
// <copyright file="UsingMicroHIDEnergy.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using InventorySystem.Items.MicroHID.Modules;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="EnergyManagerModule.EquipUpdate" />.
    /// Adds the <see cref="Handlers.Player.UsingMicroHIDEnergy" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.UsingMicroHIDEnergy))]
    [HarmonyPatch(typeof(EnergyManagerModule), nameof(EnergyManagerModule.EquipUpdate))]
    internal static class UsingMicroHIDEnergy
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(UsingMicroHIDEnergyEventArgs));

            int offset = -4;
            int index = newInstructions.FindLastIndex(i => i.Calls(Method(typeof(EnergyManagerModule), nameof(EnergyManagerModule.ServerSetEnergy)))) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // this.MicroHID
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Callvirt, PropertyGetter(typeof(EnergyManagerModule), nameof(EnergyManagerModule.MicroHid))),

                // newEnergy
                new(OpCodes.Ldloc_0),

                // true
                new(OpCodes.Ldc_I4_1),

                // calculate drain by delta between old energy and new energy
                // UsingMicroHIDEnergyEventArgs ev = new(this.MicroHID, newEnergy, true);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UsingMicroHIDEnergyEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Player.OnUsingMicroHIDEnergy(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnUsingMicroHIDEnergy))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(UsingMicroHIDEnergyEventArgs), nameof(UsingMicroHIDEnergyEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),

                // newEnergy = this.Energy - ev.Drain;
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(EnergyManagerModule), nameof(EnergyManagerModule.Energy))),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(UsingMicroHIDEnergyEventArgs), nameof(UsingMicroHIDEnergyEventArgs.Drain))),
                new(OpCodes.Sub),
                new(OpCodes.Stloc_0),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
