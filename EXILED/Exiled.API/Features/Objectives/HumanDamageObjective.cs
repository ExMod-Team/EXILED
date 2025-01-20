// -----------------------------------------------------------------------
// <copyright file="HumanDamageObjective.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Objectives
{
    using Exiled.API.Enums;
    using Exiled.API.Features.DamageHandlers;
    using Exiled.API.Interfaces;
    using Respawning.Objectives;

    using BaseObjective = Respawning.Objectives.HumanDamageObjective;

    /// <summary>
    /// A wrapper for the human damage objective.
    /// </summary>
    public class HumanDamageObjective : HumanObjective<DamageObjectiveFootprint>, IWrapper<BaseObjective>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HumanDamageObjective"/> class.
        /// </summary>
        /// <param name="objectiveFootprintBase"><inheritdoc cref="Base"/></param>
        internal HumanDamageObjective(BaseObjective objectiveFootprintBase)
            : base(objectiveFootprintBase)
        {
            Base = objectiveFootprintBase;
        }

        /// <inheritdoc/>
        public new BaseObjective Base { get; }

        /// <inheritdoc/>
        public override ObjectiveType Type { get; } = ObjectiveType.HumanDamage;

        /// <summary>
        /// Fakes player's damage and tries to achieve this objective.
        /// </summary>
        /// <param name="attacker">Attacker.</param>
        /// <param name="target">Target to damage.</param>
        /// <param name="amount">Amount of damage.</param>
        /// <param name="type">Type of damage.</param>
        public void Damage(Player attacker, Player target, float amount, DamageType type = DamageType.Unknown)
            => Damage(new CustomDamageHandler(target, attacker, amount, type, string.Empty));

        /// <summary>
        /// Fakes player's damage and tries to achieve this objective.
        /// </summary>
        /// <param name="damageHandler">An <see cref="AttackerDamageHandler"/> instance.</param>
        public void Damage(AttackerDamageHandler damageHandler) => Base.OnPlayerDamaged(damageHandler.Attacker.ReferenceHub, damageHandler.Base);
    }
}