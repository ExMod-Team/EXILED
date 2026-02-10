// -----------------------------------------------------------------------
// <copyright file="ReceivingGunSound.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
namespace Exiled.Events.Patches.Events.Player
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
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
    /// Patches AudioModule.ServerSendToNearbyPlayers to add <see cref="Handlers.Player.ReceivingGunSound" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ReceivingGunSound))]
    [HarmonyPatch]
    public class ReceivingGunSound
    {
        private static Type displayClassType;
        private static Type displayClassTypeForeach;

        private static MethodBase TargetMethod()
        {
            Type innerType = Inner(typeof(AudioModule), "<>c__DisplayClass31_1");

            return Method(innerType, "<ServerSendToNearbyPlayers>b__0");
        }

        private static bool Prepare()
        {
            displayClassType = Inner(typeof(AudioModule), "<>c__DisplayClass31_0");
            displayClassTypeForeach = Inner(typeof(AudioModule), "<>c__DisplayClass31_1");

            if (displayClassType == null || displayClassTypeForeach == null)
            {
                Log.Error("`<>c__DisplayClass31` _1 or _0 cannot found on ReceivingGunSound class. Class changed skipping patch.");
                return false;
            }

            return true;
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(ReceivingGunSoundEventArgs));

            Label ret = generator.DefineLabel();

            int offset = 1;
            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Stloc_0) + offset;

            newInstructions.InsertRange(
                0,
                [

                    // this.receiver
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(displayClassTypeForeach, "receiver")),

                    // this.locals1.Firearm
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(displayClassTypeForeach, "CS$<>8__locals1")),
                    new(OpCodes.Ldfld, Field(displayClassType, "<>4__this")),
                    new(OpCodes.Call, PropertyGetter(typeof(FirearmSubcomponentBase), nameof(FirearmSubcomponentBase.Firearm))),

                    // this.locals1.index
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(displayClassTypeForeach, "CS$<>8__locals1")),
                    new(OpCodes.Ldfld, Field(displayClassType, "index")),

                    // this.locals1.channel
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(displayClassTypeForeach, "CS$<>8__locals1")),
                    new(OpCodes.Ldfld, Field(displayClassType, "channel")),

                    // this.locals1.audioRange
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(displayClassTypeForeach, "CS$<>8__locals1")),
                    new(OpCodes.Ldfld, Field(displayClassType, "audioRange")),

                    // this.locals1.pitch
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(displayClassTypeForeach, "CS$<>8__locals1")),
                    new(OpCodes.Ldfld, Field(displayClassType, "pitch")),

                    // this.locals1.ownPos
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(displayClassTypeForeach, "CS$<>8__locals1")),
                    new(OpCodes.Ldfld, Field(displayClassType, "ownPos")),

                    // visibility flag
                    new(OpCodes.Ldloc_0),

                    // new(receiver, firearm, audioIndex, mixerChannel, range, pitch, ownPos, isSenderVisible)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ReceivingGunSoundEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),

                    // Handlers.Player.OnReceivingGunSound(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnReceivingGunSound))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingGunSoundEventArgs), nameof(ReceivingGunSoundEventArgs.IsAllowed))),
                    new CodeInstruction(OpCodes.Brfalse_S, ret),
                ]);

            offset = -2;
            const int count = 3;

            // index = ev.AudioIndex;
            index = newInstructions.FindLastIndex(x => x.LoadsField(Field(displayClassType, "index"))) + offset;
            newInstructions.RemoveRange(index, count);
            newInstructions.InsertRange(
                index,
                [
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingGunSoundEventArgs), nameof(ReceivingGunSoundEventArgs.AudioIndex))),
                ]);

            // pitch = ev.Pitch;
            index = newInstructions.FindLastIndex(x => x.LoadsField(Field(displayClassType, "pitch"))) + offset;
            newInstructions.RemoveRange(index, count);
            newInstructions.InsertRange(
                index,
                [
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingGunSoundEventArgs), nameof(ReceivingGunSoundEventArgs.Pitch))),
                ]);

            // channel = ev.MixerChannel;
            index = newInstructions.FindLastIndex(x => x.LoadsField(Field(displayClassType, "channel"))) + offset;
            newInstructions.RemoveRange(index, count);
            newInstructions.InsertRange(
                index,
                [
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingGunSoundEventArgs), nameof(ReceivingGunSoundEventArgs.MixerChannel))),
                ]);

            // audioRange = ev.Range;
            index = newInstructions.FindLastIndex(x => x.LoadsField(Field(displayClassType, "audioRange"))) + offset;
            newInstructions.RemoveRange(index, count);
            newInstructions.InsertRange(
                index,
                [
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingGunSoundEventArgs), nameof(ReceivingGunSoundEventArgs.Range))),
                ]);

            // ownPos = ev.SenderPosition;
            index = newInstructions.FindLastIndex(x => x.LoadsField(Field(displayClassType, "ownPos"))) + offset;
            newInstructions.RemoveRange(index, count);
            newInstructions.InsertRange(
                index,
                [
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingGunSoundEventArgs), nameof(ReceivingGunSoundEventArgs.SenderPosition))),
                ]);

            newInstructions[^1].WithLabels(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}