// -----------------------------------------------------------------------
// <copyright file="RoleData.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Structs
{
    using System;

    using Mirror;
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
        /// <param name="authority">The authority of the role data.</param>
        /// <param name="unitId">The fake UnitID, if <paramref name="role"/> is an NTF role.</param>
        public RoleData(RoleTypeId role = RoleTypeId.None, Authority authority = Authority.None, byte unitId = 0)
        {
            Role = role;
            DataAuthority = authority;
            UnitId = unitId;
        }

        /// <summary>
        /// Represents flags for how Exiled should handle edge cases.
        /// </summary>
        [Flags]
        public enum Authority
        {
            /// <summary>
            /// Indicates Exiled should only fake the role of the target of this <see cref="RoleData"/> in ideal conditions.
            /// </summary>
            None = 0,

            /// <summary>
            /// Indicates that Exiled should attempt to override other plugins fake role attempts if they exist.
            /// </summary>
            /// <remarks>This is not guaranteed to always work.</remarks>
            Override = 1,

            /// <summary>
            /// Indicates that the fake role should always be sent without checking if the player is dead, etc...
            /// </summary>
            Always = 2,

            /// <summary>
            /// Indicates that Exiled should not reset the fake role if the target of this <see cref="RoleData"/> dies.
            /// </summary>
            Persist = 4,
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
        /// Gets or sets the authority of this <see cref="RoleData"/> instance. see <see cref="Authority"/> for details.
        /// </summary>
        public Authority DataAuthority { get; set; } = Authority.None;

        /// <summary>
        /// Gets or sets custom data written to network writers when fake data is generated.
        /// </summary>
        /// <remarks>Leave this value as null unless you are writing custom role-specific data.</remarks>
        public Action<NetworkWriter> CustomData { get; set; }

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
        public bool Equals(RoleData other) => Role == other.Role && DataAuthority == other.DataAuthority && UnitId == other.UnitId;

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is RoleData other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine((int)Role, UnitId, (int)DataAuthority, CustomData);
    }
}