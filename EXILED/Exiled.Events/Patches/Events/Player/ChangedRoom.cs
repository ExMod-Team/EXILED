// -----------------------------------------------------------------------
// <copyright file="ChangedRoom.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using MapGeneration;
    using PlayerRoles;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="CurrentRoomPlayerCache.ValidateCache"/> to add the <see cref="Player.RoomChanged"/> event.
    /// </summary>
    [EventPatch(typeof(Player), nameof(Player.RoomChanged))]
    [HarmonyPatch(typeof(CurrentRoomPlayerCache), nameof(CurrentRoomPlayerCache.ValidateCache))]
    internal class ChangedRoom
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label jump = generator.DefineLabel();
            int offset = 2;
            int index = newInstructions.FindIndex(x => x.Calls(Method(typeof(Object), "op_Inequality", new[] { typeof(Object), typeof(Object) }))) + offset;

            newInstructions[index].labels.Add(jump);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // referenceHub
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(CurrentRoomPlayerCache), nameof(CurrentRoomPlayerCache._roleManager))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(PlayerRoleManager), nameof(PlayerRoleManager.Hub))),

                // oldRoom
                new(OpCodes.Ldloc_2),

                // newRoom
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(CurrentRoomPlayerCache), nameof(CurrentRoomPlayerCache._lastDetected))),

                // RoomChangedEventArgs ev = new RoomChangedEventArgs(hub, oldRoom, newRoom);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RoomChangedEventArgs))[0]),

                // Handlers.Player.OnRoomChanged(ev);
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnRoomChanged))),
            });

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
