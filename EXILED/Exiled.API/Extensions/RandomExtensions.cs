// -----------------------------------------------------------------------
// <copyright file="RandomExtensions.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;

    /// <summary>
    /// A set of extensions for <see cref="Random"/>.
    /// </summary>
    public static class RandomExtensions
    {
        /// <summary>
        /// Generate a random bool.
        /// </summary>
        /// <param name="rnd"><see cref="Random"/> object.</param>
        /// <returns>Random boolean value.</returns>
        public static bool NextBool(this Random rnd)
        {
            return (rnd.Next() & 1) == 0;
        }
    }
}
