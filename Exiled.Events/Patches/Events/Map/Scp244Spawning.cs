// -----------------------------------------------------------------------
// <copyright file="Scp244Spawning.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;
    using HarmonyLib;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.Usables.Scp244;
    using MapGeneration;
    using Mirror;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp244Spawner.SpawnScp244" />.
    /// Adds the <see cref="Handlers.Map.Scp244Spawning" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Map), nameof(Handlers.Map.Scp244Spawning))]
    [HarmonyPatch(typeof(Scp244Spawner), nameof(Scp244Spawner.SpawnScp244))]
    internal static class Scp244Spawning
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder pickup = generator.DeclareLocal(typeof(ItemPickupBase));

            Label continueLabel = generator.DefineLabel();

            int offset = -2;
            int index = newInstructions.FindIndex(i => i.Calls(Method(typeof(NetworkServer), nameof(NetworkServer.Spawn), new[] { typeof(GameObject), typeof(NetworkConnection) }))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // save Pickup from the stack
                new CodeInstruction(OpCodes.Stloc_S, pickup.LocalIndex).MoveLabelsFrom(newInstructions[index]),

                // Scp244Spawner.CompatibleRooms[num]
                new(OpCodes.Ldsfld, Field(typeof(Scp244Spawner), nameof(Scp244Spawner.CompatibleRooms))),
                new(OpCodes.Ldloc_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(List<RoomIdentifier>), "Item")),

                // scp244DeployablePickup
                new(OpCodes.Ldloc_2),

                // Scp244SpawningEventArgs ev = new(Room, Scp244DeployablePickup)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(Scp244SpawningEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.Map), nameof(Handlers.Map.OnScp244Spawning))),

                // if (ev.IsAllowed) goto continueLabel;
                new(OpCodes.Call, PropertyGetter(typeof(Scp244SpawningEventArgs), nameof(Scp244SpawningEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, continueLabel),

                // scp244DeployablePickup.gameObject.Destroy()
                // return;
                new(OpCodes.Ldloc_2),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ItemPickupBase), nameof(ItemPickupBase.gameObject))),
                new(OpCodes.Call, Method(typeof(NetworkServer), nameof(NetworkServer.Destroy))),
                new(OpCodes.Ret),

                // load pickup back into the stack
                new CodeInstruction(OpCodes.Ldloc_S, pickup.LocalIndex).WithLabels(continueLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}