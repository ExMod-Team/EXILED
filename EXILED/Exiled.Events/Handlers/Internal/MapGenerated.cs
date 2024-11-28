// -----------------------------------------------------------------------
// <copyright file="MapGenerated.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using API.Features;
    using API.Features.Items;
    using API.Features.Pools;
    using API.Structs;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Lockers;
    using InventorySystem.Items.Firearms.Attachments;
    using InventorySystem.Items.Firearms.Attachments.Components;

    using MEC;

    using Utils.NonAllocLINQ;

    /// <summary>
    /// Handles <see cref="Handlers.Map.Generated"/> event.
    /// </summary>
    internal static class MapGenerated
    {
        /// <summary>
        /// Called once the map is generated.
        /// </summary>
        /// <remarks>
        /// This fixes an issue where
        /// all those extensions that
        /// require calling the central
        /// property of the Map class in
        /// the API were corrupted due to
        /// a missed call, such as before
        /// getting the elevator type.
        /// </remarks>
        public static void OnMapGenerated()
        {
            Map.ClearCache();
            PrefabHelper.LoadPrefabs();
            Locker.ClearCache();

            // TODO: Fix For (https://git.scpslgame.com/northwood-qa/scpsl-bug-reporting/-/issues/377)
            PlayerRoles.RoleAssign.HumanSpawner.Handlers[PlayerRoles.Team.ChaosInsurgency] = new PlayerRoles.RoleAssign.OneRoleHumanSpawner(PlayerRoles.RoleTypeId.ChaosConscript);
            PlayerRoles.RoleAssign.HumanSpawner.Handlers[PlayerRoles.Team.OtherAlive] = new PlayerRoles.RoleAssign.OneRoleHumanSpawner(PlayerRoles.RoleTypeId.Tutorial);
            PlayerRoles.RoleAssign.HumanSpawner.Handlers[PlayerRoles.Team.Dead] = new PlayerRoles.RoleAssign.OneRoleHumanSpawner(PlayerRoles.RoleTypeId.Spectator);

            Timing.CallDelayed(1, Handlers.Map.OnGenerated);
        }
    }
}