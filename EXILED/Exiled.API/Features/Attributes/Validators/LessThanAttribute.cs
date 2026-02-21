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
        public LessThanAttribute(IComparable value)
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
            return Value.CompareTo(value) <= -1;
        }
    }
}