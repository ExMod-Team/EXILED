// -----------------------------------------------------------------------
// <copyright file="NonNegativeAttribute.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes.Validators
{
    using System;

    using Exiled.API.Interfaces;

    /// <summary>
    /// Checks if value is 0 or greater.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NonNegativeAttribute : Attribute, IValidator
    {
        /// <inheritdoc/>
        public bool Check(object other) => Convert.ToDecimal(other) >= 0;
    }
}