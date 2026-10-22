// -----------------------------------------------------------------------
// <copyright file="IEnumerableExtensions.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A set of extensions for <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Perform an action on each element of a collection.
        /// </summary>
        /// <typeparam name="T">Type of <see cref="IEnumerable{T}"/> elements.</typeparam>
        /// <param name="enumerable"><see cref="IEnumerable{T}"/> in this collection, the elements will perform actions.</param>
        /// <param name="action">Action that needs to be performed.</param>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable is null || action is null)
                return;

            foreach (T e in enumerable)
                action(e);
        }
    }
}
