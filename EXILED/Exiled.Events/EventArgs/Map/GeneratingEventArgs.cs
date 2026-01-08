// -----------------------------------------------------------------------
// <copyright file="GeneratingEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using Exiled.API.Enums;
    using Exiled.Events.EventArgs.Interfaces;
    using UnityEngine;

    /// <summary>
    /// Contains all information after the server generates a seed, but before the map is generated.
    /// </summary>
    public class GeneratingEventArgs : IDeniableEvent
    {
        private int seed;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratingEventArgs"/> class.
        /// </summary>
        /// <param name="seed"><inheritdoc cref="Seed"/></param>
        /// <param name="lcz"><inheritdoc cref="LczLayout"/></param>
        /// <param name="hcz"><inheritdoc cref="HczLayout"/></param>
        /// <param name="ez"><inheritdoc cref="EzLayout"/></param>
        public GeneratingEventArgs(int seed, LczFacilityLayout lcz, HczFacilityLayout hcz, EzFacilityLayout ez)
        {
            LczLayout = lcz;
            HczLayout = hcz;
            EzLayout = ez;

            Seed = seed;
            IsAllowed = true;
        }

        /// <summary>
        /// Gets or sets the layout of the light containment zone.
        /// </summary>
        public LczFacilityLayout LczLayout { get; set; }

        /// <summary>
        /// Gets or sets the layout of the heavy containment zone.
        /// </summary>
        public HczFacilityLayout HczLayout { get; set; }

        /// <summary>
        /// Gets or sets the layout of the entrance zone.
        /// </summary>
        public EzFacilityLayout EzLayout { get; set; }

        /// <summary>
        /// Gets or sets the seed of the map.
        /// </summary>
        /// <remarks>This property overrides any changes in <see cref="LczLayout"/>, <see cref="HczLayout"/>, or <see cref="EzLayout"/>.</remarks>
        public int Seed
        {
            get => seed;
            set => seed = Mathf.Clamp(value, 0, int.MaxValue);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the map can be generated.
        /// </summary>
        /// <remarks>This property overrides any changes in <see cref="Seed"/>.</remarks>
        public bool IsAllowed { get; set; }
    }
}