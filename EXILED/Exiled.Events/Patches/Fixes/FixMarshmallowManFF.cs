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
            Attacker = new(marshmallowItem.Owner);
            Damage = marshmallowItem._attackDamage;
            AllowSelfDamage = false;
            ServerLogsText = "MarshmallowManFF Fix";
        }

        public override Footprint Attacker { get; set; }

        public override bool AllowSelfDamage { get; }

        public override float Damage { get; set; }

        public override string RagdollInspectText { get; }

        public override CassieAnnouncement CassieDeathAnnouncement
        {
            get
            {
                return new CassieAnnouncement()
                {
                    Announcement = "TERMINATED BY MARSHMALLOW MAN",
                    SubtitleParts = new SubtitlePart[]
                    {
                        new SubtitlePart(SubtitleType.TerminatedByMarshmallowMan, null),
                    },
                };
            }
        }

        public override string DeathScreenText { get; }

        public override string ServerLogsText { get; }
#pragma warning restore SA1600 // Elements should be documented
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

        private static bool Prefix(MarshmallowItem __instance, ReferenceHub syncTarget)
        {
            foreach (IDestructible destructible in __instance.DetectDestructibles())
            {
                HitboxIdentity hitboxIdentity = destructible as HitboxIdentity;
                if ((hitboxIdentity == null || (!(hitboxIdentity.TargetHub != syncTarget) && (__instance.EvilMode || HitboxIdentity.IsDamageable(__instance.Owner, hitboxIdentity.TargetHub)))) && destructible.Damage(__instance._attackDamage, new FixMarshmallowManFF(__instance), destructible.CenterOfMass))
                {
                    HitboxIdentity hitboxIdentity2 = destructible as HitboxIdentity;
                    if (hitboxIdentity2 != null && !hitboxIdentity2.TargetHub.IsAlive())
                    {
                        __instance.Owner.playerEffectsController.GetEffect<SugarCrave>().OnKill();
                    }

                    Hitmarker.SendHitmarkerDirectly(__instance.Owner, 1f, true);
                    __instance.ServerSendPublicRpc(writer =>
                    {
                        writer.WriteByte(1);
                    });
                    break;
                }
            }

            return false;
        }
    }
}
