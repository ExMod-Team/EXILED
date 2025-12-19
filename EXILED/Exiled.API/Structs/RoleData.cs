// -----------------------------------------------------------------------
// <copyright file="RoleData.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Structs
{
    using System;

    using PlayerRoles;

    /// <summary>
    /// A struct representing all data regarding a fake role.
    /// </summary>
    public struct RoleData : IEquatable<RoleData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleData"/> struct.
        /// </summary>
        /// <param name="role">The fake role.</param>
        /// <param name="unitId">The fake UnitID, if <paramref name="role"/> is an NTF role.</param>
        public RoleData(RoleTypeId role = RoleTypeId.None, byte unitId = 0)
        {
            Role = role;
            UnitId = unitId;
        }

        /// <summary>
        /// Gets the static <see cref="RoleData"/> representing no data.
        /// </summary>
        public static RoleData None { get; } = new(RoleTypeId.None);

        /// <summary>
        /// Gets or sets the fake role.
        /// </summary>
        public RoleTypeId Role { get; set; }

        /// <summary>
        /// Gets or sets the UnitID of the fake role, if <see cref="Role"/> is an NTF role.
        /// </summary>
        public byte UnitId { get; set; }

        /// <summary>
        /// Checks if 2 <see cref="RoleData"/> are equal.
        /// </summary>
        /// <param name="left">A <see cref="RoleData"/>.</param>
        /// <param name="right">The other <see cref="RoleData"/>.</param>
        /// <returns>Whether the parameters are equal.</returns>
        public static bool operator ==(RoleData left, RoleData right) => left.Equals(right);

        /// <summary>
        /// Checks if 2 <see cref="RoleData"/> are not equal.
        /// </summary>
        /// <param name="left">A <see cref="RoleData"/>.</param>
        /// <param name="right">The other <see cref="RoleData"/>.</param>
        /// <returns>Whether the parameters are not equal.</returns>
        public static bool operator !=(RoleData left, RoleData right) => !left.Equals(right);

        /// <inheritdoc/>
        public bool Equals(RoleData other) => Role == other.Role && UnitId == other.UnitId;

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is RoleData other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)Role * 397) ^ UnitId.GetHashCode();
            }
        }
    }
}