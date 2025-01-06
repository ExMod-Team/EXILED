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
    using System.Linq;
    using System.Reflection.Emit;

    using Attributes;

    using Exiled.API.Features.Pools;

    using Exiled.Events.EventArgs.Player;

    using Handlers;

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
            int index = newInstructions.FindLastIndex(x => x.Calls(PropertySetter(typeof(AutomaticActionModule), nameof(AutomaticActionModule.Cocked)))) + offset;

            newInstructions.InsertRange(index, GetInstructions(newInstructions[index], ret));

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /* ------------> Commented out, because it is never called (nw moment). Calls 2 PumpActionModule.ShootOneBarrel(bool) instead.
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

            newInstructions.InsertRange(0, DryFireAutomatic.GetInstructions(newInstructions[0], ret));

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
    */

    /// <summary>
    /// Patches <see cref="PumpActionModule.ShootOneBarrel(bool)"/>
    /// to add <see cref="Player.DryfiringWeapon"/> event for pump shots.
    /// </summary>
    [EventPatch(typeof(Player), nameof(Player.DryfiringWeapon))]
    [HarmonyPatch(typeof(PumpActionModule), nameof(PumpActionModule.ShootOneBarrel))]
    internal class DryFirePump
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label ret = generator.DefineLabel();

            int offset = -7;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Stloc_1) + offset;

            // firstly, set `flag` to a false, and then if `IsAllowed == false` we could return without problems
            newInstructions.InsertRange(index, new CodeInstruction[]
                {
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new(OpCodes.Stloc_1),
                }.Concat(DryFireAutomatic.GetInstructions(newInstructions[0], ret)));

            // place return label to a ldloc instruction, to return `flag` variables
            newInstructions[newInstructions.Count - 2].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}