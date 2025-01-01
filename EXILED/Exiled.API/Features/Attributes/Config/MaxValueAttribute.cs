// -----------------------------------------------------------------------
// <copyright file="MaxValueAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes.Config
{
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
        public MaxValueAttribute(object maxValue, bool inclusive = true)
            : base(x => Comparer.Default.Compare(maxValue, x) > (inclusive ? -1 : 0))
        {
        }
    }
}