// -----------------------------------------------------------------------
// <copyright file="SendingRoleEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Interfaces;

    using PlayerRoles;

    /// <summary>
    /// Contains all information before a <see cref="API.Features.Player"/>'s role is sent to a client.
    /// </summary>
    public class SendingRoleEventArgs : IPlayerEvent
    {
        private RoleTypeId roleTypeId;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendingRoleEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="target">
        /// <inheritdoc cref="Target" />
        /// </param>
        /// <param name="roleType">
        /// <inheritdoc cref="RoleType" />
        /// </param>
        public SendingRoleEventArgs(Player player, uint target, RoleTypeId roleType)
        {
            Player = player;
            Target = Player.Get(target);
            roleTypeId = roleType;
        }

        /// <summary>
        /// Gets the <see cref="API.Features.Player"/> on whose behalf the role change request is sent.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// gets the <see cref="API.Features.Player"/> to whom the request is sent.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets or sets the <see cref="RoleTypeId"/> that is sent to the <see cref="Target"/>.
        /// </summary>
        /// <remarks>Checks value by player <see cref="Role.CheckAppearanceCompatibility(RoleTypeId)"/>.</remarks>
        public RoleTypeId RoleType
        {
            get => roleTypeId;

            set
            {
                if (Player.Role.CheckAppearanceCompatibility(value))
                {
                    roleTypeId = value;
                }
            }
        }
    }
}
