// -----------------------------------------------------------------------
// <copyright file="Round.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CentralAuth;
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Core.UserSettings;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pools;
    using Exiled.API.Features.Roles;
    using Exiled.API.Structs;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Scp049;
    using Exiled.Loader;
    using Exiled.Loader.Features;
    using InventorySystem;
    using InventorySystem.Items.Firearms.Attachments;
    using InventorySystem.Items.Firearms.Attachments.Components;
    using InventorySystem.Items.Usables;
    using InventorySystem.Items.Usables.Scp244.Hypothermia;
    using InventorySystem.Items.Usables.Scp330;
    using Mirror;
    using PlayerRoles;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.RoleAssign;
    using RelativePositioning;
    using Respawning.NamingRules;
    using UnityEngine;
    using Utils.Networking;
    using Utils.NonAllocLINQ;

    /// <summary>
    /// Handles some round clean-up events and some others related to players.
    /// </summary>
    internal static class Round
    {
        /// <inheritdoc cref="Handlers.Player.OnUsedItem" />
        public static void OnServerOnUsingCompleted(ReferenceHub hub, UsableItem usable) => Handlers.Player.OnUsedItem(new (hub, usable, false));

        /// <inheritdoc cref="Handlers.Server.OnWaitingForPlayers" />
        public static void OnWaitingForPlayers()
        {
            GenerateAttachments();
            MultiAdminFeatures.CallEvent(MultiAdminFeatures.EventType.WAITING_FOR_PLAYERS);

            if (Events.Instance.Config.ShouldReloadConfigsAtRoundRestart)
                ConfigManager.Reload();

            if (Events.Instance.Config.ShouldReloadTranslationsAtRoundRestart)
                TranslationManager.Reload();

            RoundSummary.RoundLock = false;
        }

        /// <inheritdoc cref="Handlers.Server.OnRestartingRound" />
        public static void OnRestartingRound()
        {
            Scp049Role.TurnedPlayers.Clear();
            Scp173Role.TurnedPlayers.Clear();
            Scp096Role.TurnedPlayers.Clear();
            Scp079Role.TurnedPlayers.Clear();

            MultiAdminFeatures.CallEvent(MultiAdminFeatures.EventType.ROUND_END);

            TeslaGate.IgnoredPlayers.Clear();
            TeslaGate.IgnoredRoles.Clear();
            TeslaGate.IgnoredTeams.Clear();

            API.Features.Round.IgnoredPlayers.Clear();
        }

        /// <inheritdoc cref="Handlers.Server.OnRoundStarted" />
        public static void OnRoundStarted() => MultiAdminFeatures.CallEvent(MultiAdminFeatures.EventType.ROUND_START);

        /// <inheritdoc cref="Handlers.Player.OnChangingRole(ChangingRoleEventArgs)" />
        public static void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (!ev.Player.IsHost && ev.NewRole == RoleTypeId.Spectator && ev.Reason != API.Enums.SpawnReason.Destroyed && Events.Instance.Config.ShouldDropInventory)
                ev.Player.Inventory.ServerDropEverything();
        }

        /// <inheritdoc cref="Handlers.Player.OnSpawned(SpawnedEventArgs)" />
        public static void OnSpawned(SpawnedEventArgs ev)
        {
            foreach (Player viewer in Player.Enumerable)
            {
                if (viewer == ev.Player)
                    continue;

                foreach (Func<Player, RoleData> generator in ev.Player.FakeRoleGenerator)
                {
                    RoleData data = generator(viewer);
                    if (data.Role != RoleTypeId.None)
                    {
                        viewer.FakeRoles[ev.Player] = data;
                    }
                }
            }
        }

        /// <inheritdoc cref="Handlers.Player.OnDying(DyingEventArgs)" />
        public static void OnDying(DyingEventArgs ev)
        {
            if (!ev.IsAllowed)
                return;

            foreach (Player viewer in Player.Enumerable)
            {
                if (viewer == ev.Player)
                    continue;

                if (viewer.FakeRoles.TryGetValue(ev.Player, out RoleData data) && (data.DataAuthority & RoleData.Authority.Persist) == RoleData.Authority.None)
                    viewer.FakeRoles.Remove(ev.Player);
            }
        }

        /// <inheritdoc cref="Handlers.Player.OnSpawningRagdoll(SpawningRagdollEventArgs)" />
        public static void OnSpawningRagdoll(SpawningRagdollEventArgs ev)
        {
            if (ev.Role.IsDead() || !ev.Role.IsFpcRole())
                ev.IsAllowed = false;

            if (ev.DamageHandlerBase is Exiled.Events.Patches.Fixes.FixMarshmallowManFF fixMarshamllowManFf)
                ev.DamageHandlerBase = fixMarshamllowManFf.MarshmallowItem.NewDamageHandler;
        }

        /// <inheritdoc cref="Scp049.OnActivatingSense(ActivatingSenseEventArgs)" />
        public static void OnActivatingSense(ActivatingSenseEventArgs ev)
        {
            if (ev.Target is null)
                return;
            if ((Events.Instance.Config.CanScp049SenseTutorial || ev.Target.Role.Type is not RoleTypeId.Tutorial) && !Scp049Role.TurnedPlayers.Contains(ev.Target))
                return;
            ev.Target = ev.Scp049.SenseAbility.CanFindTarget(out ReferenceHub hub) ? Player.Get(hub) : null;
        }

        /// <inheritdoc cref="Handlers.Player.OnVerified(VerifiedEventArgs)" />
        public static void OnVerified(VerifiedEventArgs ev)
        {
            RoleAssigner.CheckLateJoin(ev.Player.ReferenceHub, ClientInstanceMode.ReadyClient);

            if (SettingBase.SyncOnJoin != null && SettingBase.SyncOnJoin(ev.Player))
                SettingBase.SendToPlayer(ev.Player);

            // TODO: Remove if this has been fixed for https://git.scpslgame.com/northwood-qa/scpsl-bug-reporting/-/issues/52
            foreach (Room room in Room.List.Where(current => current.AreLightsOff))
            {
                ev.Player.SendFakeSyncVar(room.RoomLightControllerNetIdentity, typeof(RoomLightController), nameof(RoomLightController.NetworkLightsEnabled), true);
                ev.Player.SendFakeSyncVar(room.RoomLightControllerNetIdentity, typeof(RoomLightController), nameof(RoomLightController.NetworkLightsEnabled), false);
            }

            // Fix bug that player that Join do not receive information about other players Scale
            foreach (Player player in ReferenceHub.AllHubs.Select(Player.Get))
            {
                player.SetFakeScale(player.Scale, new List<Player>() { ev.Player });

                if (player != ev.Player)
                {
                    foreach (Func<Player, RoleData> generator in player.FakeRoleGenerator)
                    {
                        RoleData data = generator(ev.Player);
                        if (data.Role != RoleTypeId.None)
                        {
                            ev.Player.FakeRoles[player] = data;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Makes fake role API work.
        /// </summary>
        /// <param name="ownerHub">The <see cref="ReferenceHub"/> of the player.</param>
        /// <param name="viewerHub">The <see cref="ReferenceHub"/> of the viewer.</param>
        /// <param name="actualRole">The actual <see cref="RoleTypeId"/>.</param>
        /// <param name="writer">The pooled <see cref="NetworkWriter"/>.</param>
        /// <returns>A role, fake if needed.</returns>
        public static RoleTypeId OnRoleSyncEvent(ReferenceHub ownerHub, ReferenceHub viewerHub, RoleTypeId actualRole, NetworkWriter writer)
        {
            Player owner = Player.Get(ownerHub);
            Player viewer = Player.Get(viewerHub);

            if (viewer.FakeRoles.TryGetValue(owner, out RoleData data))
            {
                if (data.Role == actualRole)
                    return actualRole;

                // if another plugin has written data, we can't reliably modify and expect non-breaking behavior.
                // if we send faulty data we can accidentally soft-dc the entire server which is much worse than a plugin not working.
                if (writer.Position != 0 && (data.DataAuthority & RoleData.Authority.Override) == RoleData.Authority.None)
                    return actualRole;

                writer.Position = 0;

                // I doubt most devs want people who are dead to have fake roles.
                if (actualRole.IsDead() && (data.DataAuthority & RoleData.Authority.Always) == RoleData.Authority.None)
                    return actualRole;

                if (data.CustomData != null)
                {
                    data.CustomData(writer);
                }
                else
                {
                    if (data.Role.GetRoleBase() is PlayerRoles.HumanRole { UsesUnitNames: true })
                    {
                        if (data.UnitId != 0)
                        {
                            writer.WriteByte(data.UnitId);
                        }
                        else
                        {
                            if (!NamingRulesManager.GeneratedNames.TryGetValue(Team.FoundationForces, out List<string> list))
                                return actualRole;

                            writer.WriteByte((byte)list.Count);
                        }
                    }

                    if (data.Role.GetRoleBase() is PlayerRoles.PlayableScps.Scp1507.Scp1507Role flamingo)
                        writer.WriteByte((byte)flamingo.ServerSpawnReason);

                    if (data.Role == RoleTypeId.Scp0492)
                    {
                        writer.WriteUShort((ushort)Mathf.Clamp(Mathf.CeilToInt(owner.MaxHealth), 0, ushort.MaxValue));
                        writer.WriteBool(false);
                    }

                    writer.WriteRelativePosition(new RelativePosition(owner.Position));
                    writer.WriteUShort((ushort)Mathf.RoundToInt(Mathf.InverseLerp(0.0f, 360f, owner.Rotation.eulerAngles.y) * ushort.MaxValue));
                }

                return data.Role;
            }

            return actualRole;
        }

        /// <inheritdoc cref="Handlers.Warhead.OnDetonated()"/>
        public static void OnWarheadDetonated()
        {
            // fix for black candy
            CandyBlack.Outcomes.RemoveAll(outcome => outcome is TeleportOutcome);
        }

        private static void GenerateAttachments()
        {
            foreach (FirearmType firearmType in EnumUtils<FirearmType>.Values)
            {
                if (firearmType == FirearmType.None)
                    continue;

                if (Item.Create(firearmType.GetItemType()) is not Firearm firearm)
                    continue;

                Firearm.ItemTypeToFirearmInstance[firearmType] = firearm;

                List<AttachmentIdentifier> attachmentIdentifiers = ListPool<AttachmentIdentifier>.Pool.Get();
                HashSet<AttachmentSlot> attachmentsSlots = HashSetPool<AttachmentSlot>.Pool.Get();

                uint code = 1;

                foreach (Attachment attachment in firearm.Attachments)
                {
                    attachmentsSlots.Add(attachment.Slot);
                    attachmentIdentifiers.Add(new(code, attachment.Name, attachment.Slot));
                    code *= 2U;
                }

                uint baseCode = 0;
                attachmentsSlots.ForEach(slot => baseCode += attachmentIdentifiers
                        .Where(attachment => attachment.Slot == slot)
                        .Min(slot => slot.Code));

                Firearm.BaseCodesValue[firearmType] = baseCode;
                Firearm.AvailableAttachmentsValue[firearmType] = attachmentIdentifiers.ToArray();

                ListPool<AttachmentIdentifier>.Pool.Return(attachmentIdentifiers);
                HashSetPool<AttachmentSlot>.Pool.Return(attachmentsSlots);
            }
        }
    }
}
