// -----------------------------------------------------------------------
// <copyright file="OneOfAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes.Config
{
    using System;

    /// <summary>
    /// An attribute to check if a value is one of a possible values.
    /// </summary>
    public class OneOfAttribute : CustomValidatorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OneOfAttribute"/> class.
        /// </summary>
        /// <param name="values">Values.</param>
        public OneOfAttribute(params object[] values)
            : base(x => Array.IndexOf(values, x) != -1)
        {
        }
    }
}