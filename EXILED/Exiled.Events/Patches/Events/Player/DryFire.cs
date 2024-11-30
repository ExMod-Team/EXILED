// -----------------------------------------------------------------------
// <copyright file="DryFire.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1402
#pragma warning disable SA1600
#pragma warning disable SA1649
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Extensions;
    using Exiled.API.Features.Pools;

    using Exiled.Events.Attributes;

    using Exiled.Events.EventArgs.Player;

    using Exiled.Events.Handlers;

    using HarmonyLib;

    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.Modules;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="AutomaticActionModule.UpdateServer()"/>
    /// to add <see cref="Player.DryfiringWeapon"/> event for automatic firearms.
    /// </summary>
    [EventPatch(typeof(Player), nameof(Player.DryfiringWeapon))]
    [HarmonyPatch(typeof(AutomaticActionModule), nameof(AutomaticActionModule.UpdateServer))]
    internal class DryFireAutomatic
    {
        internal static IEnumerable<CodeInstruction> GetInstructions(CodeInstruction start, Label ret)
        {
            // this.Firearm
            yield return new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(start);
            yield return new(OpCodes.Callvirt, PropertyGetter(typeof(FirearmSubcomponentBase), nameof(FirearmSubcomponentBase.Firearm)));

            // DryfiringWeaponEventArgs ev = new(firearm);
            yield return new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DryfiringWeaponEventArgs))[0]);
            yield return new(OpCodes.Dup);

            // Handlers.Player.OnDryfiringWeapon(ev);
            yield return new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnDryfiringWeapon)));

            // if (!ev.IsAllowed)
            //     return;
            yield return new(OpCodes.Callvirt, PropertyGetter(typeof(DryfiringWeaponEventArgs), nameof(DryfiringWeaponEventArgs.IsAllowed)));
            yield return new(OpCodes.Brfalse_S, ret);
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            // for returning we will goto ServerSendResponse method, it is necessarily
            int offset = -3;
            Label ret = newInstructions[newInstructions.Count - 1 + offset].labels[0];

            offset = -2;
            int index = newInstructions.FindIndex(x => x.Calls(PropertyGetter(typeof(AutomaticActionModule), nameof(AutomaticActionModule.Cocked)))) + offset;

            newInstructions.InsertRange(
                index,
                GetInstructions(newInstructions[index], ret));

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="DoubleActionModule.FireDry()"/>
    /// to add <see cref="Player.DryfiringWeapon"/> event for double shots.
    /// </summary>
    [EventPatch(typeof(Player), nameof(Player.DryfiringWeapon))]
    [HarmonyPatch(typeof(DoubleActionModule), nameof(DoubleActionModule.FireDry))]
    internal class DryFireDouble
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label ret = generator.DefineLabel();

            newInstructions.InsertRange(
                0,
                DryFireAutomatic.GetInstructions(newInstructions[0], ret));

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}