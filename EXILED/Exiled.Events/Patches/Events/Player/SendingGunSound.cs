// -----------------------------------------------------------------------
// <copyright file="SendingGunSound.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
namespace Exiled.Events.Patches.Events.Player
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.Modules;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches AudioModule.ServerSendToNearbyPlayers to add <see cref="Handlers.Player.SendingGunSound" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.SendingGunSound))]
    [HarmonyPatch(typeof(AudioModule), nameof(AudioModule.ServerSendToNearbyPlayers))]
    public class SendingGunSound
    {
        private static Type displayClassType;

        private static bool Prepare()
        {
            displayClassType = Inner(typeof(AudioModule), "<>c__DisplayClass31_0");

            if (displayClassType == null)
            {
                Log.Error("`<>c__DisplayClass31_0` cannot found on SendingGunSound class. Class changed skipping patch.");
                return false;
            }

            return true;
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(SendingGunSoundEventArgs));

            Label ret = generator.DefineLabel();

            int index = newInstructions.FindLastIndex(x => x.Calls(PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.AllHubs))));

            newInstructions.InsertRange(
                index,
                [

                    // this.Firearm
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(FirearmSubcomponentBase), nameof(FirearmSubcomponentBase.Firearm))),

                    // index
                    new(OpCodes.Ldarg_1),

                    // channel
                    new(OpCodes.Ldarg_2),

                    // audioRange
                    new(OpCodes.Ldarg_3),

                    // pitch
                    new(OpCodes.Ldarg_S, 4),

                    // new(firearm, audioIndex, mixerChannel, range, pitch)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SendingGunSoundEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),

                    // Handlers.Player.OnSendingGunShotSound(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnSendingGunSound))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SendingGunSoundEventArgs), nameof(SendingGunSoundEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, ret),

                    // index = ev.AudioIndex;
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SendingGunSoundEventArgs), nameof(SendingGunSoundEventArgs.AudioIndex))),
                    new(OpCodes.Stfld, Field(displayClassType, "index")),

                    // channel = ev.MixerChannel;
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SendingGunSoundEventArgs), nameof(SendingGunSoundEventArgs.MixerChannel))),
                    new(OpCodes.Stfld, Field(displayClassType, "channel")),

                    // audioRange = ev.Range;
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SendingGunSoundEventArgs), nameof(SendingGunSoundEventArgs.Range))),
                    new(OpCodes.Stfld, Field(displayClassType, "audioRange")),
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SendingGunSoundEventArgs), nameof(SendingGunSoundEventArgs.Range))),
                    new(OpCodes.Ldc_R4, AudioModule.SendDistanceBuffer),
                    new(OpCodes.Add),
                    new(OpCodes.Dup),
                    new(OpCodes.Mul),
                    new(OpCodes.Stloc_2),

                    // pitch = ev.Pitch;
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SendingGunSoundEventArgs), nameof(SendingGunSoundEventArgs.Pitch))),
                    new CodeInstruction(OpCodes.Stfld, Field(displayClassType, "pitch")),
                ]);

            newInstructions[^1].WithLabels(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
