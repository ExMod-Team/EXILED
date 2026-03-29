// -----------------------------------------------------------------------
// <copyright file="ChangingDisruptorMode.cs" company="ExMod Team">
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
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder newMode = generator.DeclareLocal(typeof(bool));

            int offset = 2;
            int index = newInstructions.FindIndex(x => x.Calls(Method(typeof(NetworkReaderExtensions), nameof(NetworkReaderExtensions.ReadBool))));

            newInstructions.InsertRange(index + offset, new CodeInstruction[]
            {
                // this.Firearm;
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(DisruptorModeSelector), nameof(DisruptorModeSelector.Firearm))),

                // newMode;
                new(OpCodes.Ldloc, newMode),

                // ChangerDisruptorModeEventArgs ev = new(this.Firearm, newMode);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingDisruptorModeEventArgs))[0]),

                // Handlers.Player.OnChangingDisruptorMode(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnChangingDisruptorMode))),
            });

            offset = 1;
            newInstructions.InsertRange(index + offset, new CodeInstruction[]
            {
                // bool newMode = reader.ReadBool();
                new(OpCodes.Dup),
                new(OpCodes.Stloc, newMode),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
