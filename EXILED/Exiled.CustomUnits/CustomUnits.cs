// -----------------------------------------------------------------------
// <copyright file="CustomUnits.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomUnits
{
    using Exiled.API.Features;

    /// <summary>
    /// A class for custom units that implements <see cref="Plugin{T}"/>.
    /// </summary>
    public class CustomUnits : Plugin<Config>
    {
        /// <summary>
        /// Gets the current instance of <see cref="CustomUnits"/>.
        /// </summary>
        public static CustomUnits Instance { get; private set; }

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Instance = this;

            base.OnEnabled();
        }
    }
}