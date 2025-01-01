// -----------------------------------------------------------------------
// <copyright file="UnitRole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomUnits.API.Features
{
    using Exiled.CustomRoles.API.Features;
    using PlayerRoles;

    /// <summary>
    /// A wrapper for roles in <see cref="CustomUnit"/>.
    /// </summary>
    public class UnitRole
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitRole"/> class.
        /// </summary>
        public UnitRole()
        {
            RoleTypeId = RoleTypeId.None;
            CustomRole = null;
            MaximumAmount = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitRole"/> class.
        /// </summary>
        /// <param name="roleTypeId"><inheritdoc cref="RoleTypeId"/></param>
        /// <param name="maximumAmount"><inheritdoc cref="MaximumAmount"/></param>
        public UnitRole(RoleTypeId roleTypeId, int maximumAmount = 1)
        {
            RoleTypeId = roleTypeId;
            CustomRole = null;
            MaximumAmount = maximumAmount;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitRole"/> class.
        /// </summary>
        /// <param name="customRole"><inheritdoc cref="CustomRole"/></param>
        /// <param name="maximumAmount"><inheritdoc cref="MaximumAmount"/></param>
        public UnitRole(CustomRole customRole,  int maximumAmount = 1)
        {
            RoleTypeId = RoleTypeId.None;
            CustomRole = customRole;
            MaximumAmount = maximumAmount;
        }

        /// <summary>
        /// Gets or sets a game role.
        /// </summary>
        public RoleTypeId RoleTypeId { get; set; }

        /// <summary>
        /// Gets or sets a custom role.
        /// </summary>
        public CustomRole? CustomRole { get; set; }

        /// <summary>
        /// Gets or sets a maximum amount of players that can get this role.
        /// </summary>
        public int MaximumAmount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this role must spawn in wave.
        /// </summary>
        public bool MustSpawn { get; set; } = false;
    }
}