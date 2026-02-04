// -----------------------------------------------------------------------
// <copyright file="Draw.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;

    using DrawableLine;

    using Mirror;

    using UnityEngine;

    /// <summary>
    /// A utility class for drawing debug lines, shapes, and bounds for players or globally.
    /// </summary>
    public static class Draw
    {
        /// <summary>
        /// Draws a line between two specified points.
        /// </summary>
        /// <param name="start">The starting position of the line.</param>
        /// <param name="end">The ending position of the line.</param>
        /// <param name="color">The color of the lines.</param>
        /// <param name="duration"> How long the line should remain visible.<para><warning><b>Warning:</b> Avoid using <see cref="float.PositiveInfinity"/> or extremely large values, as these lines cannot be removed from the client once sent.</warning></para></param>
        /// <param name="players">A collection of <see cref="Player"/>s to show the line to.</param>
        public static void Line(Vector3 start, Vector3 end, Color color, float duration, IEnumerable<Player> players = null)
        {
            Send(players, duration, color, start, end);
        }

        /// <summary>
        /// Draws a connected path through a series of points.
        /// </summary>
        /// <param name="points">An array of <see cref="Vector3"/> points to connect sequentially.</param>
        /// <param name="color">The color of the lines.</param>
        /// <param name="duration"> How long the line should remain visible.<para><warning><b>Warning:</b> Avoid using <see cref="float.PositiveInfinity"/> or extremely large values, as these lines cannot be removed from the client once sent.</warning></para></param>
        /// <param name="players">A collection of <see cref="Player"/>s to show the path to.</param>
        public static void Path(Vector3[] points, Color color, float duration, IEnumerable<Player> players = null)
        {
            Send(players, duration, color, points);
        }

        /// <summary>
        /// Draws a circle at a specific position.
        /// </summary>
        /// <param name="origin">The center point of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="color">The color of the lines.</param>
        /// <param name="duration"> How long the line should remain visible.<para><warning><b>Warning:</b> Avoid using <see cref="float.PositiveInfinity"/> or extremely large values, as these lines cannot be removed from the client once sent.</warning></para></param>
        /// <param name="players">A collection of <see cref="Player"/>s to show the circle to.</param>
        /// <param name="horizontal">Indicates whether the circle should be drawn on the horizontal plane (XZ) or vertical plane (XY).</param>
        /// <param name="segments">The number of line segments used to draw the circle. Higher values result in a smoother circle.</param>
        public static void Circle(Vector3 origin, float radius, Color color, float duration, IEnumerable<Player> players = null, bool horizontal = true, int segments = 16)
        {
            Vector3[] circlePoints = DrawableLines.GetCircle(origin, radius, horizontal, segments);
            Send(players, duration, color, circlePoints);
        }

        /// <summary>
        /// Draws a wireframe sphere composed of two circles (horizontal and vertical).
        /// </summary>
        /// <param name="origin">The center point of the sphere.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <param name="color">The color of the lines.</param>
        /// <param name="duration"> How long the line should remain visible.<para><warning><b>Warning:</b> Avoid using <see cref="float.PositiveInfinity"/> or extremely large values, as these lines cannot be removed from the client once sent.</warning></para></param>
        /// <param name="players">A collection of <see cref="Player"/>s to show the sphere to.</param>
        /// <param name="segments">The number of segments for the circles. Higher values result in a smoother sphere.</param>
        public static void Sphere(Vector3 origin, float radius, Color color, float duration, IEnumerable<Player> players = null, int segments = 16)
        {
            Vector3[] horizontal = DrawableLines.GetCircle(origin, radius, true, segments);
            Send(players, duration, color, horizontal);

            Vector3[] vertical = DrawableLines.GetCircle(origin, radius, false, segments);
            Send(players, duration, color, vertical);
        }

        /// <summary>
        /// Draws the edges of a <see cref="Bounds"/> object.
        /// </summary>
        /// <param name="bounds">The <see cref="Bounds"/> object to visualize.</param>
        /// <param name="color">The color of the lines.</param>
        /// <param name="duration"> How long the line should remain visible.<para><warning><b>Warning:</b> Avoid using <see cref="float.PositiveInfinity"/> or extremely large values, as these lines cannot be removed from the client once sent.</warning></para></param>
        /// <param name="players">A collection of <see cref="Player"/>s to show the bounds to.</param>
        public static void Bounds(Bounds bounds, Color color, float duration, IEnumerable<Player> players = null)
        {
            Vector3 center = bounds.center;
            Vector3 extents = bounds.extents;

            Vector3[] bottomRect = new Vector3[5];
            Vector3[] topRect = new Vector3[5];

            bottomRect[0] = center + new Vector3(-extents.x, -extents.y, -extents.z);
            bottomRect[1] = center + new Vector3(extents.x, -extents.y, -extents.z);
            bottomRect[2] = center + new Vector3(extents.x, -extents.y, extents.z);
            bottomRect[3] = center + new Vector3(-extents.x, -extents.y, extents.z);
            bottomRect[4] = bottomRect[0];

            topRect[0] = center + new Vector3(-extents.x, extents.y, -extents.z);
            topRect[1] = center + new Vector3(extents.x, extents.y, -extents.z);
            topRect[2] = center + new Vector3(extents.x, extents.y, extents.z);
            topRect[3] = center + new Vector3(-extents.x, extents.y, extents.z);
            topRect[4] = topRect[0];

            Send(players, duration, color, bottomRect);
            Send(players, duration, color, topRect);

            for (int i = 0; i < 4; i++)
            {
                Send(players, duration, color, bottomRect[i], topRect[i]);
            }
        }

        private static void Send(IEnumerable<Player> players, float duration, Color color, params Vector3[] points)
        {
            if (points == null || points.Length < 2)
                return;

            DrawableLineMessage msg = new(duration, color, points);

            if (players != null)
            {
                using NetworkWriterPooled writer = NetworkWriterPool.Get();
                NetworkMessages.Pack(msg, writer);
                ArraySegment<byte> segment = writer.ToArraySegment();

                foreach (Player ply in players)
                {
                    ply?.Connection.Send(segment);
                }
            }
            else
            {
                NetworkServer.SendToReady(msg);
            }
        }
    }
}