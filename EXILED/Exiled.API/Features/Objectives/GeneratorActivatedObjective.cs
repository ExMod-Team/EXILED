// -----------------------------------------------------------------------
// <copyright file="GeneratorActivatedObjective.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Objectives
{
    using Exiled.API.Enums;
    using Exiled.API.Interfaces;
    using Respawning.Objectives;

    using BaseObjective = Respawning.Objectives.GeneratorActivatedObjective;

    /// <summary>
    /// Represents an objective that is completed when a generator is activated.
    /// </summary>
    public class GeneratorActivatedObjective : HumanObjective<GeneratorObjectiveFootprint>, IWrapper<BaseObjective>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratorActivatedObjective"/> class.
        /// </summary>
        /// <param name="objectiveFootprintBase">A <see cref="BaseObjective"/> instance.</param>
        internal GeneratorActivatedObjective(BaseObjective objectiveFootprintBase)
            : base(objectiveFootprintBase)
        {
            Base = objectiveFootprintBase;
        }

        /// <inheritdoc/>
        public new BaseObjective Base { get; }

        /// <inheritdoc/>
        public override ObjectiveType Type { get; } = ObjectiveType.GeneratorActivation;

        /// <summary>
        /// Fakes generator activation and tries to achieve this objective.
        /// </summary>
        /// <param name="generator">Generator that is activated.</param>
        /// <param name="player">Player that activated the generator.</param>
        public void Activate(Generator generator, Player player = null) => Base.OnGeneratorEngaged(generator.Base, (player ?? Server.Host).Footprint);
    }
}