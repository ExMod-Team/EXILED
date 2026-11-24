// -----------------------------------------------------------------------
// <copyright file="MetalGate.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Hazards
{
    using System.Collections.Generic;

    using Exiled.API.Features.Core;
    using Exiled.API.Interfaces;

    using global::Hazards;

    using PlayerRoles;

    /// <summary>
    /// A wrapper for <see cref="MetalGateHazard"/>.
    /// </summary>
    public class MetalGate : TypeCastObject<MetalGate>, IWrapper<MetalGateHazard>
    {
        /// <summary>
        /// <see cref="Dictionary{TKey,TValue}"/> with <see cref="MetalGateHazard"/> to it's <see cref="Hazard"/>.
        /// </summary>
        internal static readonly Dictionary<MetalGateHazard, MetalGate> MetalGateHazardToMetalGate = new(new ComponentsEqualityComparer());

        /// <summary>
        /// Initializes a new instance of the <see cref="MetalGate"/> class.
        /// </summary>
        /// <param name="hazard">The <see cref="MetalGateHazard"/> instance.</param>
        public MetalGate(MetalGateHazard hazard)
        {
            Base = hazard;

            MetalGateHazardToMetalGate.Add(hazard, this);
        }

        /// <summary>
        /// Gets the list of all hazards.
        /// </summary>
        public static IReadOnlyCollection<MetalGate> List => MetalGateHazardToMetalGate.Values;

        /// <summary>
        /// Gets the <see cref="MetalGateHazard"/>.
        /// </summary>
        public MetalGateHazard Base { get; }
    }
}