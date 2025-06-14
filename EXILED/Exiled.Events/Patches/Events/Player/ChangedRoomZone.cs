// -----------------------------------------------------------------------
// <copyright file="ChangedRoomZone.cs" company="ExMod Team">
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
    /// Patches <see cref="CurrentRoomPlayerCache.ValidateCache"/> to add the <see cref="Player.RoomChanged"/> and <see cref="Player.ZoneChanged"/> events.
    /// </summary>
    [EventPatch(typeof(Player), nameof(Player.RoomChanged))]
    [EventPatch(typeof(Player), nameof(Player.ZoneChanged))]
    [HarmonyPatch(typeof(CurrentRoomPlayerCache), nameof(CurrentRoomPlayerCache.ValidateCache))]
    internal class ChangedRoomZone
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder hub = generator.DeclareLocal(typeof(ReferenceHub));
            LocalBuilder oldRoom = generator.DeclareLocal(typeof(RoomIdentifier));
            LocalBuilder newRoom = generator.DeclareLocal(typeof(RoomIdentifier));

            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldloca_S);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // oldRoom = this._lastDetected
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(CurrentRoomPlayerCache), nameof(CurrentRoomPlayerCache._lastDetected))),
                new(OpCodes.Stloc_S, oldRoom),
            });

            int lastIndex = newInstructions.Count - 1;

            newInstructions[lastIndex].WithLabels(returnLabel);

            newInstructions.InsertRange(lastIndex, new CodeInstruction[]
            {
                // newRoom = this._lastDetected
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(CurrentRoomPlayerCache), nameof(CurrentRoomPlayerCache._lastDetected))),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, newRoom),

                new(OpCodes.Ldloc_S, oldRoom),

                // if (oldRoom == newRoom) return;
                new(OpCodes.Call, Method(typeof(object), nameof(object.Equals), new[] { typeof(object), typeof(object) })),
                new(OpCodes.Brtrue_S, returnLabel),

                // ReferenceHub hub = this._roleManager.gameObject.GetComponent<ReferenceHub>();
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(CurrentRoomPlayerCache), nameof(CurrentRoomPlayerCache._roleManager))),
                new(OpCodes.Call, Method(typeof(Component), nameof(Component.GetComponent)).MakeGenericMethod(typeof(ReferenceHub))),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, hub),

                // oldRoom
                new(OpCodes.Ldloc_S, oldRoom),

                // newRoom
                new(OpCodes.Ldloc_S, newRoom),

                // Handlers.Player.OnRoomChanged(new RoomChangedEventArgs(hub, oldRoom, newRoom));
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RoomChangedEventArgs))[0]),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnRoomChanged))),

                // oldRoom.Zone
                new(OpCodes.Ldloc_S, oldRoom),
                new(OpCodes.Ldfld, Field(typeof(RoomIdentifier), nameof(RoomIdentifier.Zone))),

                // newRoom.Zone
                new(OpCodes.Ldloc_S, newRoom),
                new(OpCodes.Ldfld, Field(typeof(RoomIdentifier), nameof(RoomIdentifier.Zone))),

                // if (oldRoom.Zone == newRoom.Zone) return;
                new(OpCodes.Ceq),
                new(OpCodes.Brtrue_S, returnLabel),

                // hub
                new(OpCodes.Ldloc_S, hub),

                // oldRoom
                new(OpCodes.Ldloc_S, oldRoom),

                // newRoom
                new(OpCodes.Ldloc_S, newRoom),

                // Handlers.Player.OnZoneChanged(new ZoneChangedEventArgs(hub, oldRoom, newRoom));
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ZoneChangedEventArgs))[0]),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnZoneChanged))),
            });

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
