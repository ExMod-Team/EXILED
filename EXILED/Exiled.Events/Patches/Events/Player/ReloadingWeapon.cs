// -----------------------------------------------------------------------
// <copyright file="ReloadingWeapon.cs" company="ExMod Team">
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
    using InventorySystem.Items.Firearms.Modules;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="AnimatorReloaderModuleBase.ServerProcessCmd" />.
    /// Adds the <see cref="Handlers.Player.ReloadingWeapon" /> and <see cref="Handlers.Player.UnloadingWeapon" />event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ReloadingWeapon))]
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.UnloadingWeapon))]
    [HarmonyPatch(typeof(AnimatorReloaderModuleBase), nameof(AnimatorReloaderModuleBase.ServerProcessCmd))]
    internal static class ReloadingWeapon
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev1 = generator.DeclareLocal(typeof(ReloadingWeaponEventArgs));
            LocalBuilder ev2 = generator.DeclareLocal(typeof(UnloadingWeaponEventArgs));

            int offset = -2;
            int index = newInstructions.FindIndex(x => x.Calls(Method(typeof(IReloadUnloadValidatorModule), nameof(IReloadUnloadValidatorModule.ValidateReload)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // this.Firearm
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AnimatorReloaderModuleBase), nameof(AnimatorReloaderModuleBase.Firearm))),

                    // ReloadingWeaponEventArgs ev = new(firearm)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ReloadingWeaponEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev1.LocalIndex),

                    // Player.OnReloadingWeapon(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnReloadingWeapon))),
                });

            offset = 1;
            index = newInstructions.FindIndex(x => x.Calls(Method(typeof(IReloadUnloadValidatorModule), nameof(IReloadUnloadValidatorModule.ValidateReload)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // flag = IReloadUnloadValidatorModule.ValidateReload(base.Firearm) && ev.IsAllowed;
                    new(OpCodes.Ldloc_S, ev1.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ReloadingWeaponEventArgs), nameof(ReloadingWeaponEventArgs.IsAllowed))),
                    new(OpCodes.And),
                });

            offset = -2;
            index = newInstructions.FindIndex(x => x.Calls(Method(typeof(IReloadUnloadValidatorModule), nameof(IReloadUnloadValidatorModule.ValidateUnload)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // this.Firearm
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AnimatorReloaderModuleBase), nameof(AnimatorReloaderModuleBase.Firearm))),

                    // UnloadingWeaponEventArgs ev = new(firearm)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UnloadingWeaponEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev2.LocalIndex),

                    // Player.OnUnloadingWeapon(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnUnloadingWeapon))),
                });

            offset = 1;
            index = newInstructions.FindIndex(x => x.Calls(Method(typeof(IReloadUnloadValidatorModule), nameof(IReloadUnloadValidatorModule.ValidateUnload)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // flag = IReloadUnloadValidatorModule.ValidateUnload(base.Firearm) && ev.IsAllowed;
                    new(OpCodes.Ldloc_S, ev2.LocalIndex),
                    new (OpCodes.Callvirt, PropertyGetter(typeof(UnloadingWeaponEventArgs), nameof(UnloadingWeaponEventArgs.IsAllowed))),
                    new(OpCodes.And),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
