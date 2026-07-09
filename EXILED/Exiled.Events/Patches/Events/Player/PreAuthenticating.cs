// -----------------------------------------------------------------------
// <copyright file="PreAuthenticating.cs" company="ExMod Team">
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

    using LiteNetLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="CustomLiteNetLib4MirrorTransport.ProcessConnectionRequest(ConnectionRequest)" />.
    /// Adds the <see cref="Handlers.Player.PreAuthenticating" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.PreAuthenticating))]
    [HarmonyPatch(typeof(CustomLiteNetLib4MirrorTransport), nameof(CustomLiteNetLib4MirrorTransport.ProcessConnectionRequest))]
    internal static class PreAuthenticating
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldstr && instruction.operand == (object)"{0};{1};{2};{3}");

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // userid
                    new CodeInstruction(OpCodes.Ldloc_S, 10),

                    // ipaddress
                    new (OpCodes.Ldloc_S, 15),

                    // expiration
                    new (OpCodes.Ldloc_S, 11),

                    // flags
                    new (OpCodes.Ldloc_S, 17),

                    // country
                    new (OpCodes.Ldloc_S, 13),

                    // signature
                    new (OpCodes.Ldloc_S, 14),

                    // request
                    new (OpCodes.Ldarg_1),

                    // position
                    new (OpCodes.Ldloc_S, 9),

                    // PreAuthenticatingEventArgs ev = new (userid, ipaddress, expiration, flags, country, signature, request, position)
                    new (OpCodes.Newobj, GetDeclaredConstructors(typeof(PreAuthenticatingEventArgs))[0]),

                    // OnPreAuthenticating(ev)
                    new (OpCodes.Call, AccessTools.Method(typeof(Handlers.Player), nameof(Handlers.Player.OnPreAuthenticating))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
