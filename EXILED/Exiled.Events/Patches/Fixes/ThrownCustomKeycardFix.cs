// -----------------------------------------------------------------------
// <copyright file="ThrownCustomKeycardFix.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Extensions;
    using Exiled.API.Features.Items.Keycards;
    using Exiled.API.Features.Pools;
    using HarmonyLib;
    using Interactables.Interobjects.DoorUtils;
    using InventorySystem.Items.Keycards;
    using InventorySystem.Items.Pickups;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="KeycardPickup.ProcessCollision"/> to fix custom keycards with custom permissions not working.
    /// Bug reported to NW (https://git.scpslgame.com/northwood-qa/scpsl-bug-reporting/-/issues/1625).
    /// </summary>
    [HarmonyPatch(typeof(KeycardPickup), nameof(KeycardPickup.ProcessCollision))]
    public class ThrownCustomKeycardFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder permissions = generator.DeclareLocal(typeof(DoorPermissionFlags));

            LocalBuilder type = generator.DeclareLocal(typeof(ItemType));
            LocalBuilder serial = generator.DeclareLocal(typeof(ushort));

            Label newDefault = generator.DefineLabel();

            // index before this.Info.ItemId.TryGetTemplate<KeycardItem>(out provider);
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldflda) - 1;

            // get label of start of TryGetTemplate from earlier brtrue
            object runDefault = newInstructions[index - 2].operand;
            newInstructions[index - 2].operand = newDefault;

            // get label of after TryGetTemplate from later brtrue
            object skipDefault = newInstructions[index + 5].operand;

            newInstructions.InsertRange(index, new[]
            {
                // type = this.Info.ItemId;
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(newDefault),
                new(OpCodes.Ldflda, Field(typeof(KeycardPickup), nameof(KeycardPickup.Info))),
                new(OpCodes.Ldfld, Field(typeof(PickupSyncInfo), nameof(PickupSyncInfo.ItemId))),
                new(OpCodes.Stloc, type),

                // type.IsCustomKeycard();
                new(OpCodes.Ldloc, type),
                new(OpCodes.Call, Method(typeof(ItemExtensions), nameof(ItemExtensions.IsCustomKeycard))),

                // jump to default execution if keycard is not custom
                new(OpCodes.Brfalse, runDefault),

                // load dictionary
                new(OpCodes.Ldsfld, Field(typeof(CustomPermsDetail), nameof(CustomPermsDetail.CustomPermissions))),

                // serial = this.Info.Serial;
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldflda, Field(typeof(KeycardPickup), nameof(KeycardPickup.Info))),
                new(OpCodes.Ldfld, Field(typeof(PickupSyncInfo), nameof(PickupSyncInfo.Serial))),
                new(OpCodes.Stloc, serial),

                // Dictionary.TryGetValue(serial, out DoorPermissionFlags permissions);
                new(OpCodes.Ldloc, serial),
                new(OpCodes.Ldloca, permissions),
                new(OpCodes.Callvirt, Method(typeof(Dictionary<ushort, DoorPermissionFlags>), nameof(Dictionary<ushort, DoorPermissionFlags>.TryGetValue))),

                // go to normal execution if no custom perms found
                new(OpCodes.Brfalse, runDefault),

                // provider = new PermissionsProvider(permissions, type, serial);
                new(OpCodes.Ldloc, permissions),
                new(OpCodes.Ldloc, type),
                new(OpCodes.Ldloc, serial),
                new(OpCodes.Newobj, Constructor(typeof(PermissionsProvider), new[] { typeof(DoorPermissionFlags), typeof(ItemType), typeof(ushort) })),
                new(OpCodes.Stloc_2),

                // skip past TryGetTemplate
                new(OpCodes.Br, skipDefault),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}