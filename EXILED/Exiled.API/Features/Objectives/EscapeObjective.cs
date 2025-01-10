// -----------------------------------------------------------------------
// <copyright file="EscapeObjective.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Objectives
{
    using Exiled.API.Enums;
    using Exiled.API.Interfaces;
    using PlayerRoles;
    using Respawning.Objectives;

    using BaseObjective = Respawning.Objectives.EscapeObjective;

    /// <summary>
    /// Represents an objective that is completed when a player escapes.
    /// </summary>
    public class EscapeObjective : HumanObjective<EscapeObjectiveFootprint>, IWrapper<BaseObjective>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EscapeObjective"/> class.
        /// </summary>
        /// <param name="objectiveFootprintBase">A <see cref="BaseObjective"/> instance.</param>
        internal EscapeObjective(BaseObjective objectiveFootprintBase)
            : base(objectiveFootprintBase)
        {
            Base = objectiveFootprintBase;
        }

        /// <inheritdoc />
        public new BaseObjective Base { get; }

        /// <inheritdoc />
        public override ObjectiveType Type { get; } = ObjectiveType.Escape;

        /// <summary>
        /// Fakes player's escape and tries to achieve this objective.
        /// </summary>
        /// <param name="player">Target that has escaped.</param>
        /// <param name="newRole">Role that target will get after escaping.</param>
        public void Escape(Player player, RoleTypeId newRole) => Base.OnServerRoleSet(player.ReferenceHub, newRole, RoleChangeReason.Escaped);
    }
}