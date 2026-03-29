// -----------------------------------------------------------------------
// <copyright file="PlacedAmnesticCloudEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp939
{
    using API.Features;
    using API.Features.Hazards;
    using Interfaces;
    using PlayerRoles.PlayableScps.Scp939;

    using Scp939Role = API.Features.Roles.Scp939Role;

    /// <summary>
    /// Contains all information after SCP-939 used its amnestic cloud ability.
    /// </summary>
    public class PlacedAmnesticCloudEventArgs : IScp939Event, IHazardEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlacedAmnesticCloudEventArgs" /> class.
        /// </summary>
        /// <param name="hub">
        /// <inheritdoc cref="ReferenceHub" />
        /// </param>
        /// <param name="cloud">
        /// <inheritdoc cref="Scp939AmnesticCloudInstance" />
        /// </param>
        public PlacedAmnesticCloudEventArgs(ReferenceHub hub, Scp939AmnesticCloudInstance cloud)
        {
            Player = Player.Get(hub);
            AmnesticCloud = Hazard.Get<AmnesticCloudHazard>(cloud);
            Scp939 = Player.Role.As<Scp939Role>();
        }

        /// <summary>
        /// Gets the player who's controlling SCP-939.
        /// </summary>
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