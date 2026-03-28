// -----------------------------------------------------------------------
// <copyright file="Resurrecting.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp1509
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp1509;
    using HarmonyLib;
    using InventorySystem.Items.Scp1509;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp1509Item.ServerApplyResurrectEffects"/>
    /// to add <see cref="Handlers.Scp1509.Resurrecting"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp1509), nameof(Handlers.Scp1509.Resurrecting))]
    [HarmonyPatch(typeof(Scp1509Item), nameof(Scp1509Item.ServerApplyResurrectEffects))]
    internal class Resurrecting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldarg_2);

            LocalBuilder ev = generator.DeclareLocal(typeof(ResurrectingEventArgs));

            Label continueLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(resurrectedPlayer)
                new(OpCodes.Ldarg_2),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // Player.Get(victim)
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // respawnRole
                new(OpCodes.Ldarg_3),

                // this
                new(OpCodes.Ldarg_0),

                // true
                new(OpCodes.Ldc_I4_1),

                // ResurrectingEventArgs ev = new(Player.Get(resurrectedPlayer), Player.Get(victim), respawnRole, this, true);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ResurrectingEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Scp1509.OnResurrecting(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Scp1509), nameof(Handlers.Scp1509.OnResurrecting))),

                // if (!ev.IsAllowed)
                //     return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(ResurrectingEventArgs), nameof(ResurrectingEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, continueLabel),

                new(OpCodes.Ret),

                // respawnRole = ev.NewRole;
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(continueLabel),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ResurrectingEventArgs), nameof(ResurrectingEventArgs.NewRole))),
                new(OpCodes.Starg_S, 3),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}