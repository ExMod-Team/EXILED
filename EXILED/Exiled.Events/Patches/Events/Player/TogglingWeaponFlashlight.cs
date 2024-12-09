// -----------------------------------------------------------------------
// <copyright file="TogglingWeaponFlashlight.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Extensions;
    using Exiled.API.Features.Pools;

    using Exiled.Events.Attributes;

    using Exiled.Events.EventArgs.Player;

    using Exiled.Events.Handlers;

    using HarmonyLib;

    using InventorySystem.Items.Autosync;
    using InventorySystem.Items.Firearms.Attachments;
    using InventorySystem.Items.Firearms.Modules;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="FlashlightAttachment.ServerProcessCmd(Mirror.NetworkReader)"/>
    /// to add <see cref="Player.TogglingWeaponFlashlight"/> event.
    /// </summary>
    [EventPatch(typeof(Player), nameof(Player.TogglingWeaponFlashlight))]
    [HarmonyPatch(typeof(FlashlightAttachment), nameof(FlashlightAttachment.ServerProcessCmd))]
    internal class TogglingWeaponFlashlight
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(TogglingWeaponFlashlightEventArgs));

            int offset = -1;
            int index = newInstructions.FindIndex(x => x.Calls(PropertyGetter(typeof(SubcomponentBase), nameof(SubcomponentBase.ItemSerial)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // this.Firearm
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(LinearAdsModule), nameof(LinearAdsModule.Firearm))),

                    // previousValue
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(FlashlightAttachment), nameof(FlashlightAttachment.ItemSerial))),
                    new(OpCodes.Call, Method(typeof(FlashlightAttachment), nameof(FlashlightAttachment.GetEnabled))),

                    // TogglingWeaponFlashlightEventArgs ev = new(firearm, previousValue);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(TogglingWeaponFlashlightEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnTogglingWeaponFlashlight(ev);
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnTogglingWeaponFlashlight))),

                    // if (!ev.IsAllowed)
                    //     return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(TogglingWeaponFlashlightEventArgs), nameof(TogglingWeaponFlashlightEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, retLabel),

                    // this.ServerSendStatus(ev.NewState)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(TogglingWeaponFlashlightEventArgs), nameof(TogglingWeaponFlashlightEventArgs.NewState))),
                    new(OpCodes.Callvirt, Method(typeof(FlashlightAttachment), nameof(FlashlightAttachment.ServerSendStatus))),
                    new(OpCodes.Ret),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}