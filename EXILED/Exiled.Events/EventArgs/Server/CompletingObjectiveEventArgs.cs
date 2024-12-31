// -----------------------------------------------------------------------
// <copyright file="CompletingObjectiveEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Server
{
    using Exiled.API.Features.Objectives;
    using Exiled.Events.EventArgs.Interfaces;
    using Respawning.Objectives;

    /// <summary>
    /// Contains all information before the completion of an objective.
    /// </summary>
    public class CompletingObjectiveEventArgs : IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompletingObjectiveEventArgs"/> class.
        /// </summary>
        /// <param name="objective"><inheritdoc cref="Objective"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public CompletingObjectiveEventArgs(FactionObjectiveBase objective, bool isAllowed = true)
        {
            Objective = Objective.Get(objective);
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the objective that is being completed.
        /// </summary>
        public Objective Objective { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }
    }
}