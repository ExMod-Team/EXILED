// -----------------------------------------------------------------------
// <copyright file="StranglingEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp3114
{
    using API.Features;
    using Interfaces;
    using PlayerRoles.PlayableScps.Scp3114;

    using Scp3114Role = Exiled.API.Features.Roles.Scp3114Role;

    /// <summary>
    ///     Contains all information before SCP-3114 strangles a player.
    /// </summary>
    public class StranglingEventArgs : IScp3114Event, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="StranglingEventArgs" /> class.
        /// </summary>
        /// <param name="hub">
        ///     The <see cref="Player"/> instance which this is being instantiated from.
        /// </param>
        /// <param name="target">
        ///     The <see cref="API.Features.Player"/> being targeted.
        /// </param>
        public StranglingEventArgs(ReferenceHub hub, ReferenceHub target)
        {
            Player = Player.Get(hub);
            Scp3114 = Player.Role.As<Scp3114Role>();
            Target = Player.Get(target);
        }

        /// <inheritdoc/>
        /// <summary>
        ///     The <see cref="Player"/> who is Scp-3114.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp3114Role Scp3114 { get; }

        /// <summary>
        ///     Gets the <see cref="Player"/> being strangled.
        /// </summary>
        public Player Target { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; } = true;
    }
}
