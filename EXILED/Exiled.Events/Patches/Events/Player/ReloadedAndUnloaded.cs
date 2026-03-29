// -----------------------------------------------------------------------
// <copyright file="ReloadedAndUnloaded.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;
    using InventorySystem.Items.Autosync;
    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.Modules;
    using InventorySystem.Items.Firearms.Modules.Misc;
    using InventorySystem.Searching;
    using Mirror;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    /// Patches <see cref="AnimatorReloaderModuleBase" /> to add missing event handler to the
    /// <see cref="Handlers.Player.UnloadedWeapon" /> and <see cref="Handlers.Player.ReloadedWeapon" /> .
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.UnloadedWeapon))]
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ReloadedWeapon))]
    [HarmonyPatch(typeof(AnimatorReloaderModuleBase), nameof(AnimatorReloaderModuleBase.StopReloadingAndUnloading))]
    internal static class ReloadedAndUnloaded
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label skipLabel = generator.DefineLabel();
            Label retLabel = generator.DefineLabel();
            LocalBuilder isReloaded = generator.DeclareLocal(typeof(bool));
            LocalBuilder isUnloaded = generator.DeclareLocal(typeof(bool));
            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // bool isReloaded = __instance.IsReloading;
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(AnimatorReloaderModuleBase), nameof(AnimatorReloaderModuleBase.IsReloading))),
                new(OpCodes.Stloc_S, isReloaded.LocalIndex),

                // bool isUnloaded = __instance.IsUnloading;
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(AnimatorReloaderModuleBase), nameof(AnimatorReloaderModuleBase.IsUnloading))),
                new(OpCodes.Stloc_S, isUnloaded.LocalIndex),
            });

            newInstructions.InsertRange(newInstructions.Count - 1, new[]
            {
                // if (!isReloaded) goto skipLabel;
                new(OpCodes.Ldloc_S, isReloaded.LocalIndex),
                new(OpCodes.Brfalse_S, skipLabel),

                // __instance.Firearm
                new CodeInstruction(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(AnimatorReloaderModuleBase), nameof(AnimatorReloaderModuleBase.Firearm))),

                // ReloadedWeaponEventArgs evReloadedWeapon = new(Firearm)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ReloadedWeaponEventArgs))[0]),

                // Handlers.Player.OnReloadedWeapon(evReloadedWeapon)
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnReloadedWeapon))),

                // if (!isUnloaded) return;
                new CodeInstruction(OpCodes.Ldloc_S, isUnloaded.LocalIndex).WithLabels(skipLabel),
                new(OpCodes.Brfalse_S, retLabel),

                // __instance.Firearm
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(AnimatorReloaderModuleBase), nameof(AnimatorReloaderModuleBase.Firearm))),

                // UnloadedWeaponEventArgs evUnloadedWeaponE = new(Firearm)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UnloadedWeaponEventArgs))[0]),

                // Handlers.Player.OnUnloadedWeapon(evUnloadedWeaponE)
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnUnloadedWeapon))),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}