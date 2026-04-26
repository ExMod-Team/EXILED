// -----------------------------------------------------------------------
// <copyright file="NonPositiveAttribute.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes.Validators
{
    using System;

    using Exiled.API.Interfaces;

    /// <summary>
    /// Check if value is 0 or less.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NonPositiveAttribute : Attribute, IValidator
    {
        /// <inheritdoc/>
        public bool Check(object other) => Convert.ToDecimal(other) <= 0;
    }
}