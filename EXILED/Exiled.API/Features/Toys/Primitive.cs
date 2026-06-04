// -----------------------------------------------------------------------
// <copyright file="Primitive.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Toys
{
    using AdminToys;

    using Enums;

    using Exiled.API.Interfaces;
    using Exiled.API.Structs;

    using UnityEngine;

    using Object = UnityEngine.Object;

    /// <summary>
    /// A wrapper class for <see cref="PrimitiveObjectToy"/>.
    /// </summary>
    public class Primitive : AdminToy, IWrapper<PrimitiveObjectToy>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Primitive"/> class.
        /// </summary>
        /// <param name="toyAdminToyBase">The <see cref="PrimitiveObjectToy"/> of the toy.</param>
        internal Primitive(PrimitiveObjectToy toyAdminToyBase)
            : base(toyAdminToyBase, AdminToyType.PrimitiveObject) => Base = toyAdminToyBase;

        /// <summary>
        /// Gets the prefab.
        /// </summary>
        public static PrimitiveObjectToy Prefab { get; internal set; }

        /// <summary>
        /// Gets the base <see cref="PrimitiveObjectToy"/>.
        /// </summary>
        public PrimitiveObjectToy Base { get; }

        /// <summary>
        /// Gets or sets the type of the primitive.
        /// </summary>
        public PrimitiveType Type
        {
            get => Base.NetworkPrimitiveType;
            set => Base.NetworkPrimitiveType = value;
        }

        /// <summary>
        /// Gets or sets the material color of the primitive.
        /// </summary>
        public Color Color
        {
            get => Base.NetworkMaterialColor;
            set => Base.NetworkMaterialColor = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the primitive can be collided with.
        /// </summary>
        public bool Collidable
        {
            get => Flags.HasFlag(PrimitiveFlags.Collidable);
            set => Flags = value ? (Flags | PrimitiveFlags.Collidable) : (Flags & ~PrimitiveFlags.Collidable);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the primitive is visible.
        /// </summary>
        public bool Visible
        {
            get => Flags.HasFlag(PrimitiveFlags.Visible);
            set => Flags = value ? (Flags | PrimitiveFlags.Visible) : (Flags & ~PrimitiveFlags.Visible);
        }

        /// <summary>
        /// Gets or sets the primitive flags.
        /// </summary>
        public PrimitiveFlags Flags
        {
            get => Base.NetworkPrimitiveFlags;
            set => Base.NetworkPrimitiveFlags = value;
        }

        /// <summary>
        /// Converts PrimitiveObjectToy to Primitive.
        /// </summary>
        /// <param name="primitiveObjectToy">The PrimitiveObjectToy.</param>
        /// <returns>EXILED Primitive.</returns>
        public static implicit operator Primitive(PrimitiveObjectToy primitiveObjectToy) => (Primitive)Get(primitiveObjectToy);

        /// <summary>
        /// Creates a new <see cref="Primitive"/>.
        /// </summary>
        /// <param name="type">The type of primitive to spawn.</param>
        /// <param name="position">The position of the <see cref="Primitive"/>.</param>
        /// <returns>The new <see cref="Primitive"/>.</returns>
        public static Primitive Create(PrimitiveType type, Vector3 position) => Create(type: type, position: position, spawn: true);

        /// <summary>
        /// Creates a new <see cref="Primitive"/>.
        /// </summary>
        /// <param name="parent">The transform to create this <see cref="Primitive"/> on.</param>
        /// <param name="position">The local position of the <see cref="Primitive"/>.</param>
        /// <param name="rotation">The local rotation of the <see cref="Primitive"/>.</param>
        /// <param name="scale">The scale of the <see cref="Primitive"/>.</param>
        /// <param name="type">The type of primitive to spawn.</param>
        /// <param name="flags">The primitive flags.</param>
        /// <param name="color">The color of the <see cref="Primitive"/>.</param>
        /// <param name="spawn">Whether the <see cref="Primitive"/> should be initially spawned.</param>
        /// <returns>The new <see cref="Primitive"/>.</returns>
        public static Primitive Create(Transform parent = null, Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null, PrimitiveType type = PrimitiveType.Sphere, PrimitiveFlags flags = PrimitiveFlags.Visible | PrimitiveFlags.Collidable, Color? color = null, bool spawn = true)
        {
            Primitive toy = new(Object.Instantiate(Prefab, parent))
            {
                Type = type,
                Flags = flags,
                LocalPosition = position ?? Vector3.zero,
                LocalRotation = rotation ?? Quaternion.identity,
                Scale = scale ?? Vector3.one,
                Color = color ?? Color.gray,
            };

            if (spawn)
                toy.Spawn();

            return toy;
        }

        /// <summary>
        /// Creates a new <see cref="Primitive"/> with using <see cref="PrimitiveSettings"/>.
        /// </summary>
        /// <param name="primitiveSettings">The settings of the <see cref="Primitive"/>.</param>
        /// <returns>The new <see cref="Primitive"/>.</returns>
        public static Primitive Create(PrimitiveSettings primitiveSettings)
        {
            Primitive toy = new(Object.Instantiate(Prefab))
            {
                Type = primitiveSettings.PrimitiveType,
                Flags = primitiveSettings.Flags,
                LocalPosition = primitiveSettings.Position,
                LocalRotation = Quaternion.Euler(primitiveSettings.Rotation),
                Scale = primitiveSettings.Scale,
                Color = primitiveSettings.Color,
                IsStatic = primitiveSettings.IsStatic,
            };

            if (primitiveSettings.Spawn)
                toy.Spawn();

            return toy;
        }
    }
}