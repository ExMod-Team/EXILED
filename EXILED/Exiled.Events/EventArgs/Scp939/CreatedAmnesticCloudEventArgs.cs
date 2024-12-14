// -----------------------------------------------------------------------
// <copyright file="CreatedAmnesticCloudEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp939
{
    using API.Features;

    using Exiled.API.Features.Hazards;
    using Exiled.API.Features.Roles;
    using Interfaces;

    using PlayerRoles.PlayableScps.Scp939;

    using Scp939Role = API.Features.Roles.Scp939Role;

    /// <summary>
    /// Contains all information after SCP-939 fully created target <see cref="PlacedAmnesticCloudEventArgs"/>.
    /// </summary>
    public class CreatedAmnesticCloudEventArgs : IScp939Event, IHazardEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatedAmnesticCloudEventArgs" /> class.
        /// </summary>
        /// <param name="hub">
        /// <inheritdoc cref="ReferenceHub" />
        /// </param>
        /// <param name="cloud">
        /// <inheritdoc cref="PlayerRoles.PlayableScps.Scp939.Scp939AmnesticCloudInstance" />
        /// </param>
        public CreatedAmnesticCloudEventArgs(ReferenceHub hub, Scp939AmnesticCloudInstance cloud)
        {
            Player = Player.Get(hub);
            AmnesticCloud = Hazard.Get<AmnesticCloudHazard>(cloud);
            Scp939 = Player.Role.As<Scp939Role>();
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="AmnesticCloudHazard"/> instance.
        /// </summary>
        public AmnesticCloudHazard AmnesticCloud { get; }

        /// <inheritdoc/>
        public Scp939Role Scp939 { get; }

        /// <inheritdoc/>
        public Hazard Hazard => AmnesticCloud;
    }
}