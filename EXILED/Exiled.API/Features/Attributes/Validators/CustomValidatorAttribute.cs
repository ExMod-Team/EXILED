// -----------------------------------------------------------------------
// <copyright file="CustomValidatorAttribute.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes.Validators
{
    using System;

    using Exiled.API.Interfaces;

    /// <summary>
    /// Check a value with custom function.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomValidatorAttribute : Attribute, IValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomValidatorAttribute"/> class.
        /// </summary>
        /// <param name="customFunction"><inheritdoc cref="CustomFunction"/></param>
        public CustomValidatorAttribute(Func<object, bool> customFunction)
        {
            CustomFunction = customFunction;
        }

        /// <summary>
        /// Gets the custom check function.
        /// </summary>
        public Func<object, bool> CustomFunction { get; }

        /// <inheritdoc/>
        public bool Check(object value) => CustomFunction(value);
    }
}