// -----------------------------------------------------------------------
// <copyright file="MapGenerated.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.Internal
{
    using System.Collections.Generic;
    using System.Linq;

    using API.Features;
    using Exiled.API.Features.Lockers;

    using MEC;

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
            Locker.ClearCache();

            // TODO: Fix For (https://git.scpslgame.com/northwood-qa/scpsl-bug-reporting/-/issues/377)
            PlayerRoles.RoleAssign.HumanSpawner.Handlers[PlayerRoles.Team.ChaosInsurgency] = new PlayerRoles.RoleAssign.OneRoleHumanSpawner(PlayerRoles.RoleTypeId.ChaosConscript);
            PlayerRoles.RoleAssign.HumanSpawner.Handlers[PlayerRoles.Team.OtherAlive] = new PlayerRoles.RoleAssign.OneRoleHumanSpawner(PlayerRoles.RoleTypeId.Tutorial);
            PlayerRoles.RoleAssign.HumanSpawner.Handlers[PlayerRoles.Team.Dead] = new PlayerRoles.RoleAssign.OneRoleHumanSpawner(PlayerRoles.RoleTypeId.Spectator);
            PlayerRoles.RoleAssign.HumanSpawner.Handlers[PlayerRoles.Team.Flamingos] = new PlayerRoles.RoleAssign.OneRoleHumanSpawner(PlayerRoles.RoleTypeId.Flamingo);

            Timing.CallDelayed(1, Handlers.Map.OnGenerated);
        }
    }
}