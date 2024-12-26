// -----------------------------------------------------------------------
// <copyright file="Aiming.cs" company="ExMod Team">
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
    using InventorySystem.Items.Firearms.Modules;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="LinearAdsModule.ServerProcessCmd(Mirror.NetworkReader)"/>
    /// to add <see cref="Player.AimingDownSight"/> event.
    /// </summary>
    [EventPatch(typeof(Player), nameof(Player.AimingDownSight))]
    [HarmonyPatch(typeof(LinearAdsModule), nameof(LinearAdsModule.ServerProcessCmd))]
    internal class Aiming
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ads = generator.DeclareLocal(typeof(bool));

            int offset = 0;
            int index = newInstructions.FindIndex(x => x.StoresField(Field(typeof(LinearAdsModule), nameof(LinearAdsModule._userInput)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // read bool value
                    new CodeInstruction(OpCodes.Stloc_S, ads.LocalIndex),

                    // this.Firearm;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(LinearAdsModule), nameof(LinearAdsModule.Firearm))),

                    // userInput
                    new(OpCodes.Ldloc_S, ads.LocalIndex),

                    // AimingDownSightEventArgs ev = new(firearm, userInput);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AimingDownSightEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnAimingDownSight(ev);
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnAimingDownSight))),

                    // loads AdsIn
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AimingDownSightEventArgs), nameof(AimingDownSightEventArgs.AdsIn))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}