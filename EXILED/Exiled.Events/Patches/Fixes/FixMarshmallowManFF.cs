// -----------------------------------------------------------------------
// <copyright file="FixMarshmallowManFF.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using CustomPlayerEffects;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Footprinting;
    using HarmonyLib;
    using InventorySystem.Items.MarshmallowMan;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.ThrowableProjectiles;
    using Mirror;
    using PlayerRoles;
    using PlayerStatsSystem;
    using Respawning.NamingRules;
    using Subtitles;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches the <see cref="MarshmallowItem.ServerAttack"/> delegate.
    /// </summary>
    [HarmonyPatch(typeof(MarshmallowItem), nameof(MarshmallowItem.ServerAttack))]
    internal class FixMarshmallowManFF : AttackerDamageHandler
    {
#pragma warning disable SA1600 // Elements should be documented
        public FixMarshmallowManFF(MarshmallowItem marshmallowItem)
        {
            Attacker = new Footprint(marshmallowItem.Owner);
            Damage = marshmallowItem._attackDamage;
            AllowSelfDamage = false;
            ServerLogsText = "MarshmallowManFF Fix";
        }

        public override Footprint Attacker { get; set; }

        public override bool AllowSelfDamage { get; }

        public override float Damage { get; set; }

        public override string RagdollInspectText { get; } = DeathTranslations.MarshmallowMan.RagdollTranslation;

        public override CassieAnnouncement CassieDeathAnnouncement { get; } = new()
        {
            Announcement = "TERMINATED BY MARSHMALLOW MAN",
            SubtitleParts =
            [
                new SubtitlePart(SubtitleType.TerminatedByMarshmallowMan, null),
            ],
        };

        public override string DeathScreenText { get; } = DeathTranslations.MarshmallowMan.DeathscreenTranslation;

        public override string ServerLogsText { get; }
#pragma warning restore SA1600 // Elements should be documented
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindIndex(instruction => instruction.Calls(PropertyGetter(typeof(MarshmallowItem), nameof(MarshmallowItem.NewDamageHandler))));

            // replace the getter for NewDamageHandler with ctor of FixMarshmallowManFF
            newInstructions[index] = new CodeInstruction(OpCodes.Newobj, Constructor(typeof(FixMarshmallowManFF), new[] { typeof(MarshmallowItem) }));

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
