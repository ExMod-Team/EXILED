// -----------------------------------------------------------------------
// <copyright file="ModifyingFactionInfluenceEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using System.Diagnostics;

    using API.Features;

    using Interfaces;

    using PlayerRoles;

    /// <summary>
    /// Contains all information after activating a generator.
    /// </summary>
    public class ModifyingFactionInfluenceEventArgs : IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyingFactionInfluenceEventArgs" /> class.
        /// </summary>
        /// <param name="team">
        /// <inheritdoc cref="Team" />
        /// </param>
        /// <param name="influence">
        /// <inheritdoc cref="Influence" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public ModifyingFactionInfluenceEventArgs(Team team, float influence, bool isAllowed = true)
        {
            Team = team;
            Influence = influence;
            IsAllowed = isAllowed;
            _ = new StackFrame(4)?.GetMethod()?.Name;
            /*
            OnServerRoleSet (Escaping)
            OnPlayerDamaged
            OnKill
            OnGeneratorEngaged
            OnItemAdded
            */
        }

        /// <summary>
        /// Gets a value indicating where the Modification has been taken from.
        /// </summary>
        public string StackFrame { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether team will get this influence.
        /// </summary>
        public Team Team { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how much Influence will be changed.
        /// </summary>
        public float Influence { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the generator can be activated.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}