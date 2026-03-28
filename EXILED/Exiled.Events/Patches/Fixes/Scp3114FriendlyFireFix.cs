// -----------------------------------------------------------------------
// <copyright file="Scp3114FriendlyFireFix.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
#pragma warning disable SA1402 // File may only contain a single type
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;

    using Exiled.API.Features;

    using Footprinting;
    using HarmonyLib;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.ThrowableProjectiles;
    using PlayerRoles;
    using PlayerStatsSystem;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches the <see cref="Scp2176Projectile.ServerShatter()"/> delegate.
    /// Fix Throwing a ghostlight with Scp in the room stun 079.
    /// Bug reported to NW (https://git.scpslgame.com/northwood-qa/scpsl-bug-reporting/-/issues/55).
    /// </summary>
    [HarmonyPatch(typeof(Scp2176Projectile), nameof(Scp2176Projectile.ServerShatter))]
    internal class Scp3114FriendlyFireFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label cnt = generator.DefineLabel();

            int offset = 4;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldsfld) + offset;

            Label skip = (Label)newInstructions[index].operand;

            offset = -4;
            index += offset;

            newInstructions.InsertRange(index, new[]
            {
                // if (this.PreviousOwner.Role.GetTeam() is Team.SCPs)
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Ldfld, Field(typeof(Scp2176Projectile), nameof(Scp2176Projectile.PreviousOwner))),
                new(OpCodes.Ldfld, Field(typeof(Footprint), nameof(Footprint.Role))),
                new(OpCodes.Call, Method(typeof(PlayerRolesUtils), nameof(PlayerRolesUtils.GetTeam), new[] { typeof(RoleTypeId) })),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Ceq),

                new(OpCodes.Brfalse_S, cnt),
                new(OpCodes.Br_S, skip),

                new CodeInstruction(OpCodes.Nop).WithLabels(cnt),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches the <see cref="CollisionDetectionPickup.ProcessCollision(UnityEngine.Collision)"/> delegate.
    /// Fix Throwing a ghostlight with Scp in the room stun 079.
    /// Bug reported to NW (https://git.scpslgame.com/northwood-qa/scpsl-bug-reporting/-/issues/55).
    /// </summary>
    [HarmonyPatch(typeof(CollisionDetectionPickup), nameof(CollisionDetectionPickup.ProcessCollision))]
    internal class Scp3114FriendlyFireFix2 : AttackerDamageHandler
    {
#pragma warning disable SA1600 // Elements should be documented
        public Scp3114FriendlyFireFix2(Footprint attacker, float damage)
        {
            Attacker = attacker;
            Damage = damage;
            AllowSelfDamage = false;
            ServerLogsText = "Scp3114 Fix";
        }

        public override Footprint Attacker { get; set; }

        public override bool AllowSelfDamage { get; }

        public override float Damage { get; set; }

        public override string RagdollInspectText { get; }

        public override string DeathScreenText { get; }

        public override string ServerLogsText { get; }
#pragma warning restore SA1600 // Elements should be documented

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = 0;
            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldnull) + offset;

            // replace null with new Scp3114FriendlyFireFix2(this.PreviousOwner, num2)
            newInstructions.RemoveAt(index);
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // new Scp3114FriendlyFireFix2(this.PreviousOwner, num2)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(CollisionDetectionPickup), nameof(CollisionDetectionPickup.PreviousOwner))),
                new(OpCodes.Ldloc_3),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(Scp3114FriendlyFireFix2))[0]),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}