// -----------------------------------------------------------------------
// <copyright file="MinValueAttribute.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes.Config
{
    using System;
    using System.Collections;

    /// <summary>
    /// An attribute to check if value is greater than or equal to min value.
    /// </summary>
    public class MinValueAttribute : CustomValidatorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MinValueAttribute"/> class.
        /// </summary>
        /// <param name="minValue">Minimum value.</param>
        /// <param name="inclusive">Whether check should be inclusive or not.</param>
        public MinValueAttribute(IComparable minValue, bool inclusive = true)
            : base(x => minValue.CompareTo(x) > (inclusive ? -1 : 0))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MinValueAttribute"/> class.
        /// </summary>
        /// <param name="minValue">Minimum value.</param>
        /// <param name="comparer">Comparer for value.</param>
        /// <param name="inclusive">Whether check should be inclusive or not.</param>
        public MinValueAttribute(object minValue, IComparer comparer, bool inclusive = true)
            : base(x => comparer.Compare(x, minValue) > (inclusive ? -1 : 0))
        {
        }
    }
}