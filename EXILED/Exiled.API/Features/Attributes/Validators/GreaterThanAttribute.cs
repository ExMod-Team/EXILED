// -----------------------------------------------------------------------
// <copyright file="GreaterThanAttribute.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes.Validators
{
    using System;

    using Interfaces;

    /// <summary>
    /// Check if value is greater.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class GreaterThanAttribute : Attribute, IValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GreaterThanAttribute"/> class.
        /// </summary>
        /// <param name="value"><inheritdoc cref="Value"/></param>
        public GreaterThanAttribute(IComparable value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the minimum value.
        /// </summary>
        public IComparable Value { get; }

        /// <inheritdoc/>
        public bool Check(object value)
        {
            return Value.CompareTo(value) >= 1;
        }
    }
}