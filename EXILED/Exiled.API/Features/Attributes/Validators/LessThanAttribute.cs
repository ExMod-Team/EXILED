// -----------------------------------------------------------------------
// <copyright file="LessThanAttribute.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes.Validators
{
    using System;

    using Exiled.API.Interfaces;

    /// <summary>
    /// Checks if value is less.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class LessThanAttribute : Attribute, IValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LessThanAttribute"/> class.
        /// </summary>
        /// <param name="value"><inheritdoc cref="Value"/></param>
        /// <remarks>value must be able to convert to your target type via <see cref="Convert.ChangeType(object, Type)"/>.</remarks>
        public LessThanAttribute(object value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the maximum value.
        /// </summary>
        public object Value { get; }

        /// <inheritdoc/>
        public bool Check(object other) => Convert.ChangeType(Value, other.GetType()) is IComparable max && max.CompareTo(other) > 0;
    }
}