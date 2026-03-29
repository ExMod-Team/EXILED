// -----------------------------------------------------------------------
// <copyright file="AspectRatioType.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// All available screen aspect ratio types.
    /// </summary>
    public enum AspectRatioType : byte
    {
        /// <summary>
        /// Unknown aspect ratio.
        /// </summary>
        Unknown,

        /// <summary>
        /// 1:1 aspect ratio (square screen).
        /// </summary>
        Ratio1_1,

        /// <summary>
        /// 3:2 aspect ratio.
        /// </summary>
        Ratio3_2,

        /// <summary>
        /// 4:3 aspect ratio (standard definition TVs, older monitors).
        /// </summary>
        Ratio4_3,

        /// <summary>
        /// 5:4 aspect ratio (some older computer monitors).
        /// </summary>
        Ratio5_4,

        /// <summary>
        /// 16:9 aspect ratio (modern widescreen displays, HDTV).
        /// </summary>
        Ratio16_9,

        /// <summary>
        /// 16:10 aspect ratio (common in productivity monitors and laptops).
        /// </summary>
        Ratio16_10,

        /// <summary>
        /// 21:9 aspect ratio (ultrawide displays).
        /// </summary>
        Ratio21_9,

        /// <summary>
        /// 32:9 aspect ratio (super ultrawide displays).
        /// </summary>
        Ratio32_9,
    }
}
