// -----------------------------------------------------------------------
// <copyright file="NonPositiveAttribute.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes.Validators
{
    using System;

    /// <summary>
    /// Check if value is 0 or less.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NonPositiveAttribute : LessOrEqualAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NonPositiveAttribute"/> class.
        /// </summary>
        public NonPositiveAttribute()
            : base(0)
        {
        }
    }
}