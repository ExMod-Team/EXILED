// -----------------------------------------------------------------------
// <copyright file="HumanObjective.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Objectives
{
    using Exiled.API.Interfaces;
    using Respawning.Objectives;

    /// <summary>
    /// Represents a human objective.
    /// </summary>
    /// <typeparam name="T">An objective footprint type.</typeparam>
    public class HumanObjective<T> : Objective, IWrapper<HumanObjectiveBase<T>>
        where T : ObjectiveFootprintBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HumanObjective{T}"/> class.
        /// </summary>
        /// <param name="objectiveFootprintBase">A <see cref="HumanObjectiveBase{T}"/> instance.</param>
        internal HumanObjective(HumanObjectiveBase<T> objectiveFootprintBase)
            : base(objectiveFootprintBase)
        {
            Base = objectiveFootprintBase;
        }

        /// <inheritdoc/>
        public new HumanObjectiveBase<T> Base { get; }

        /// <summary>
        /// Gets or sets the objective footprint.
        /// </summary>
        public T ObjectiveFootprint
        {
            get => (T)Base.ObjectiveFootprint;
            set => Base.ObjectiveFootprint = value;
        }

        /// <summary>
        /// Achieves the objective.
        /// </summary>
        /// <param name="objectiveFootprint">An objective footprint instance.</param>
        public void Achieve(T objectiveFootprint)
        {
            ObjectiveFootprint = objectiveFootprint;
            Achieve();
        }
    }
}