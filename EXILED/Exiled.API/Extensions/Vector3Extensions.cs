// -----------------------------------------------------------------------
// <copyright file="Vector3Extensions.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using Exiled.API.Enums;
    using Exiled.API.Features;

    using UnityEngine;

    /// <summary>
    /// A set of extensions for <see cref="Vector3"/> that provide conversions between world space and
    /// room‑relative local space.
    /// </summary>
    public static class Vector3Extensions
    {
        /// <summary>
        /// Converts a world position to a position relative to the specified room's local coordinate system.
        /// </summary>
        /// <param name="worldPos">The world‑space position to convert.</param>
        /// <param name="room">The room whose local space will be used as the reference.</param>
        /// <returns>
        /// The position expressed in the room's local space.
        /// If the room is the <see cref="RoomType.Surface"/>, the original world position is returned unchanged.
        /// </returns>
        public static Vector3 FromWorldToRelativePos(this Vector3 worldPos, Room room)
        {
            if (room.Type == RoomType.Surface)
                return worldPos;
            return room.Transform.InverseTransformPoint(worldPos);
        }

        /// <summary>
        /// Converts a position relative to the specified room's local space back to world space.
        /// </summary>
        /// <param name="relativePos">The local‑space position to convert.</param>
        /// <param name="room">The room whose local space was used as the reference.</param>
        /// <returns>
        /// The position expressed in world space.
        /// If the room is the <see cref="RoomType.Surface"/>, the original local position is returned unchanged
        /// (since surface uses world coordinates directly).
        /// </returns>
        public static Vector3 FromRelativeToWorldPos(this Vector3 relativePos, Room room)
        {
            if (room.Type == RoomType.Surface)
                return relativePos;
            return room.Transform.TransformPoint(relativePos);
        }
    }
}
