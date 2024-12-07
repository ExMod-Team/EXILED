// -----------------------------------------------------------------------
// <copyright file="DestroyedRole.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using PlayerRoles;
    using PlayerRoles.Voice;

    /// <summary>
    /// Defines a role that represents players with destroyed role.
    /// </summary>
    internal class DestroyedRole : Role
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DestroyedRole"/> class.
        /// </summary>
        /// <param name="baseRole">the base <see cref="DestroyedRole"/>.</param>
        internal DestroyedRole(PlayerRoleBase baseRole)
            : base(baseRole)
        {
        }

        /// <inheritdoc/>
        public override RoleTypeId Type { get; } = RoleTypeId.Destroyed;
    }
}
