// -----------------------------------------------------------------------
// <copyright file="Light.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Toys
{
    using System;
    using System.Linq;

    using AdminToys;

    using Enums;

    using Exiled.API.Interfaces;

    using UnityEngine;

    using Object = UnityEngine.Object;

    /// <summary>
    /// A wrapper class for <see cref="LightSourceToy"/>.
    /// </summary>
    public class Light : AdminToy, IWrapper<LightSourceToy>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Light"/> class.
        /// </summary>
        /// <param name="lightSourceToy">The <see cref="LightSourceToy"/> of the toy.</param>
        internal Light(LightSourceToy lightSourceToy)
            : base(lightSourceToy, AdminToyType.LightSource)
        {
            Base = lightSourceToy;
        }

        /// <summary>
        /// Gets the prefab.
        /// </summary>
        public static LightSourceToy Prefab { get; internal set; }

        /// <summary>
        /// Gets the base <see cref="LightSourceToy"/>.
        /// </summary>
        public LightSourceToy Base { get; }

        /// <summary>
        /// Gets or sets the intensity of the light.
        /// </summary>
        public float Intensity
        {
            get => Base.NetworkLightIntensity;
            set => Base.NetworkLightIntensity = value;
        }

        /// <summary>
        /// Gets or sets the range of the light.
        /// </summary>
        public float Range
        {
            get => Base.NetworkLightRange;
            set => Base.NetworkLightRange = value;
        }

        /// <summary>
        /// Gets or sets the angle of the light.
        /// </summary>
        public float SpotAngle
        {
            get => Base.NetworkSpotAngle;
            set => Base.NetworkSpotAngle = value;
        }

        /// <summary>
        /// Gets or sets the inner angle of the light.
        /// </summary>
        public float InnerSpotAngle
        {
            get => Base.NetworkInnerSpotAngle;
            set => Base.NetworkInnerSpotAngle = value;
        }

        /// <summary>
        /// Gets or sets the shadow strength of the light.
        /// </summary>
        public float ShadowStrength
        {
            get => Base.NetworkShadowStrength;
            set => Base.NetworkShadowStrength = value;
        }

        /// <summary>
        /// Gets or sets the color of the primitive.
        /// </summary>
        public Color Color
        {
            get => Base.NetworkLightColor;
            set => Base.NetworkLightColor = value;
        }

        /// <summary>
        /// Gets or sets the shape that the Light emits.
        /// </summary>
        [Obsolete("This property has been deprecated. Use LightType.Spot, LightType.Pyramid, or LightType.Box instead.")]
        public LightShape LightShape
        {
            get => Base.NetworkLightShape;
            set => Base.NetworkLightShape = value;
        }

        /// <summary>
        /// Gets or sets the type of light the Light emits.
        /// </summary>
        public LightType LightType
        {
            get => Base.NetworkLightType;
            set => Base.NetworkLightType = value;
        }

        /// <summary>
        /// Gets or sets the type of shadows the light casts.
        /// </summary>
        public LightShadows ShadowType
        {
            get => Base.NetworkShadowType;
            set => Base.NetworkShadowType = value;
        }

        /// <summary>
        /// Creates a new <see cref="Light"/>.
        /// </summary>
        /// <param name="position">The position of the <see cref="Light"/>.</param>
        /// <param name="color">The color of the <see cref="Light"/>.</param>
        /// <returns>The new <see cref="Light"/>.</returns>
        public static Light Create(Vector3 position, Color color) => Create(position: position, color: color, spawn: true);

        /// <summary>
        /// Creates a new <see cref="Light"/>.
        /// </summary>
        /// <param name="parent">The transform to create this <see cref="Light"/> on.</param>
        /// <param name="position">The local position of the <see cref="Light"/>.</param>
        /// <param name="rotation">The local rotation of the <see cref="Light"/>.</param>
        /// <param name="scale">The scale of the <see cref="Light"/>.</param>
        /// <param name="color">The color of the <see cref="Light"/>.</param>
        /// <param name="intensity">The intensity of the light.</param>
        /// <param name="range">The range of the light.</param>
        /// <param name="spotAngle">The angle of the light.</param>
        /// <param name="innerSpotAngle">The inner angle of the light.</param>
        /// <param name="shadowStrength">The shadow strength of the light.</param>
        /// <param name="lightType">The type of light the Light emits.</param>
        /// <param name="shadowType">The type of shadows the light casts.</param>
        /// <param name="spawn">Whether the <see cref="Light"/> should be initially spawned.</param>
        /// <returns>The new <see cref="Light"/>.</returns>
        public static Light Create(Transform parent = null, Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null, Color? color = null, float? intensity = null, float? range = null, float? spotAngle = null, float? innerSpotAngle = null, float? shadowStrength = null, LightType? lightType = null, LightShadows? shadowType = null, bool spawn = true)
        {
            Light toy = new(Object.Instantiate(Prefab, parent))
            {
                LocalPosition = position ?? Vector3.zero,
                LocalRotation = rotation ?? Quaternion.identity,
                Scale = scale ?? Vector3.one,
                Color = color ?? Color.white,
            };

            if (intensity.HasValue)
                toy.Intensity = intensity.Value;

            if (range.HasValue)
                toy.Range = range.Value;

            if (spotAngle.HasValue)
                toy.SpotAngle = spotAngle.Value;

            if (innerSpotAngle.HasValue)
                toy.InnerSpotAngle = innerSpotAngle.Value;

            if (shadowStrength.HasValue)
                toy.ShadowStrength = shadowStrength.Value;

            if (lightType.HasValue)
                toy.LightType = lightType.Value;

            if (shadowType.HasValue)
                toy.ShadowType = shadowType.Value;

            if (spawn)
                toy.Spawn();

            return toy;
        }

        /// <summary>
        /// Gets the <see cref="Light"/> belonging to the <see cref="LightSourceToy"/>.
        /// </summary>
        /// <param name="lightSourceToy">The <see cref="LightSourceToy"/> instance.</param>
        /// <returns>The corresponding <see cref="LightSourceToy"/> instance.</returns>
        [Obsolete("This method is deprecated, please use the generic Get<T> method instead.")]
        public static Light Get(LightSourceToy lightSourceToy) => Get<Light>(lightSourceToy);
    }
}