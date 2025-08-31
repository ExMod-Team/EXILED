// -----------------------------------------------------------------------
// <copyright file="VoiceChatting.cs" company="ExMod Team">
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

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using Mirror;

    using PlayerRoles.Voice;

    using VoiceChat.Networking;

    using static HarmonyLib.AccessTools;
    using static VoiceChat.Networking.VoiceTransceiver;

    /// <summary>
    /// Patches <see cref="VoiceTransceiver.ServerReceiveMessage(NetworkConnection, VoiceMessage)"/>.
    /// Adds the <see cref="Handlers.Player.SendingVoiceMessage"/> event.
    /// Adds the <see cref="Handlers.Player.ReceivingVoiceMessage"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.SendingVoiceMessage))]
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ReceivingVoiceMessage))]
    [HarmonyPatch(typeof(VoiceTransceiver), nameof(VoiceTransceiver.ServerReceiveMessage))]
    internal static class VoiceChatting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder evSending = generator.DeclareLocal(typeof(SendingVoiceMessageEventArgs));
            LocalBuilder evReceiving = generator.DeclareLocal(typeof(ReceivingVoiceMessageEventArgs));

            Label retLabel = generator.DefineLabel();
            Label skipLabel = generator.DefineLabel();

            const int offset = 1;
            int index = newInstructions.FindIndex(i => i.Calls(PropertySetter(typeof(VoiceModuleBase), nameof(VoiceModuleBase.CurrentChannel)))) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // msg
                new(OpCodes.Ldarg_1),

                // SendingVoiceMessageEventArgs ev = new(VoiceMessage);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SendingVoiceMessageEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, evSending.LocalIndex),

                // Handlers.Player.OnSendingVoiceMessage(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnSendingVoiceMessage))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(SendingVoiceMessageEventArgs), nameof(SendingVoiceMessageEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),

                // msg = ev.VoiceMessage;
                new(OpCodes.Ldloc_S, evSending.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(SendingVoiceMessageEventArgs), nameof(SendingVoiceMessageEventArgs.VoiceMessage))),
                new(OpCodes.Starg_S, 1),
            });

            index = newInstructions.FindIndex(i => i.Calls(Method(typeof(VoiceMessageReceiving), nameof(VoiceMessageReceiving.Invoke)))) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // msg
                new(OpCodes.Ldarg_1),

                // allHub;
                new(OpCodes.Ldloc_S, 4),

                // ReceivingVoiceMessageEventArgs ev = new(msg, allHub);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ReceivingVoiceMessageEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, evReceiving.LocalIndex),

                // Handlers.Player.OnReceivingVoiceMessage(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnReceivingVoiceMessage))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingVoiceMessageEventArgs), nameof(ReceivingVoiceMessageEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, skipLabel),

                // msg = ev.VoiceMessage;
                new(OpCodes.Ldloc_S, evReceiving.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingVoiceMessageEventArgs), nameof(ReceivingVoiceMessageEventArgs.VoiceMessage))),
                new(OpCodes.Starg_S, 1),
            });

            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Callvirt && i.operand is MethodInfo mi && mi.Name == nameof(NetworkConnection.Send) && mi.IsGenericMethod) + offset;

            newInstructions[index].labels.Add(skipLabel);
            newInstructions[newInstructions.Count - 1].WithLabels(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
