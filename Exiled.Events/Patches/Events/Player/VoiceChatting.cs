// -----------------------------------------------------------------------
// <copyright file="VoiceChatting.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using API.Features.Pools;

    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using Mirror;

    using PlayerRoles.Voice;

    using VoiceChat;
    using VoiceChat.Networking;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="VoiceTransceiver.ServerReceiveMessage(NetworkConnection, VoiceMessage)"/>.
    /// Adds the <see cref="Handlers.Player.VoiceChatting"/> event.
    /// Adds the <see cref="Handlers.Player.Transmitting"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.VoiceChatting))]
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Transmitting))]
    [HarmonyPatch(typeof(VoiceTransceiver), nameof(VoiceTransceiver.ServerReceiveMessage))]
    internal static class VoiceChatting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();
            Label skipLabel = generator.DefineLabel();

            LocalBuilder player = generator.DeclareLocal(typeof(API.Features.Player));
            LocalBuilder voiceModule = generator.DeclareLocal(typeof(VoiceModuleBase));
            LocalBuilder evTransmitting = generator.DeclareLocal(typeof(TransmittingEventArgs));
            LocalBuilder evVoiceChatting = generator.DeclareLocal(typeof(VoiceChattingEventArgs));

            int offset = -1;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Newobj && (ConstructorInfo)i.operand == GetDeclaredConstructors(typeof(LabApi.Events.Arguments.PlayerEvents.PlayerSendingVoiceMessageEventArgs))[0]) + offset;

            newInstructions[index].labels.Add(skipLabel);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Player.Get(msg.Speaker);
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, Field(typeof(VoiceMessage), nameof(VoiceMessage.Speaker))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, player.LocalIndex),

                // voiceModule
                new(OpCodes.Ldloc_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(IVoiceRole), nameof(IVoiceRole.VoiceModule))),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, voiceModule.LocalIndex),

                // msg
                new(OpCodes.Ldarg_1),

                // VoiceChattingEventArgs ev = new(Player, VoiceModuleBase, VoiceMessage);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(VoiceChattingEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, evVoiceChatting.LocalIndex),

                // Handlers.Player.OnVoiceChatting(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnVoiceChatting))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(VoiceChattingEventArgs), nameof(VoiceChattingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),

                // msg = ev.VoiceMessage;
                new(OpCodes.Ldloc_S, evVoiceChatting.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(VoiceChattingEventArgs), nameof(VoiceChattingEventArgs.VoiceMessage))),
                new(OpCodes.Starg_S, 1),

                // if(voiceModule.CurrentChannel != VoiceChatChannel.Radio)
                //     goto skipLabel;
                new(OpCodes.Ldloc_S, voiceModule.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(VoiceModuleBase), nameof(VoiceModuleBase.CurrentChannel))),
                new(OpCodes.Ldc_I4_S, (sbyte)VoiceChatChannel.Radio),
                new(OpCodes.Ceq),
                new(OpCodes.Brfalse_S, skipLabel),

                // player
                new(OpCodes.Ldloc_S, player.LocalIndex),

                // msg
                new(OpCodes.Ldarg_1),

                // voiceModule
                new(OpCodes.Ldloc_S, voiceModule.LocalIndex),

                // TransmittingEventArgs ev = new TransmittingEventArgs(Player, VoiceMessage, VoiceModuleBase)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(TransmittingEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, evTransmitting.LocalIndex),

                // Handlers.Player.OnTransmitting(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnTransmitting))),

                // if(!ev.IsAllowed)
                //     return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(TransmittingEventArgs), nameof(TransmittingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),

                // msg = ev.VoiceMessage;
                new(OpCodes.Ldloc_S, evTransmitting.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(TransmittingEventArgs), nameof(TransmittingEventArgs.VoiceMessage))),
                new(OpCodes.Starg_S, 1),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}