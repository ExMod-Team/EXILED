// -----------------------------------------------------------------------
// <copyright file="FloatExtensions.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Enums;

    /// <summary>
    /// A set of extensions for <see cref="float"/>.
    /// </summary>
    public static class FloatExtensions
    {
        private static readonly Dictionary<AspectRatioType, float> AspectRatioReferences = new()
        {
            { AspectRatioType.Unknown, 0f },
            { AspectRatioType.Ratio1_1, 1f },
            { AspectRatioType.Ratio3_2, 3f / 2f },
            { AspectRatioType.Ratio4_3, 4f / 3f },
            { AspectRatioType.Ratio5_4, 5f / 4f },
            { AspectRatioType.Ratio16_9, 16f / 9f },
            { AspectRatioType.Ratio16_10, 16f / 10f },
            { AspectRatioType.Ratio21_9, 21f / 9f },
            { AspectRatioType.Ratio32_9, 32f / 9f },
        };

        /// <summary>
        /// Gets the closest <see cref="AspectRatioType"/> for a given aspect ratio value.
        /// </summary>
        /// <param name="ratio">The aspect ratio value to compare.</param>
        /// <returns>The closest matching <see cref="AspectRatioType"/>.</returns>
        public static AspectRatioType GetAspectRatioLabel(this float ratio)
        {
            float closestDiff = float.MaxValue;
            AspectRatioType closestRatio = AspectRatioType.Unknown;

            foreach (KeyValuePair<AspectRatioType, float> kvp in AspectRatioReferences)
            {
                float diff = Math.Abs(ratio - kvp.Value);
                if (diff < closestDiff)
                {
                    closestDiff = diff;
                    closestRatio = kvp.Key;
                }
            }

            return closestRatio;
        }
    }
}
