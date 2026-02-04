// -----------------------------------------------------------------------
// <copyright file="ReceivingVoiceMessage.cs" company="ExMod Team">
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

    using VoiceChat.Networking;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="VoiceTransceiver.ServerReceiveMessage(NetworkConnection, VoiceMessage)"/>.
    /// Adds the <see cref="Handlers.Player.ReceivingVoiceMessage"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ReceivingVoiceMessage))]
    [HarmonyPatch(typeof(VoiceTransceiver), nameof(VoiceTransceiver.ServerReceiveMessage))]

    internal static class ReceivingVoiceMessage
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label skipHub = generator.DefineLabel();
            Label skipLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(ReceivingVoiceMessageEventArgs));

            int offset = -2;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Newobj && (ConstructorInfo)i.operand == GetDeclaredConstructors(typeof(LabApi.Events.Arguments.PlayerEvents.PlayerReceivingVoiceMessageEventArgs))[0]) + offset;

            newInstructions[index].labels.Add(skipLabel);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Player.Get(hub);
                new(OpCodes.Ldloc_S, 4),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // Player.Get(msg.Speaker);
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, Field(typeof(VoiceMessage), nameof(VoiceMessage.Speaker))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // voiceModule
                new(OpCodes.Ldloc_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(IVoiceRole), nameof(IVoiceRole.VoiceModule))),

                // msg
                new(OpCodes.Ldarg_1),

                // ReceivingVoiceMessageEventArgs ev = new(Player receiver, Player sender, VoiceModuleBase, VoiceMessage);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ReceivingVoiceMessageEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Player.OnVoiceChatting(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnReceivingVoiceMessage))),

                // if (!ev.IsAllowed)
                //    continue;
                new(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingVoiceMessageEventArgs), nameof(ReceivingVoiceMessageEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, skipHub),

                // msg = ev.VoiceMessage;
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingVoiceMessageEventArgs), nameof(ReceivingVoiceMessageEventArgs.VoiceMessage))),
                new(OpCodes.Starg_S, 1),
            });

            offset = 1;
            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Callvirt && i.operand is MethodInfo mi && mi.Name == nameof(NetworkConnection.Send) && mi.IsGenericMethod) + offset;

            newInstructions[index].labels.Add(skipHub);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}