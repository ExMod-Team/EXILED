// -----------------------------------------------------------------------
// <copyright file="ChangingDisruptorMode.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using InventorySystem.Items;
    using InventorySystem.Items.Firearms.Modules;
    using Mirror;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="DisruptorModeSelector.ServerProcessCmd"/>
    /// to add <see cref="Handlers.Player.ChangingDisruptorMode"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ChangingDisruptorMode))]
    [HarmonyPatch(typeof(DisruptorModeSelector), nameof(DisruptorModeSelector.ServerProcessCmd))]
    internal static class ChangingDisruptorMode
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldarg_0);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Item.Get(this.Firearm);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(DisruptorModeSelector), nameof(DisruptorModeSelector.Firearm))),
                new(OpCodes.Call, GetDeclaredMethods(typeof(Item)).Find(x => !x.IsGenericMethod && x.GetParameters().FirstOrDefault()?.ParameterType == typeof(ItemBase))),

                // reader.ReadBool();
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(NetworkReaderExtensions), nameof(NetworkReaderExtensions.ReadBool))),

                // ChangerDisruptorModeEventArgs ev = new(Item.Get(this.Firearm), reader.ReadBool(), true);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingDisruptorModeEventArgs))[0]),

                // Handlers.Player.OnChangingDisruptorMode(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnChangingDisruptorMode))),

                // reader.Position--;
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, Field(typeof(NetworkReader), nameof(NetworkReader.Position))),
                new(OpCodes.Ldc_I4_M1),
                new(OpCodes.Add),
                new(OpCodes.Stfld, Field(typeof(NetworkReader), nameof(NetworkReader.Position))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}