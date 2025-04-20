// -----------------------------------------------------------------------
// <copyright file="FinishingSenseEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp049
{
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before SCP-049 finishes his sense ability.
    /// </summary>
    public class FinishingSenseEventArgs : IScp049Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FinishingSenseEventArgs"/> class.
        /// </summary>
        /// <param name="scp049">The SCP-049 instance triggering the event.
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="target">The player targeted by SCP-049's Sense ability.
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="cooldowntime">The time in seconds before the Sense ability can be used again.
        /// <inheritdoc cref="double" />
        /// </param>
        /// <param name="isAllowed">Specifies whether the Sense effect is allowed to finish.
        /// <inheritdoc cref="bool" />
        /// </param>
        public FinishingSenseEventArgs(ReferenceHub scp049, ReferenceHub target, double cooldowntime, bool isAllowed = true)
        {
            Player = Player.Get(scp049);
            Scp049 = Player.Role.As<Scp049Role>();
            Target = Player.Get(target);
            IsAllowed = isAllowed;
            CooldownTime = cooldowntime;
        }

        /// <inheritdoc/>
        public Scp049Role Scp049 { get; }

        /// <summary>
        /// Gets the player who is controlling SCP-049.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the player who is SCP-049's active target. Can be null.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets or sets the cooldown duration of the Sense ability.
        /// </summary>
        public double CooldownTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the server will finishing or not finishing 049 Sense Ability.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
