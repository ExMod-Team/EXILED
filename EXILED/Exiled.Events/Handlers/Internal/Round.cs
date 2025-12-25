// -----------------------------------------------------------------------
// <copyright file="Round.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.Internal
{
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
    using PlayerRoles;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.RoleAssign;
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
            }
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
