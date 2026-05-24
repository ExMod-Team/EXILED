// -----------------------------------------------------------------------
// <copyright file="ChangingWearables.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using Mirror;

    using PlayerRoles.FirstPersonControl.Thirdperson.Subcontrollers.Wearables;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="WearableSync.OverrideWearables"/>
    /// to add <see cref="Handlers.Player.ChangingWearables"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ChangingWearables))]
    [HarmonyPatch(typeof(WearableSync), nameof(WearableSync.OverrideWearables))]
    internal static class ChangingWearables
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldarg_1);

            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingWearablesEventArgs));

            Label returnLabel = generator.DefineLabel();
            newInstructions[^1].labels.Add(returnLabel);

            newInstructions.RemoveAt(index);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Player.Get(hub)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // newWearables
                new(OpCodes.Ldarg_1),

                // true
                new(OpCodes.Ldc_I4_1),

                // ChangingWearablesEventArgs ev = new(Player.Get(hub), newWearables, true);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingWearablesEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Player.OnChangingWearables(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnChangingWearables))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingWearablesEventArgs), nameof(ChangingWearablesEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),

                // push ev.NewWearables to stack for check
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingWearablesEventArgs), nameof(ChangingWearablesEventArgs.NewWearables))),
            });

            int offset = -4;
            index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldarg_0) + offset;

            newInstructions.RemoveRange(index, 4);
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // newWearables = ChangingWearables.WriteArmor(ev);
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Call, Method(typeof(ChangingWearables), nameof(WriteArmor))),
                new(OpCodes.Starg_S, 1),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static WearableElements WriteArmor(ChangingWearablesEventArgs ev)
        {
            WearableElementType value = ev.NewWearables;

            if (value is WearableElementType.None)
                return WearableElements.None;

            if (value.HasFlag(WearableElementType.ArmorDefault))
            {
                ItemType displayedArmor = value.HasFlag(WearableElementType.ArmorLight) ? ItemType.ArmorLight :
                    value.HasFlag(WearableElementType.ArmorCombat) ? ItemType.ArmorCombat :
                    value.HasFlag(WearableElementType.ArmorHeavy) ? ItemType.ArmorHeavy :
                    ev.Player.CurrentArmor?.Type ?? ItemType.None;

                WearableSync.PayloadWriter.WriteSByte((sbyte)displayedArmor);

                value &= ~WearableElementType.ArmorLight | WearableElementType.ArmorCombat | WearableElementType.ArmorHeavy;
            }

            return (WearableElements)value;
        }
    }
}