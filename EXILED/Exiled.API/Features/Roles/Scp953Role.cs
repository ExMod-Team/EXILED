// -----------------------------------------------------------------------
// <copyright file="Scp953Role.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using System.Collections.Generic;

    using Exiled.API.Enums;

    using PlayerRoles;
    using PlayerRoles.PlayableScps;
    using PlayerRoles.PlayableScps.HumeShield;
    using PlayerRoles.PlayableScps.Scp953;
    using PlayerRoles.Subroutines;

    using RelativePositioning;

    using UnityEngine;

    using Scp953GameRole = PlayerRoles.PlayableScps.Scp953.Scp953Role;

    /// <summary>
    /// Defines a role that represents SCP-939.
    /// </summary>
    public class Scp953Role : FpcRole, ISubroutinedScpRole, IHumeShieldRole, ISpawnableScp
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp953Role"/> class.
        /// </summary>
        /// <param name="baseRole">the base <see cref="Scp953GameRole"/>.</param>
        internal Scp953Role(Scp953GameRole baseRole)
            : base(baseRole)
        {
            Base = baseRole;
            SubroutineModule = baseRole.SubroutineModule;
            HumeShieldModule = baseRole.HumeShieldModule;

            if (!SubroutineModule.TryGetSubroutine(out Scp953SlashAbility scp953SlashAbility))
                Log.Error("Scp953SlashAbility not found in Scp953Role::ctor");

            SlashAbility = scp953SlashAbility;
        }

        /// <inheritdoc/>
        public override RoleTypeId Type { get; } = RoleTypeId.Scp953;

        /// <inheritdoc/>
        public SubroutineManagerModule SubroutineModule { get; }

        /// <inheritdoc/>
        public HumeShieldModuleBase HumeShieldModule { get; }

        /// <summary>
        /// Gets SCP-939's <see cref="Scp953SlashAbility"/>.
        /// </summary>
        public Scp953SlashAbility SlashAbility { get; }

        /// <summary>
        /// Gets a value indicating whether .
        /// </summary>
        public bool RespectPreferences => true;

        /// <summary>
        /// Gets a value indicating whether .
        /// </summary>
        public bool AllowFallback => true;

        /// <summary>
        /// Gets or sets the amount of time before SCP-939 can attack again.
        /// </summary>
        public float AttackCooldown
        {
            get => SlashAbility.Cooldown.Remaining;
            set
            {
                SlashAbility.Cooldown.Remaining = value;
                SlashAbility.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Gets the <see cref="Scp953GameRole"/> instance.
        /// </summary>
        public new Scp953GameRole Base { get; }

        /// <summary>
        /// Gets the Spawn Chance of SCP-939.
        /// </summary>
        /// <param name="alreadySpawned">The List of Roles already spawned.</param>
        /// <returns>The Spawn Chance.</returns>
        public float GetSpawnChance(List<RoleTypeId> alreadySpawned) => Base.GetSpawnChance(alreadySpawned);
    }
}