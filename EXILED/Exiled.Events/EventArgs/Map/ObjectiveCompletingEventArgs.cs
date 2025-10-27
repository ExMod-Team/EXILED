// -----------------------------------------------------------------------
// <copyright file="ObjectiveCompletingEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using Exiled.API.Features.Objectives;
    using Interfaces;
    using PlayerRoles;
    using Respawning.Objectives;

    /// <summary>
    /// Contains all information after activating a generator.
    /// </summary>
    public class ObjectiveCompletingEventArgs : IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectiveCompletingEventArgs" /> class.
        /// </summary>
        /// <param name="factionObjectiveBase">
        /// <inheritdoc cref="Objective" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public ObjectiveCompletingEventArgs(FactionObjectiveBase factionObjectiveBase, bool isAllowed = true)
        {
            Objective = Objective.Get(factionObjectiveBase);
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets a value indicating where the Modification has been taken from.
        /// </summary>
        public Objective Objective { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the generator can be activated.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}