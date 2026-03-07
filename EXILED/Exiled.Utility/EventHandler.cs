// -----------------------------------------------------------------------
// <copyright file="EventHandler.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Decals;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.CustomRoles.API.Features;
    using Exiled.Events.EventArgs.Map;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Utility.Enums;
    using InventorySystem;
    using InventorySystem.Items.Firearms.Modules;
    using PlayerRoles;

#pragma warning disable CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement
#pragma warning disable SA1600 // Elements should be documented
    public class EventHandler
    {
        public Config Config { get; internal set; }

        public void OnWaitingForPlayers()
        {
            if (!Config.RecontainScp079IfNoScpsLeft)
                PlayerRoleManager.OnServerRoleSet -= Recontainer.Base.OnServerRoleChanged;
        }

        /// <inheritdoc cref="Events.Handlers.Player.OnChangingRole(ChangingRoleEventArgs)" />
        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (!ev.Player.IsHost && ev.NewRole == RoleTypeId.Spectator && ev.Reason is not SpawnReason.Destroyed && Config.ShouldDropInventory)
                ev.Player.Inventory.ServerDropEverything();
        }

        public void OnSpawned(SpawnedEventArgs ev)
        {
            Scp173Role.TurnedPlayers.Remove(ev.Player);
            Scp096Role.TurnedPlayers.Remove(ev.Player);
            Scp049Role.TurnedPlayers.Remove(ev.Player);
            Scp0492Role.TurnedPlayers.Remove(ev.Player);
            Scp079Role.TurnedPlayers.Remove(ev.Player);
            string role = ev.Player.Role.Type.ToString();
            foreach (CustomRole customRole in CustomRole.Registered)
            {
                if (customRole.Check(ev.Player))
                {
                    role = customRole.Name;
                    break;
                }
            }

            if (Config.NewStuffThatWasAboutTutorial.TryGetValue(role, out NewEnumForAllStuffThatWasAboutTutorial bruh))
            {
                if (bruh.HasFlag(NewEnumForAllStuffThatWasAboutTutorial.CanBlockScp173))
                {
                    Scp173Role.TurnedPlayers.Add(ev.Player);
                }

                if (bruh.HasFlag(NewEnumForAllStuffThatWasAboutTutorial.CanTriggerScp096))
                {
                    Scp096Role.TurnedPlayers.Add(ev.Player);
                }

                if (bruh.HasFlag(NewEnumForAllStuffThatWasAboutTutorial.CanScp049Sense))
                {
                    Scp049Role.TurnedPlayers.Add(ev.Player);
                }

                if (bruh.HasFlag(NewEnumForAllStuffThatWasAboutTutorial.CanScp0492Sense))
                {
                    Scp0492Role.TurnedPlayers.Add(ev.Player);
                }

                if (bruh.HasFlag(NewEnumForAllStuffThatWasAboutTutorial.NotAffectedByScp079Scan))
                {
                    Scp079Role.TurnedPlayers.Add(ev.Player);
                }
            }
        }

        public void OnPlacingBulletHole(PlacingBulletHoleEventArgs ev)
        {
            ImpactEffectsModule impactEffectsModule = ev.Firearm.HitscanHitregModule._impactEffectsModule;
            int? num = null;
            for (int i = 0; i < impactEffectsModule.AttachmentOverrides.Length; i++)
            {
                if (impactEffectsModule.AttachmentOverrides[i].GetEnabled(ev.Firearm.Base))
                {
                    num = new int?(i);
                    break;
                }
            }

            DecalPoolType decalPoolType = ev.Firearm.HitscanHitregModule._impactEffectsModule.GetSettings(num).BulletholeDecal;

            if (Config.PreventDecalSpawn.Contains(decalPoolType))
                ev.IsAllowed = false;
        }
    }
}
