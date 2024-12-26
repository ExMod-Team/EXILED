// -----------------------------------------------------------------------
// <copyright file="DoorList.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313
#pragma warning disable SA1402
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using API.Features;

    using Exiled.API.Features.Doors;
    using Exiled.API.Features.Pools;

    using HarmonyLib;

    using Interactables.Interobjects.DoorUtils;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="DoorVariant.RegisterRooms"/>.
    /// </summary>
    [HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.RegisterRooms))]
    internal class DoorList
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label ret = generator.DefineLabel();

            // if (Rooms != null)
            //     return;
            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DoorVariant), nameof(DoorVariant.Rooms))),
                    new(OpCodes.Brtrue_S, ret),
                });

            // DoorList.InitDoor(this);
            newInstructions.InsertRange(
                newInstructions.Count - 1,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, Method(typeof(DoorList), nameof(DoorList.InitDoor))),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void InitDoor(DoorVariant doorVariant)
        {
            if (Door.DoorVariantToDoor.ContainsKey(doorVariant))
                return;

            List<Room> rooms = doorVariant.Rooms.Select(identifier => Room.RoomIdentifierToRoom[identifier]).ToList();

            Door door = Door.Create(doorVariant, rooms);

            foreach (Room room in rooms)
            {
                room.DoorsValue.Add(door);
                room.NearestRoomsValue.AddRange(rooms.Except(new List<Room>() { room }));
            }

            if (door.Is(out CheckpointDoor checkpoint))
            {
                foreach (DoorVariant subDoor in checkpoint.Base.SubDoors)
                {
                    subDoor.RegisterRooms();
                    BreakableDoor targetDoor = Door.Get<BreakableDoor>(subDoor);

                    targetDoor.ParentCheckpointDoor = checkpoint;
                    checkpoint.SubDoorsValue.Add(targetDoor);
                }
            }

            return;
        }
    }

    /// <summary>
    /// Patches <see cref="DoorVariant.OnDestroy"/>.
    /// </summary>
    [HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.OnDestroy))]
    internal class DoorListRemove
    {
        private static void Prefix(DoorVariant __instance)
        {
            Door.DoorVariantToDoor.Remove(__instance);
        }
    }
}