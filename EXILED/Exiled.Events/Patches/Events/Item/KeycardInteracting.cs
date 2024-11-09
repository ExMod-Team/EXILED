// -----------------------------------------------------------------------
// <copyright file="KeycardInteracting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Item
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pickups;
    using API.Features.Pools;

    using Exiled.Events;
    using Exiled.Events.EventArgs.Item;

    using Footprinting;

    using HarmonyLib;

    using Interactables.Interobjects.DoorUtils;

    using InventorySystem.Items;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using BaseKeycardPickup = InventorySystem.Items.Keycards.KeycardPickup;

    /// <summary>
    /// Patches <see cref="BaseKeycardPickup.ProcessCollision(Collision)"/> and adds <see cref="KeycardPickup.Permissions"/> implementation.
    /// Adds the <see cref="Handlers.Player.InteractingDoor"/> event.
    /// </summary>
    [HarmonyPatch(typeof(BaseKeycardPickup), nameof(BaseKeycardPickup.ProcessCollision))]
    internal static class KeycardInteracting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder isUnlocked = generator.DeclareLocal(typeof(bool));
            LocalBuilder notEmptyPermissions = generator.DeclareLocal(typeof(bool));
            LocalBuilder havePermissions = generator.DeclareLocal(typeof(bool));

            Label skip = generator.DefineLabel();
            Label ret = generator.DefineLabel();

            int offset = 1;
            int index = newInstructions.FindIndex(i => i.LoadsField(Field(typeof(DoorVariant), nameof(DoorVariant.ActiveLocks)))) + offset;

            newInstructions.RemoveAt(index);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // check and write door lock state (isUnlocked)
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Ceq),
                    new CodeInstruction(OpCodes.Stloc_S, isUnlocked.LocalIndex),
                });

            index = newInstructions.FindIndex(i => i.LoadsField(Field(typeof(DoorPermissions), nameof(DoorPermissions.RequiredPermissions)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // checking empty permissions
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Cgt),

                    new(OpCodes.Stloc_S, notEmptyPermissions.LocalIndex),
                    new(OpCodes.Br_S, skip),

                    // save original return
                    new CodeInstruction(OpCodes.Ret).MoveLabelsFrom(newInstructions[index + 1]),
                    new CodeInstruction(OpCodes.Nop).WithLabels(skip),
                });

            // 6 new instructions
            offset = 6;
            index += offset;

            newInstructions.RemoveRange(index, 14);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // override permissions check, to implement KeycardPickup::Permissions
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldloc_1),
                    new CodeInstruction(OpCodes.Call, Method(typeof(KeycardInteracting), nameof(KeycardInteracting.CheckPermissions))),
                    new CodeInstruction(OpCodes.Stloc_S, havePermissions.LocalIndex),
                });

            // 4 new instructions
            offset = 4;
            index += offset;

            newInstructions.RemoveRange(index, 2);

            offset = -5;
            index = newInstructions.FindIndex(i => i.Calls(PropertySetter(typeof(DoorVariant), nameof(DoorVariant.NetworkTargetState)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // pickup
                    new(OpCodes.Ldarg_0),

                    // PreviousOwner.Hub
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Ldflda, Field(typeof(BaseKeycardPickup), nameof(BaseKeycardPickup.PreviousOwner))),
                    new(OpCodes.Ldfld, Field(typeof(Footprint), nameof(Footprint.Hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // door
                    new(OpCodes.Ldloc_1),

                    // allowed calculate
                    new(OpCodes.Ldloc_S, isUnlocked),

                    new(OpCodes.Ldloc_S, havePermissions),

                    new(OpCodes.Ldloc_S, notEmptyPermissions),
                    new(OpCodes.Call, PropertyGetter(typeof(Events), nameof(Events.Instance))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Events), nameof(Events.Config))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Config), nameof(Config.CanKeycardThrowAffectDoors))),
                    new(OpCodes.Or),

                    new(OpCodes.And),
                    new(OpCodes.And),

                    // ThrowKeycardInteractingEventArgs ev = new(pickup, player, door, isAllowed);
                    //
                    // Item.OnThrowKeycardInteracting(ev);
                    //
                    // if (!ev.IsAllowed)
                    //     return;
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(KeycardInteractingEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnKeycardInteracting))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(KeycardInteractingEventArgs), nameof(KeycardInteractingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, ret),
                });

            newInstructions.InsertRange(
                newInstructions.Count - 1,
                new[]
                {
                    // call animation sets(we don't want to call the event more than once)
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new(OpCodes.Callvirt, Method(typeof(DoorVariant), nameof(DoorVariant.TargetStateChanged))),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static bool CheckPermissions(BaseKeycardPickup keycard, DoorVariant door)
        {
            DoorPermissions permissions = door.RequiredPermissions;
            if (permissions.RequiredPermissions == KeycardPermissions.None)
            {
                return true;
            }

            if (Pickup.Get(keycard) is KeycardPickup keycardPickup)
            {
                if (!permissions.RequireAll)
                {
                    return ((KeycardPermissions)keycardPickup.Permissions & permissions.RequiredPermissions) != 0;
                }

                return ((KeycardPermissions)keycardPickup.Permissions & permissions.RequiredPermissions) == permissions.RequiredPermissions;
            }

            return InventorySystem.InventoryItemLoader.AvailableItems.TryGetValue(keycard.Info.ItemId, out ItemBase itemBase) && permissions.CheckPermissions(itemBase, null);
        }
    }
}