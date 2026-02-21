// -----------------------------------------------------------------------
// <copyright file="GreaterOrEqualAttribute.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes.Validators
{
    using System;

    using Exiled.API.Interfaces;

    /// <summary>
    /// Checks if value greater or equal.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class GreaterOrEqualAttribute : Attribute, IValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GreaterOrEqualAttribute"/> class.
        /// </summary>
        /// <param name="value"><inheritdoc cref="Value"/></param>
        public GreaterOrEqualAttribute(IComparable value)
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
            return Value.CompareTo(value) >= 0;
        }
    }
}