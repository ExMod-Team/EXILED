// -----------------------------------------------------------------------
// <copyright file="IValidator.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Interfaces
{
    /// <summary>
    /// Interface for all validations attributes.
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Checks if <paramref name="value"/> is satisfying this attributes condition.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>Whether the value has passed check.</returns>
        public bool Check(object value);
    }
}