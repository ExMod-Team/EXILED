// -----------------------------------------------------------------------
// <copyright file="NoneRole.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using PlayerRoles;
    using PlayerRoles.Voice;

    using NoneGameRole = PlayerRoles.NoneRole;

    /// <summary>
    /// Defines a role that represents players with no role.
    /// </summary>
    public class NoneRole : Role, IVoiceRole
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoneRole"/> class.
        /// </summary>
        /// <param name="baseRole">the base <see cref="NoneGameRole"/>.</param>
        internal NoneRole(PlayerRoleBase baseRole)
            : base(baseRole)
        {
        }

        /// <inheritdoc/>
        public override RoleTypeId Type { get; } = RoleTypeId.None;

        /// <inheritdoc/>
        public VoiceModuleBase VoiceModule => (Base as NoneGameRole) !.VoiceModule;
    }
}