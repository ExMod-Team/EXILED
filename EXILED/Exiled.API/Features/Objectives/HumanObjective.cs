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
        where T : ObjectiveFootprintBase, new()
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
        /// <remarks>Can be <c>null</c>. It's being set by game only before achieving.</remarks>
        public T ObjectiveFootprint
        {
            get => (T)Base.ObjectiveFootprint;
            set => Base.ObjectiveFootprint = value;
        }

        /// <summary>
        /// Gets or sets the time reward.
        /// </summary>
        /// <remarks>
        /// Can be <c>0</c> if <see cref="ObjectiveFootprint"/> is <c>null</c>.
        /// Setter affects only client notification.
        /// </remarks>
        public float TimeReward
        {
            get => ObjectiveFootprint?.TimeReward ?? 0;
            set
            {
                ObjectiveFootprint ??= new T();
                ObjectiveFootprint.TimeReward = value;
            }
        }

        /// <summary>
        /// Gets or sets the influence reward.
        /// </summary>
        /// <remarks>
        /// Can be <c>0</c> if <see cref="ObjectiveFootprint"/> is <c>null</c>.
        /// Setter affects only client notification.
        /// </remarks>
        public float InfluenceReward
        {
            get => ObjectiveFootprint?.InfluenceReward ?? 0;
            set
            {
                ObjectiveFootprint ??= new T();
                ObjectiveFootprint.InfluenceReward = value;
            }
        }

        /// <summary>
        /// Gets or sets the achiever.
        /// </summary>
        /// <remarks>
        /// Can be <c>null</c> if <see cref="ObjectiveFootprint"/> is <c>null</c>.
        /// Setter affects only client notification.
        /// </remarks>
        public Player Achiever
        {
            get => ObjectiveFootprint == null ? null : Player.Get(ObjectiveFootprint.AchievingPlayer.Nickname);
            set
            {
                ObjectiveFootprint ??= new T();
                ObjectiveFootprint.AchievingPlayer = new(value.Footprint);
            }
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