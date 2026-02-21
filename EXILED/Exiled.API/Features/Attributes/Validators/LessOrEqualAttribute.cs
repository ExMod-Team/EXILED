// -----------------------------------------------------------------------
// <copyright file="LessOrEqualAttribute.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes.Validators
{
    using System;

    using Exiled.API.Interfaces;

    /// <summary>
    /// Checks if value less or equal.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class LessOrEqualAttribute : Attribute, IValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LessOrEqualAttribute"/> class.
        /// </summary>
        /// <param name="value"><inheritdoc cref="Value"/></param>
        public LessOrEqualAttribute(IComparable value)
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
            return Value.CompareTo(value) <= 0;
        }
    }
}