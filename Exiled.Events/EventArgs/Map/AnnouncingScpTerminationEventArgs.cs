// -----------------------------------------------------------------------
// <copyright file="AnnouncingScpTerminationEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using System;

    using API.Features;
    using API.Features.DamageHandlers;
    using API.Features.Roles;
    using Interfaces;

    /// <summary>
    /// Contains all information before C.A.S.S.I.E announces an SCP termination.
    /// </summary>
    public class AnnouncingScpTerminationEventArgs : IAttackerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnnouncingScpTerminationEventArgs" /> class.
        /// </summary>
        /// <param name="scp">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="terminationCause">
        /// <inheritdoc cref="TerminationCause"/>
        /// </param>
        public AnnouncingScpTerminationEventArgs(Player scp, string terminationCause)
        {
            Player = scp;
            Role = scp.Role;
            TerminationCause = terminationCause;
            IsAllowed = true;
        }

        /// <summary>
        /// Gets the killed <see cref="API.Features.Roles.Role" />.
        /// </summary>
        public Role Role { get; }

        /// <summary>
        /// Gets or sets the termination cause.
        /// </summary>
        public string TerminationCause { get; set; }

        /// <summary>
        /// Gets the player the announcement is being played for.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the player who killed the SCP.
        /// </summary>
        [Obsolete("Attacker can no longer be acquired for this event. This will be readded in a different event.")]
        public Player Attacker { get; }

        /// <summary>
        /// Gets or sets the <see cref="CustomDamageHandler" />.
        /// </summary>
        [Obsolete("DamageHandler can no longer be acquired for this event. This will be readded in a different event.")]
        public CustomDamageHandler DamageHandler { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the SCP termination will be announced by C.A.S.S.I.E.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}