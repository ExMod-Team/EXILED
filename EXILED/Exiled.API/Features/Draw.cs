// -----------------------------------------------------------------------
// <copyright file="Draw.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Buffers;
    using System.Collections.Generic;

    using DrawableLine;

    using Exiled.API.Features.Pools;

    using Mirror;

    using UnityEngine;

    using Utils;

    /// <summary>
    /// A utility class for drawing debug lines, shapes, and bounds for players or globally.
    /// </summary>
    public static class Draw
    {
        // smallest array that fits the largest default segment (17 for sphere)
        private static readonly Vector3[] ArrayNonAlloc17 = new Vector3[5];

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
            ArrayNonAlloc17[0] = start;
            ArrayNonAlloc17[1] = end;

            Send(players, duration, color, ArrayNonAlloc17, 2);
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
        /// <param name="rotation">The rotation of the circle.</param>
        /// <param name="scale">The scale of the circle.</param>
        /// <param name="color">The color of the lines.</param>
        /// <param name="duration"> How long the line should remain visible.<para><warning><b>Warning:</b> Avoid using <see cref="float.PositiveInfinity"/> or extremely large values, as these lines cannot be removed from the client once sent.</warning></para></param>
        /// <param name="players">A collection of <see cref="Player"/>s to show the circle to.</param>
        /// <param name="horizontal">Indicates whether the circle should be drawn on the horizontal plane (XZ) or vertical plane (XY).</param>
        /// <param name="segments">The number of line segments used to draw the circle. Higher values result in a smoother circle.</param>
        public static void Circle(Vector3 origin, Quaternion rotation, Vector3 scale, Color color, float duration, IEnumerable<Player> players = null, bool horizontal = true, int segments = 16)
        {
            Vector3[] circlePoints = GetCirclePoints(origin, rotation, scale, segments, horizontal);
            Send(players, duration, color, circlePoints);
        }

        /// <summary>
        /// Draws a wireframe sphere composed of two circles (horizontal and vertical).
        /// </summary>
        /// <param name="origin">The center point of the sphere.</param>
        /// <param name="rotation">The rotation of the sphere.</param>
        /// <param name="scale">The scale of the sphere.</param>
        /// <param name="color">The color of the lines.</param>
        /// <param name="duration"> How long the line should remain visible.<para><warning><b>Warning:</b> Avoid using <see cref="float.PositiveInfinity"/> or extremely large values, as these lines cannot be removed from the client once sent.</warning></para></param>
        /// <param name="players">A collection of <see cref="Player"/>s to show the sphere to.</param>
        /// <param name="segments">The number of segments for the circles. Higher values result in a smoother sphere.</param>
        public static void Sphere(Vector3 origin, Quaternion rotation, Vector3 scale, Color color, float duration, IEnumerable<Player> players = null, int segments = 16)
        {
            List<Player> list = ListPool<Player>.Pool.Get(players);

            Vector3[] horizontal = GetCirclePoints(origin, rotation, scale, segments, true);
            Send(list, duration, color, horizontal, segments);

            Vector3[] vertical = GetCirclePoints(origin, rotation, scale, segments, false);
            Send(list, duration, color, vertical, segments);

            ListPool<Player>.Pool.Return(list);
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
            Box(bounds.center, bounds.size, Quaternion.identity, color, duration, players);
        }

        /// <summary>
        /// Draws the edges of a <see cref="RelativeBounds"/> object.
        /// </summary>
        /// <param name="relativeBounds">The <see cref="RelativeBounds"/> object to visualize.</param>
        /// <param name="color">The color of the lines.</param>
        /// <param name="duration"> How long the line should remain visible.<para><warning><b>Warning:</b> Avoid using <see cref="float.PositiveInfinity"/> or extremely large values, as these lines cannot be removed from the client once sent.</warning></para></param>
        /// <param name="players">A collection of <see cref="Player"/>s to show the bounds to.</param>
        public static void RelativeBounds(RelativeBounds relativeBounds, Color color, float duration, IEnumerable<Player> players = null)
        {
            Box(relativeBounds.Origin, relativeBounds.Bounds.size, relativeBounds.Rotation, color, duration, players);
        }

        /// <summary>
        /// Draws a collider.
        /// </summary>
        /// <param name="collider">The <see cref="BoxCollider"/> object to visualize.</param>
        /// <param name="color">The color of the lines.</param>
        /// <param name="duration"> How long the line should remain visible.<para><warning><b>Warning:</b> Avoid using <see cref="float.PositiveInfinity"/> or extremely large values, as these lines cannot be removed from the client once sent.</warning></para></param>
        /// <param name="players">A collection of <see cref="Player"/>s to show the bounds to.</param>
        public static void Collider(Collider collider, Color color, float duration, IEnumerable<Player> players = null)
        {
            switch (collider)
            {
                case BoxCollider box:
                    Vector3 worldSize = Vector3.Scale(box.size, box.transform.lossyScale);
                    Vector3 worldCenter = box.transform.TransformPoint(box.center);
                    Box(worldCenter, worldSize, box.transform.rotation, color, duration, players);
                    break;

                case SphereCollider sphere:
                    Vector3 worldScale2 = Vector3.Scale(Vector3.one * sphere.radius, sphere.transform.lossyScale);
                    Vector3 worldCenter2 = sphere.transform.TransformPoint(sphere.center);
                    Sphere(worldCenter2, sphere.transform.rotation, worldScale2, color, duration, players);
                    break;

                case MeshCollider mesh:
                    Mesh(mesh.sharedMesh, mesh.transform, color, duration, players);
                    break;

                case CapsuleCollider capsule:
                    Vector3 lossyScale = capsule.transform.lossyScale;
                    Quaternion directionRotation = capsule.transform.rotation;
                    Vector3 visualScale = lossyScale;

                    switch (capsule.direction)
                    {
                        // X
                        case 0:
                            visualScale = new Vector3(lossyScale.y, lossyScale.x, lossyScale.z);
                            directionRotation *= Quaternion.Euler(0, 0, -90);
                            break;

                        // Y
                        case 1:
                            visualScale = lossyScale;
                            break;

                        // Z
                        case 2:
                            visualScale = new Vector3(lossyScale.x, lossyScale.z, lossyScale.y);
                            directionRotation *= Quaternion.Euler(90, 0, 0);
                            break;
                    }

                    Vector3 finalCenter = capsule.transform.TransformPoint(capsule.center);
                    Capsule(finalCenter, directionRotation, capsule.height, capsule.radius, visualScale, color, duration, players);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Draws a wireframe capsule.
        /// </summary>
        /// <param name="center">The center point of the capsule.</param>
        /// <param name="rotation">The rotation of the capsule.</param>
        /// <param name="height">The height of the capsule.</param>
        /// <param name="radius">The radius of the capsule.</param>
        /// <param name="scale">The scale of the capsule.</param>
        /// <param name="color">The color of the lines.</param>
        /// <param name="duration"> How long the line should remain visible.<para><warning><b>Warning:</b> Avoid using <see cref="float.PositiveInfinity"/> or extremely large values, as these lines cannot be removed from the client once sent.</warning></para></param>
        /// <param name="players">A collection of <see cref="Player"/>s to show the capsule to.</param>
        public static void Capsule(Vector3 center, Quaternion rotation, float height, float radius, Vector3 scale, Color color, float duration, IEnumerable<Player> players = null)
        {
            float sX = Mathf.Abs(scale.x);
            float sY = Mathf.Abs(scale.y);
            float sZ = Mathf.Abs(scale.z);

            float scaledHeight = height * sY;
            float scaledRadiusY = radius * sY;

            float cylinderHeight = Mathf.Max(0, scaledHeight - (scaledRadiusY * 2));
            float halfCylinderHeight = cylinderHeight / 2f;

            Vector3 up = rotation * Vector3.up;
            Vector3 right = rotation * Vector3.right;
            Vector3 forward = rotation * Vector3.forward;
            Vector3 topCenter = center + (up * halfCylinderHeight);
            Vector3 bottomCenter = center - (up * halfCylinderHeight);

            Vector3 ringScale = new(radius * sX, 1f, radius * sZ);

            List<Player> list = ListPool<Player>.Pool.Get(players);

            Circle(topCenter, rotation, ringScale, color, duration, list);
            Circle(bottomCenter, rotation, ringScale, color, duration, list);

            float rX = radius * sX;
            Line(topCenter + (right * rX), bottomCenter + (right * rX), color, duration, list);
            Line(topCenter - (right * rX), bottomCenter - (right * rX), color, duration, list);

            float rZ = radius * sZ;
            Line(topCenter + (forward * rZ), bottomCenter + (forward * rZ), color, duration, list);
            Line(topCenter - (forward * rZ), bottomCenter - (forward * rZ), color, duration, list);

            Vector3 arcScaleSide = new(radius * sZ, radius * sY, 1f);
            Vector3 arcScaleFront = new(radius * sX, radius * sY, 1f);

            const int segments = 8;
            Send(list, duration, color, GetArcPoints(topCenter, rotation, arcScaleSide, 180f, segments), segments);
            Send(list, duration, color, GetArcPoints(topCenter, rotation * Quaternion.Euler(0, 90, 0), arcScaleFront, 180f, segments), segments);
            Send(list, duration, color, GetArcPoints(bottomCenter, rotation * Quaternion.Euler(180, 0, 0), arcScaleSide, 180f, segments), segments);
            Send(list, duration, color, GetArcPoints(bottomCenter, rotation * Quaternion.Euler(180, 90, 0), arcScaleFront, 180f, segments), segments);
        }

        /// <summary>
        /// Draws a wireframe mesh.
        /// </summary>
        /// <param name="mesh">The <see cref="Mesh"/> to visualize.</param>
        /// <param name="transform">The Transform of the mesh.</param>
        /// <param name="color">The color of the lines.</param>
        /// <param name="duration"> How long the line should remain visible.<para><warning><b>Warning:</b> Avoid using <see cref="float.PositiveInfinity"/> or extremely large values, as these lines cannot be removed from the client once sent.</warning></para></param>
        /// <param name="players">A collection of <see cref="Player"/>s to show the capsule to.</param>
        public static void Mesh(Mesh mesh, Transform transform, Color color, float duration, IEnumerable<Player> players = null)
        {
            int[] triangles = mesh.triangles;
            Vector3[] vertices = mesh.vertices;

            List<Player> list = ListPool<Player>.Pool.Get(players);

            for (int i = 0; i < triangles.Length; i += 3)
            {
                ArrayNonAlloc17[0] = transform.TransformPoint(vertices[triangles[i]]);
                ArrayNonAlloc17[1] = transform.TransformPoint(vertices[triangles[i + 1]]);
                ArrayNonAlloc17[2] = transform.TransformPoint(vertices[triangles[i + 2]]);
                ArrayNonAlloc17[3] = ArrayNonAlloc17[0];

                Send(list, duration, color, ArrayNonAlloc17, 4);
            }

            ListPool<Player>.Pool.Return(list);
        }

        /// <summary>
        /// Draws a box using exact dimensions.
        /// </summary>
        /// <param name="center">The center point of the box.</param>
        /// <param name="size">The size of the box.</param>
        /// <param name="rotation">The rotation of the box.</param>
        /// <param name="color">The color of the lines.</param>
        /// <param name="duration"> How long the line should remain visible.<para><warning><b>Warning:</b> Avoid using <see cref="float.PositiveInfinity"/> or extremely large values, as these lines cannot be removed from the client once sent.</warning></para></param>
        /// <param name="players">A collection of <see cref="Player"/>s to show the sphere to.</param>
        public static void Box(Vector3 center, Vector3 size, Quaternion rotation, Color color, float duration, IEnumerable<Player> players = null)
        {
            Vector3 extents = size * 0.5f;

            float width = extents.x;
            float length = extents.z;
            float height = extents.y;

            ArrayNonAlloc17[0] = center + (rotation * new Vector3(-width, -height, -length));
            ArrayNonAlloc17[1] = center + (rotation * new Vector3(width, -height, -length));
            ArrayNonAlloc17[2] = center + (rotation * new Vector3(width, -height, length));
            ArrayNonAlloc17[3] = center + (rotation * new Vector3(-width, -height, length));
            ArrayNonAlloc17[4] = ArrayNonAlloc17[0];

            ArrayNonAlloc17[5] = center + (rotation * new Vector3(-width, height, -length));
            ArrayNonAlloc17[6] = center + (rotation * new Vector3(width, height, -length));
            ArrayNonAlloc17[7] = center + (rotation * new Vector3(width, height, length));
            ArrayNonAlloc17[8] = center + (rotation * new Vector3(-width, height, length));
            ArrayNonAlloc17[9] = ArrayNonAlloc17[5];

            // reduce enumeration
            List<Player> list = ListPool<Player>.Pool.Get(players);

            Send(list, duration, color, ArrayNonAlloc17, 5);
            Send(list, duration, color, ArrayNonAlloc17, 5, 5);

            for (int i = 0; i < 4; i++)
            {
                ArrayNonAlloc17[10] = ArrayNonAlloc17[i];
                ArrayNonAlloc17[11] = ArrayNonAlloc17[i + 5];
                Send(list, duration, color, ArrayNonAlloc17, 2, 10);
            }

            ListPool<Player>.Pool.Return(list);
        }

        private static Vector3[] GetCirclePoints(Vector3 origin, Quaternion rotation, Vector3 scale, int segments, bool horizontal)
        {
            if (segments <= 5)
                segments = 8;

            if (segments % 2 != 0)
                segments++;

            Vector3[] array = segments < 17 ? ArrayNonAlloc17 : new Vector3[segments + 1];
            float num = MathF.PI * 2f / (float)segments;

            for (int i = 0; i < segments; i++)
            {
                float f = (float)i * num;
                float num2 = Mathf.Cos(f);
                float num3 = Mathf.Sin(f);
                Vector3 offset = horizontal ? new Vector3(num2 * scale.x, 0f, num3 * scale.z) : new Vector3(num2 * scale.x, num3 * scale.y, 0f);
                array[i] = origin + (rotation * offset);
            }

            array[segments] = array[0];
            return array;
        }

        private static Vector3[] GetArcPoints(Vector3 origin, Quaternion rotation, Vector3 scale, float angle, int segments = 8)
        {
            Vector3[] array = segments < 17 ? ArrayNonAlloc17 : new Vector3[segments + 1];
            float angleStep = (angle * Mathf.Deg2Rad) / segments;

            for (int i = 0; i <= segments; i++)
            {
                float currentRad = i * angleStep;
                float sin = Mathf.Sin(currentRad);
                float cos = Mathf.Cos(currentRad);
                Vector3 localPoint = new(cos * scale.x, sin * scale.y, 0);
                array[i] = origin + (rotation * localPoint);
            }

            return array;
        }

        private static void Send(IEnumerable<Player> players, float duration, Color color, Vector3[] points, int? count = null, int offset = 0)
        {
            if (points == null || points.Length - offset < 2 || (count ??= points.Length) - offset < 2)
                return;

            ArraySegment<byte> data;
            using (NetworkWriterPooled writer = NetworkWriterPool.Get())
            {
                writer.WriteUShort((ushort)typeof(DrawableLineMessage).FullName.GetStableHashCode());
                writer.WriteFloatNullable(duration);
                writer.WriteColorNullable(color);
                for (int i = offset; i < count + offset; i++)
                    writer.Write(points[i]);
                data = writer.ToArraySegment();
            }

            if (players != null)
            {
                foreach (Player ply in players)
                {
                    ply.Connection.Send(data);
                }
            }
            else
            {
                foreach (NetworkConnectionToClient connectionToClient in NetworkServer.connections.Values)
                {
                    if (connectionToClient.isReady)
                    {
                        connectionToClient.Send(data);
                    }
                }
            }
        }
    }
}