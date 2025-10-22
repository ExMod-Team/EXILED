// -----------------------------------------------------------------------
// <copyright file="RangeAttribute.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes.Validators
{
    using System;

    using Exiled.API.Interfaces;

    /// <summary>
    /// Check if an <see cref="IComparable"/> is inside a specific range.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RangeAttribute : Attribute, IValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RangeAttribute"/> class.
        /// </summary>
        /// <param name="min"><inheritdoc cref="Min"/></param>
        /// <param name="max"><inheritdoc cref="Max"/></param>
        /// <param name="inclusive"><inheritdoc cref="Inclusive"/></param>
        public RangeAttribute(IComparable min, IComparable max, bool inclusive = false)
        {
            Min = min;
            Max = max;
            Inclusive = inclusive;
        }

        /// <summary>
        /// Gets the minimum value.
        /// </summary>
        public IComparable Max { get; }

        /// <summary>
        /// Gets the maximum value.
        /// </summary>
        public IComparable Min { get; }

        /// <summary>
        /// Gets a value indicating whether check is inclusive.
        /// </summary>
        public bool Inclusive { get; }

        /// <inheritdoc/>
        public bool Check(object value)
        {
            int minResult = Inclusive ? 0 : -1;
            int maxResult = Inclusive ? 0 : 1;

            return Max.CompareTo(value) <= minResult && Min.CompareTo(value) >= maxResult;
        }
    }
}