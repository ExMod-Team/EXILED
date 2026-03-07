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
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Map;
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
