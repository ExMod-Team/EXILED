// -----------------------------------------------------------------------
// <copyright file="Draw.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using DrawableLine;

    using UnityEngine;

    /// <summary>
    /// A utility class for drawing debug lines, shapes, and bounds for players or globally.
    /// </summary>
    public static class Draw
    {
        private const float DefaultDuration = 5f;

        private static readonly Color DefaultColor = Color.green;

        /// <summary>
        /// Draws a line between two specified points.
        /// </summary>
        /// <param name="start">The starting position of the line.</param>
        /// <param name="end">The ending position of the line.</param>
        /// <param name="player">The <see cref="Player"/> to show the line to. If <see langword="null"/>, it is shown to all players.</param>
        /// <param name="color">The color of the line. Defaults to <see cref="Color.green"/>.</param>
        /// <param name="duration">How long the line should remain visible.</param>
        public static void Line(Vector3 start, Vector3 end, Player player = null, Color? color = null, float duration = DefaultDuration)
        {
            Send(player, duration, color ?? DefaultColor, start, end);
        }

        /// <summary>
        /// Draws a connected path through a series of points.
        /// </summary>
        /// <param name="points">An array of <see cref="Vector3"/> points to connect sequentially.</param>
        /// <param name="player">The <see cref="Player"/> to show the path to. If <see langword="null"/>, it is shown to all players.</param>
        /// <param name="color">The color of the path. Defaults to <see cref="Color.green"/>.</param>
        /// <param name="duration">How long the path should remain visible.</param>
        public static void Path(Vector3[] points, Player player = null, Color? color = null, float duration = DefaultDuration)
        {
            Send(player, duration, color ?? DefaultColor, points);
        }

        /// <summary>
        /// Draws a circle at a specific position.
        /// </summary>
        /// <param name="origin">The center point of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="player">The <see cref="Player"/> to show the circle to. If <see langword="null"/>, it is shown to all players.</param>
        /// <param name="horizontal">Indicates whether the circle should be drawn on the horizontal plane (XZ) or vertical plane (XY).</param>
        /// <param name="segments">The number of line segments used to draw the circle. Higher values result in a smoother circle.</param>
        /// <param name="color">The color of the circle. Defaults to <see cref="Color.green"/>.</param>
        /// <param name="duration">How long the circle should remain visible.</param>
        public static void Circle(Vector3 origin, float radius, Player player = null, bool horizontal = true, int segments = 16, Color? color = null, float duration = DefaultDuration)
        {
            Vector3[] circlePoints = DrawableLines.GetCircle(origin, radius, horizontal, segments);
            Send(player, duration, color ?? DefaultColor, circlePoints);
        }

        /// <summary>
        /// Draws a wireframe sphere composed of two circles (horizontal and vertical).
        /// </summary>
        /// <param name="origin">The center point of the sphere.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <param name="player">The <see cref="Player"/> to show the sphere to. If <see langword="null"/>, it is shown to all players.</param>
        /// <param name="segments">The number of segments for the circles. Higher values result in a smoother sphere.</param>
        /// <param name="color">The color of the sphere. Defaults to <see cref="Color.green"/>.</param>
        /// <param name="duration">How long the sphere should remain visible.</param>
        public static void Sphere(Vector3 origin, float radius, Player player = null, int segments = 16, Color? color = null, float duration = DefaultDuration)
        {
            Color finalColor = color ?? DefaultColor;

            Vector3[] horizontal = DrawableLines.GetCircle(origin, radius, true, segments);
            Send(player, duration, finalColor, horizontal);

            Vector3[] vertical = DrawableLines.GetCircle(origin, radius, false, segments);
            Send(player, duration, finalColor, vertical);
        }

        /// <summary>
        /// Draws the edges of a <see cref="Bounds"/> object (box).
        /// </summary>
        /// <param name="bounds">The <see cref="Bounds"/> object to visualize.</param>
        /// <param name="player">The <see cref="Player"/> to show the bounds to. If <see langword="null"/>, it is shown to all players.</param>
        /// <param name="duration">How long the bounds should remain visible.</param>
        /// <param name="color">The color of the box. Defaults to <see cref="Color.green"/>.</param>
        public static void Bounds(Bounds bounds, Player player = null, float duration = DefaultDuration, Color? color = null)
        {
            Color finalColor = color ?? DefaultColor;
            Vector3 center = bounds.center;
            Vector3 extents = bounds.extents;

            Vector3[] array = new Vector3[5];
            Vector3[] array2 = new Vector3[5];

            array[0] = center + new Vector3(-extents.x, -extents.y, -extents.z);
            array[1] = center + new Vector3(extents.x, -extents.y, -extents.z);
            array[2] = center + new Vector3(extents.x, -extents.y, extents.z);
            array[3] = center + new Vector3(-extents.x, -extents.y, extents.z);
            array[4] = array[0];

            array2[0] = center + new Vector3(-extents.x, extents.y, -extents.z);
            array2[1] = center + new Vector3(extents.x, extents.y, -extents.z);
            array2[2] = center + new Vector3(extents.x, extents.y, extents.z);
            array2[3] = center + new Vector3(-extents.x, extents.y, extents.z);
            array2[4] = array2[0];

            Send(player, duration, finalColor, array);
            Send(player, duration, finalColor, array2);

            for (int i = 0; i < 4; i++)
            {
                Send(player, duration, finalColor, array[i], array2[i]);
            }
        }

        private static void Send(Player player, float duration, Color color, params Vector3[] points)
        {
            if (points == null || points.Length < 2)
                return;

            bool debugWas = DrawableLines.IsDebugModeEnabled;
            DrawableLines.IsDebugModeEnabled = true;

            if (player != null)
            {
                player.Connection.Send(new DrawableLineMessage(duration, color, positions));
            }
            else
            {
                DrawableLines.ServerGenerateLine(duration, color, positions: points);
            }

            DrawableLines.IsDebugModeEnabled = debugWas;
        }
    }
}