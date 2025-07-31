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

            Label returnLabel = generator.DefineLabel();

            LocalBuilder oldRoom = generator.DeclareLocal(typeof(RoomIdentifier));
            LocalBuilder newRoom = generator.DeclareLocal(typeof(RoomIdentifier));

            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldloca_S);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // RoomIdentifier oldRoom = this._lastDetected
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(CurrentRoomPlayerCache), nameof(CurrentRoomPlayerCache._lastDetected))),
                new(OpCodes.Stloc_S, oldRoom),
            });

            int lastIndex = newInstructions.Count - 1;

            newInstructions[lastIndex].WithLabels(returnLabel);

            newInstructions.InsertRange(lastIndex, new CodeInstruction[]
            {
                // newRoom = lastDetected
                new(OpCodes.Ldloc_1),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, newRoom),

                // oldRoom
                new(OpCodes.Ldloc_S, oldRoom),

                // if (oldRoom == newRoom) return;
                new(OpCodes.Call, Method(typeof(object), nameof(object.ReferenceEquals), new[] { typeof(object), typeof(object) })),
                new(OpCodes.Brtrue_S, returnLabel),

                // this._roleManager.gameObject.GetComponent<ReferenceHub>();
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(CurrentRoomPlayerCache), nameof(CurrentRoomPlayerCache._roleManager))),
                new(OpCodes.Call, Method(typeof(Component), nameof(Component.GetComponent)).MakeGenericMethod(typeof(ReferenceHub))),

                // oldRoom
                new(OpCodes.Ldloc_S, oldRoom),

                // newRoom
                new(OpCodes.Ldloc_S, newRoom),

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
