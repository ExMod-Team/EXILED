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
        /// <remarks>value must be able to convert to your target type via <see cref="Convert.ChangeType(object, Type)"/>.</remarks>
        public GreaterThanAttribute(object value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the minimum value.
        /// </summary>
        public object Value { get; }

        /// <inheritdoc/>
        public bool Check(object other) => Convert.ChangeType(Value, other.GetType()) is IComparable min && min.CompareTo(other) < 0;
    }
}