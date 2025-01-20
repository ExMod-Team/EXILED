// -----------------------------------------------------------------------
// <copyright file="UpdatedCloudStateEventArgs.cs" company="ExMod Team">
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
    /// Contains all information after <see cref="PlacedAmnesticCloudEventArgs"/> got updated state.
    /// </summary>
    public class UpdatedCloudStateEventArgs : IScp939Event, IHazardEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdatedCloudStateEventArgs" /> class.
        /// </summary>
        /// <param name="hub">
        /// <inheritdoc cref="ReferenceHub" />
        /// </param>
        /// <param name="cloudState">
        /// <inheritdoc cref="Scp939AmnesticCloudInstance.CloudState" />
        /// </param>
        /// <param name="cloud">
        /// <inheritdoc cref="Scp939AmnesticCloudInstance" />
        /// </param>
        public UpdatedCloudStateEventArgs(ReferenceHub hub, Scp939AmnesticCloudInstance.CloudState cloudState, Scp939AmnesticCloudInstance cloud)
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

        /// <summary>
        /// Gets the <see cref="Scp939AmnesticCloudInstance.CloudState"/>.
        /// </summary>
        public Scp939AmnesticCloudInstance.CloudState NewState { get; }

        /// <inheritdoc/>
        public Scp939Role Scp939 { get; }

        /// <inheritdoc/>
        public Hazard Hazard => AmnesticCloud;
    }
}