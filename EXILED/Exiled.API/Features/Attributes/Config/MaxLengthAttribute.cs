// -----------------------------------------------------------------------
// <copyright file="MaxLengthAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes.Config
{
    using System.Collections;
    using System.Linq;

    /// <summary>
    /// An attribute that checks if length of a sequence is less than a specified value.
    /// </summary>
    public class MaxLengthAttribute : CustomValidatorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaxLengthAttribute"/> class.
        /// </summary>
        /// <param name="length">Maximum length of sequence.</param>
        /// <param name="inclusive">Whether check is inclusive or not.</param>
        public MaxLengthAttribute(int length, bool inclusive = false)
            : base(x => x is IEnumerable enumerable && enumerable.Cast<object>().Count() < (inclusive ? length + 1 : length))
        {
        }
    }
}