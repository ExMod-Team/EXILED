// -----------------------------------------------------------------------
// <copyright file="EscapingEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Exiled.API.Enums;
    using Interfaces;

    using PlayerRoles;

    /// <summary>
    /// Contains all information before a player escapes.
    /// </summary>
    public class EscapingEventArgs : IPlayerEvent, IDeniableEvent
    {
        private EscapeScenario escapeScenario;

        /// <summary>
        /// Initializes a new instance of the <see cref="EscapingEventArgs" /> class.
        /// </summary>
        /// <param name="referenceHub">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="newRole">
        /// <inheritdoc cref="NewRole" />
        /// </param>
        /// <param name="escapeScenario">
        /// <inheritdoc cref="EscapeScenario" />
        /// </param>
        public EscapingEventArgs(ReferenceHub referenceHub, RoleTypeId newRole, EscapeScenario escapeScenario)
        {
            Player = Player.Get(referenceHub);
            NewRole = newRole;
            EscapeScenario = escapeScenario;
            IsAllowed = escapeScenario is not EscapeScenario.None;
        }

        /// <summary>
        /// Gets the player who's escaping.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the role that will be assigned when the player escapes.
        /// </summary>
        public RoleTypeId NewRole { get; set; }

        /// <summary>
        /// Gets or sets the EscapeScenario that will represent for this player.
        /// </summary>
        public EscapeScenario EscapeScenario
        {
            get => (escapeScenario is EscapeScenario.None && IsAllowed) ? EscapeScenario.CustomEscape : escapeScenario;
            set => escapeScenario = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the player can escape.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
