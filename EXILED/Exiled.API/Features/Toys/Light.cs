// -----------------------------------------------------------------------
// <copyright file="Light.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Toys
{
    using System.Linq;

    using AdminToys;

    using Enums;
    using Exiled.API.Interfaces;

    using UnityEngine;

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
        public static LightSourceToy Prefab => PrefabHelper.GetPrefab<LightSourceToy>(PrefabType.LightSourceToy);

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
    }
}