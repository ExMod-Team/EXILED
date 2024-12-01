// -----------------------------------------------------------------------
// <copyright file="ChangingRoleAndSpawned.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using API.Features.Roles;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using InventorySystem;
    using InventorySystem.Configs;
    using InventorySystem.Items;
    using InventorySystem.Items.Armor;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.Usables.Scp1344;
    using Mirror;

    using PlayerRoles;

    using static HarmonyLib.AccessTools;
    using static UnityEngine.GraphicsBuffer;

    using Player = Handlers.Player;

    /// <summary>
    /// Patches <see cref="PlayerRoleManager.InitializeNewRole(RoleTypeId, RoleChangeReason, RoleSpawnFlags, Mirror.NetworkReader)" />
    /// Adds the <see cref="Player.ChangingRole" /> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerRoleManager), nameof(PlayerRoleManager.InitializeNewRole))]
    internal static class ChangingRoleAndSpawned
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();
            Label continueLabel = generator.DefineLabel();
            Label jmp = generator.DefineLabel();

            LocalBuilder changingRoleEventArgs = generator.DeclareLocal(typeof(ChangingRoleEventArgs));
            LocalBuilder player = generator.DeclareLocal(typeof(API.Features.Player));

            newInstructions.InsertRange(
                0,
                new[]
                {
                    // player = Player.Get(this._hub)
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(PlayerRoleManager), nameof(PlayerRoleManager.Hub))),
                    new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Stloc_S, player.LocalIndex),

                    // if (Player.IsVerified)
                    //  goto jmp
                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.IsVerified))),
                    new(OpCodes.Brtrue_S, jmp),

                    // if (!Player.IsNpc)
                    //  goto continueLabel;
                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.IsNPC))),
                    new(OpCodes.Brfalse_S, continueLabel),

                    // jmp
                    // player
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex).WithLabels(jmp),

                    // newRole
                    new(OpCodes.Ldarg_1),

                    // reason
                    new(OpCodes.Ldarg_2),

                    // spawnFlags
                    new(OpCodes.Ldarg_3),

                    // ChangingRoleEventArgs changingRoleEventArgs = new(Player, RoleTypeId, RoleChangeReason, SpawnFlags)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingRoleEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, changingRoleEventArgs.LocalIndex),

                    // Handlers.Player.OnChangingRole(changingRoleEventArgs)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnChangingRole))),

                    // if (!changingRoleEventArgs.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // newRole = changingRoleEventArgs.NewRole;
                    new(OpCodes.Ldloc_S, changingRoleEventArgs.LocalIndex),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.NewRole))),
                    new(OpCodes.Starg_S, 1),

                    // reason = changingRoleEventArgs.Reason
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.Reason))),
                    new(OpCodes.Starg_S, 2),

                    // spawnFlags = changingRoleEventArgs.SpawnFlags
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.SpawnFlags))),
                    new(OpCodes.Starg_S, 3),

                    // UpdatePlayerRole(changingRoleEventArgs.NewRole, changingRoleEventArgs.Player)
                    new(OpCodes.Ldloc_S, changingRoleEventArgs.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.NewRole))),
                    new(OpCodes.Ldloc_S, changingRoleEventArgs.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.Player))),
                    new(OpCodes.Call, Method(typeof(ChangingRoleAndSpawned), nameof(UpdatePlayerRole))),

                    new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
                });

            int offset = 1;
            int index = newInstructions.FindIndex(
                instruction => instruction.Calls(Method(typeof(GameObjectPools.PoolObject), nameof(GameObjectPools.PoolObject.SetupPoolObject)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // player.Role = Role.Create(roleBase);
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Ldloc_2),
                    new(OpCodes.Call, Method(typeof(Role), nameof(Role.Create))),
                    new(OpCodes.Callvirt, PropertySetter(typeof(API.Features.Player), nameof(API.Features.Player.Role))),
                });

            offset = 1;
            index = newInstructions.FindIndex(i => i.Calls(Method(typeof(PlayerRoleManager.RoleChanged), nameof(PlayerRoleManager.RoleChanged.Invoke)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // changingRoleEventArgs
                    new(OpCodes.Ldloc_S, changingRoleEventArgs.LocalIndex),

                    // ChangingRole.ChangeInventory(changingRoleEventArgs, oldRoleType);
                    new(OpCodes.Call, Method(typeof(ChangingRoleAndSpawned), nameof(ChangeInventory))),

                    // invoke OnSpawned
                    // player
                    new(OpCodes.Ldloc_S, player.LocalIndex),

                    // OldRole
                    new(OpCodes.Ldloc_0),

                    // SpawnedEventArgs spawnedEventArgs = new(Player, OldRole)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawnedEventArgs))[0]),

                    // Handlers.Player.OnSpawned(spawnedEventArgs)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnSpawned))),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void UpdatePlayerRole(RoleTypeId newRole, API.Features.Player player)
        {
            if (newRole is RoleTypeId.Scp173)
                Scp173Role.TurnedPlayers.Remove(player);

            player.MaxHealth = default;
        }

        private static void ChangeInventory(ChangingRoleEventArgs ev)
        {
            try
            {
                if (!NetworkServer.active || ev == null || !ev.SpawnFlags.HasFlag(RoleSpawnFlags.AssignInventory))
                {
                    return;
                }

                Inventory inventory = ev.Player.Inventory;
                bool flag = InventoryItemProvider.KeepItemsAfterEscaping && ev.Reason == API.Enums.SpawnReason.Escaped;
                if (flag)
                {
                    List<ItemPickupBase> list = new List<ItemPickupBase>();
                    if (inventory.TryGetBodyArmor(out BodyArmor bodyArmor))
                    {
                        bodyArmor.DontRemoveExcessOnDrop = true;
                    }

                    HashSet<ushort> hashSet = HashSetPool<ushort>.Pool.Get();
                    foreach (KeyValuePair<ushort, ItemBase> item2 in inventory.UserInventory.Items)
                    {
                        if (item2.Value is Scp1344Item scp1344Item)
                        {
                            scp1344Item.Status = Scp1344Status.Idle;
                        }
                        else
                        {
                            hashSet.Add(item2.Key);
                        }
                    }

                    foreach (ushort item3 in hashSet)
                    {
                        list.Add(inventory.ServerDropItem(item3));
                    }

                    HashSetPool<ushort>.Pool.Return(hashSet);
                    InventoryItemProvider.PreviousInventoryPickups[ev.Player.ReferenceHub] = list;
                }

                if (!flag)
                {
                    while (inventory.UserInventory.Items.Count > 0)
                    {
                        inventory.ServerRemoveItem(inventory.UserInventory.Items.ElementAt(0).Key, null);
                    }

                    inventory.UserInventory.ReserveAmmo.Clear();
                    inventory.SendAmmoNextFrame = true;
                }

                if (!StartingInventories.DefinedInventories.TryGetValue(ev.NewRole, out InventoryRoleInfo value))
                {
                    return;
                }

                foreach (KeyValuePair<ItemType, ushort> item in value.Ammo)
                {
                    inventory.ServerAddAmmo(item.Key, item.Value);
                }

                for (int i = 0; i < value.Items.Length; i++)
                {
                    ItemBase arg = inventory.ServerAddItem(value.Items[i], ItemAddReason.StartingItem, 0);
                    InventoryItemProvider.OnItemProvided?.Invoke(ev.Player.ReferenceHub, arg);
                }

                InventoryItemProvider.InventoriesToReplenish.Enqueue(ev.Player.ReferenceHub);
            }
            catch (Exception exception)
            {
                Log.Error($"{nameof(ChangingRoleAndSpawned)}.{nameof(ChangeInventory)}: {exception}");
            }
        }
    }
}
