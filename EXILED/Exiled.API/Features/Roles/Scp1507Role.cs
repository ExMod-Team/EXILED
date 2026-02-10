// -----------------------------------------------------------------------
// <copyright file="Scp1507Role.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using System;
    using System.Collections.Generic;

    using PlayerRoles;
    using PlayerRoles.PlayableScps;
    using PlayerRoles.PlayableScps.HumeShield;
    using PlayerRoles.Subroutines;

    using Scp1507GameRole = PlayerRoles.PlayableScps.Scp1507.Scp1507Role;

    /// <summary>
    /// Defines a role that represents SCP-1507.
    /// </summary>
    [Obsolete("Only availaible for Christmas and AprilFools.")]
    public class Scp1507Role : FpcRole, ISubroutinedScpRole, IHumeShieldRole, ISpawnableScp
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp1507Role"/> class.
        /// </summary>
        /// <param name="baseRole">the base <see cref="Scp1507GameRole"/>.</param>
        internal Scp1507Role(Scp1507GameRole baseRole)
            : base(baseRole)
        {
            Base = baseRole;
            SubroutineModule = baseRole.SubroutineModule;
            HumeShieldModule = baseRole.HumeShieldModule;
        }

        /// <inheritdoc/>
        public override RoleTypeId Type => Base._roleTypeId;

        /// <inheritdoc/>
        public SubroutineManagerModule SubroutineModule { get; }

        /// <inheritdoc/>
        public HumeShieldModuleBase HumeShieldModule { get; }

        /// <summary>
        /// Gets the <see cref="Scp1507GameRole"/> instance.
        /// </summary>
        public new Scp1507GameRole Base { get; }

        /// <summary>
        /// Gets a value indicating whether .
        /// </summary>
        public bool RespectPreferences => true;

        /// <summary>
        /// Gets a value indicating whether .
        /// </summary>
        public bool AllowFallback => true;

        /// <summary>
        /// Gets the Spawn Chance of SCP-939.
        /// </summary>
        /// <param name="alreadySpawned">The List of Roles already spawned.</param>
        /// <returns>The Spawn Chance.</returns>
        public float GetSpawnChance(List<RoleTypeId> alreadySpawned) => Base is ISpawnableScp spawnableScp ? spawnableScp.GetSpawnChance(alreadySpawned) : 0;
    }
}
