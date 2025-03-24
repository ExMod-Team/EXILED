// -----------------------------------------------------------------------
// <copyright file="MaxValueAttribute.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes.Config
{
    using System;
    using System.Collections;

    /// <summary>
    /// An attribute to check if a value is less than a specified value.
    /// </summary>
    public class MaxValueAttribute : CustomValidatorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaxValueAttribute"/> class.
        /// </summary>
        /// <param name="maxValue">Maximum value.</param>
        /// <param name="inclusive">Whether check should be inclusive or not.</param>
        public MaxValueAttribute(IComparable maxValue, bool inclusive = true)
            : base(x => maxValue.CompareTo(x) > (inclusive ? 0 : 1))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxValueAttribute"/> class.
        /// </summary>
        /// <param name="maxValue">Maximum value.</param>
        /// <param name="comparer">Comparer to use.</param>
        /// <param name="inclusive">Whether check should be inclusive or not.</param>
        public MaxValueAttribute(object maxValue, IComparer comparer, bool inclusive = true)
            : base(x => comparer.Compare(maxValue, x) > (inclusive ? -1 : 0))
        {
        }
    }
}