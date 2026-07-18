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
    using Exiled.Events.Patches.Generic;
    using Exiled.Loader;
    using Exiled.Loader.Features;
    using global::Scp914.Processors;
    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Firearms.Attachments;
    using InventorySystem.Items.Firearms.Attachments.Components;
    using InventorySystem.Items.Usables;
    using InventorySystem.Items.Usables.Scp244.Hypothermia;
    using InventorySystem.Items.Usables.Scp330;
    using Mirror;
    using PlayerRoles;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.PlayableScps.Scp049.Zombies;
    using PlayerRoles.RoleAssign;
    using PlayerRoles.SpawnData;
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
        /// <summary>
        /// Gets or sets a value indicating whether <see cref="OnRoleSyncEvent"/> is going to be invoked from <see cref="PlayerRoleManager.SendNewRoleInfo"/>.
        /// </summary>
        /// <remarks>This is required to check if we can skip writing all the data for a fake role without looking inside the stack trace (very expensive compared to the patch at <see cref="RoleSyncCallerCheck"/>).</remarks>
        internal static bool SendingNewRoleInfo { get; set; }

        /// <inheritdoc cref="Handlers.Player.OnUsedItem" />
        public static void OnServerOnUsingCompleted(ReferenceHub hub, UsableItem usable) => Handlers.Player.OnUsedItem(new (hub, usable, false));

        /// <inheritdoc cref="Handlers.Server.OnWaitingForPlayers" />
        public static void OnWaitingForPlayers()
        {
            GenerateAttachments();
            AddMissingScp914Processors();
            MultiAdminFeatures.CallEvent(MultiAdminFeatures.EventType.WAITING_FOR_PLAYERS);

            if (Events.Instance.Config.ShouldReloadConfigsAtRoundRestart)
                ConfigManager.Reload();

            if (Events.Instance.Config.ShouldReloadTranslationsAtRoundRestart)
                TranslationManager.Reload();

            RoundSummary.RoundLock = false;

            if (Events.Instance.Config.Debug)
                Patches.Events.Map.Generating.Benchmark();
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
            if (!ev.Player.IsHost && ev.NewRole == RoleTypeId.Spectator && ev.Reason is not SpawnReason.Destroyed && Events.Instance.Config.ShouldDropInventory)
                ev.Player.Inventory.ServerDropEverything();
        }

        /// <inheritdoc cref="Handlers.Player.OnSpawned(SpawnedEventArgs)" />
        public static void OnSpawned(SpawnedEventArgs ev)
        {
            foreach (Player viewer in Player.Enumerable.Where(p => !p.IsNPC && !p.IsHost))
            {
                foreach (Func<Player, RoleData> generator in ev.Player.FakeRoleGenerator)
                {
                    RoleData data = generator(viewer);

                    if (data.Role == RoleTypeId.None)
                        continue;

                    if (viewer != ev.Player)
                    {
                        viewer.FakeRoles[ev.Player] = data;
                    }
                }
            }
        }

        /// <inheritdoc cref="Handlers.Player.OnDied(DiedEventArgs)" />
        public static void OnDied(DiedEventArgs ev)
        {
            foreach (Player viewer in Player.Enumerable.Where(p => !p.IsNPC && !p.IsHost))
            {
                if (viewer.FakeRoles.TryGetValue(ev.Player, out RoleData data) && !data.DataAuthority.HasFlag(RoleData.Authority.Persist))
                    viewer.FakeRoles.Remove(ev.Player);
            }
        }

        /// <inheritdoc cref="Handlers.Player.OnSpawningRagdoll(SpawningRagdollEventArgs)" />
        public static void OnSpawningRagdoll(SpawningRagdollEventArgs ev)
        {
            if (ev.Role.IsDead() || !ev.Role.IsFpcRole())
                ev.IsAllowed = false;
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

                foreach (Func<Player, RoleData> generator in player.FakeRoleGenerator)
                {
                    RoleData data = generator(ev.Player);

                    if (data.Role == RoleTypeId.None)
                        continue;

                    if (player != ev.Player)
                    {
                        ev.Player.FakeRoles[player] = data;
                    }
                }
            }
        }

        /// <summary>
        /// Makes fake role API work.
        /// </summary>
        /// <param name="targetHub">The <see cref="ReferenceHub"/> of the target.</param>
        /// <param name="viewerHub">The <see cref="ReferenceHub"/> of the viewer.</param>
        /// <param name="actualRole">The actual <see cref="RoleTypeId"/>.</param>
        /// <param name="writer">The pooled <see cref="NetworkWriter"/>.</param>
        /// <returns>A role, fake if needed.</returns>
        public static RoleTypeId OnRoleSyncEvent(ReferenceHub targetHub, ReferenceHub viewerHub, RoleTypeId actualRole, NetworkWriter writer)
        {
            Player target = Player.Get(targetHub);
            Player viewer = Player.Get(viewerHub);

            if (viewer.IsHost || !viewer.FakeRoles.TryGetValue(target, out RoleData data))
                return actualRole;

            if (target == viewer && !data.DataAuthority.HasFlag(RoleData.Authority.AffectSelf))
                return actualRole;

            // if another plugin has written data, we can't reliably modify and expect non-breaking behavior.
            // if we send faulty data we can accidentally soft-dc the entire server which is much worse than a plugin not working.
            if (writer.Position != 0 && !data.DataAuthority.HasFlag(RoleData.Authority.Override))
                return actualRole;

            if (!data.DataAuthority.HasFlag(RoleData.Authority.Always) && actualRole.IsDead())
                return actualRole;

            if (!data.DataAuthority.HasFlag(RoleData.Authority.AffectNPCs) && target.IsNPC)
                return actualRole;

            // this check has to be last because otherwise you can get instances where a fake role shouldn't persist due to not having a required Authority,
            // yet it would still persist because this would return the fake role if it was not here.
            if (!SendingNewRoleInfo && targetHub.roleManager.PreviouslySentRole.TryGetValue(viewerHub.netId, out RoleTypeId previousRole) && previousRole == data.Role)
                return previousRole;

            writer.Position = 0;

            if (data.CustomData != null)
            {
                data.CustomData(writer);
            }
            else
            {
                PlayerRoleBase roleBase = data.Role.GetRoleBase();

                if (roleBase is not ISpawnDataReader)
                    return data.Role;

                switch (roleBase)
                {
                    case PlayerRoles.HumanRole { UsesUnitNames: true } when data.UnitId != 0:
                        writer.WriteByte(data.UnitId);
                        break;

                    // W stylecop :heart:
#pragma warning disable SA1013
                    case PlayerRoles.HumanRole { UsesUnitNames: true }:
#pragma warning restore SA1013
                    {
                        if (!NamingRulesManager.GeneratedNames.TryGetValue(Team.FoundationForces, out List<string> list))
                            return actualRole;

                        writer.WriteByte((byte)list.Count);
                        break;
                    }

                    case PlayerRoles.PlayableScps.Scp1507.Scp1507Role flamingo:
                        writer.WriteByte((byte)flamingo.ServerSpawnReason);
                        break;

                    case ZombieRole:
                        writer.WriteUShort((ushort)Mathf.Clamp(Mathf.CeilToInt(target.MaxHealth), 0, ushort.MaxValue));
                        writer.WriteBool(false);
                        break;
                }

                if (target.Role is FpcRole role)
                {
                    writer.WriteRelativePosition(role.ClientRelativePosition);
                    writer.WriteUShort(role.FirstPersonController.FpcModule.MouseLook._prevSyncH);
                }
                else
                {
                    WaypointBase.GetRelativeRotation(target.Position, Quaternion.Euler(Vector3.up * target.Rotation.eulerAngles.y), out _, out Quaternion relativeRotation);

                    writer.WriteRelativePosition(new RelativePosition(target.Position));
                    writer.WriteUShort((ushort)Mathf.RoundToInt(Mathf.InverseLerp(0F, 360F, relativeRotation.eulerAngles.y) * ushort.MaxValue));
                }
            }

            return data.Role;
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

        private static void AddMissingScp914Processors()
        {
            foreach (KeyValuePair<ItemType, ItemBase> entry in InventoryItemLoader.AvailableItems)
            {
                ItemType itemType = entry.Key;
                ItemBase item = entry.Value;

                if (item is null || item.TryGetComponent<Scp914ItemProcessor>(out _))
                {
                    continue;
                }

                ItemType[] outputs = new[] { itemType };

                StandardItemProcessor processor = item.gameObject.AddComponent<StandardItemProcessor>();

                processor._roughOutputs = outputs;
                processor._coarseOutputs = outputs;
                processor._oneToOneOutputs = outputs;
                processor._fineOutputs = outputs;
                processor._veryFineOutputs = outputs;
                processor._fireUpgradeTrigger = false;
            }
        }
    }
}
