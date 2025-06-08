// -----------------------------------------------------------------------
// <copyright file="KeycardInteracting.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
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
    using Attributes;
    using Exiled.Events.EventArgs.Item;

    using Footprinting;

    using HarmonyLib;

    using Interactables.Interobjects.DoorUtils;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using BaseKeycardPickup = InventorySystem.Items.Keycards.KeycardPickup;

    /// <summary>
    /// Patches <see cref="BaseKeycardPickup.ProcessCollision(Collision)"/> and adds <see cref="KeycardPickup.Permissions"/> implementation.
    /// Adds the <see cref="Handlers.Player.InteractingDoor"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Item), nameof(Handlers.Item.KeycardInteracting))]
    [HarmonyPatch(typeof(BaseKeycardPickup), nameof(BaseKeycardPickup.ProcessCollision))]
    internal static class KeycardInteracting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder isUnlocked = generator.DeclareLocal(typeof(bool));
            LocalBuilder hasPermission = generator.DeclareLocal(typeof(bool));

            Label ret = generator.DefineLabel();

            int offset = 1;
            int index = newInstructions.FindIndex(i => i.LoadsField(Field(typeof(DoorVariant), nameof(DoorVariant.ActiveLocks)))) + offset;

            Label continueLabel = (Label)newInstructions[index].operand;

            newInstructions.RemoveAt(index);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // check and write door lock state (isUnlocked)
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Ceq),
                    new(OpCodes.Stloc_S, isUnlocked.LocalIndex),
                    new(OpCodes.Br, continueLabel),
                });

            offset = 1;
            index = newInstructions.FindIndex(i => i.Calls(Method(
                typeof(DoorPermissionsPolicyExtensions),
                nameof(DoorPermissionsPolicyExtensions.CheckPermissions),
                new[] { typeof(IDoorPermissionRequester), typeof(IDoorPermissionProvider), typeof(PermissionUsed).MakeByRefType() }))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
            {
                // hasPermission
                new(OpCodes.Stloc_S, hasPermission.LocalIndex),

                // pickup
                new(OpCodes.Ldarg_0),

                // PreviousOwner.Hub
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldflda, Field(typeof(BaseKeycardPickup), nameof(BaseKeycardPickup.PreviousOwner))),
                new(OpCodes.Ldfld, Field(typeof(Footprint), nameof(Footprint.Hub))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // door
                new(OpCodes.Ldloc_1),

                // isAllowed = isUnlocked && hasPermission
                new(OpCodes.Ldloc_S, isUnlocked.LocalIndex),
                new(OpCodes.Ldloc_S, hasPermission.LocalIndex),
                new(OpCodes.And),

                // ev = new KeycardInteractingEventArgs(pickup, player, door, isAllowed)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(KeycardInteractingEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnKeycardInteracting))),

                new(OpCodes.Callvirt, PropertyGetter(typeof(KeycardInteractingEventArgs), nameof(KeycardInteractingEventArgs.IsAllowed))),
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
    }
}