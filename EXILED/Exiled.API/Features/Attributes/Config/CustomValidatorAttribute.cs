// -----------------------------------------------------------------------
// <copyright file="CustomValidatorAttribute.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes.Config
{
    using System;

    /// <summary>
    /// A custom validator for config values and base class for all config validators.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class CustomValidatorAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomValidatorAttribute"/> class.
        /// </summary>
        /// <param name="validator"><inheritdoc cref="Validator"/></param>
        public CustomValidatorAttribute(Func<object, bool> validator)
        {
            Validator = validator;
        }

        /// <summary>
        /// Gets a function that validates an object.
        /// </summary>
        public Func<object, bool> Validator { get; }

        /// <summary>
        /// Validates an object.
        /// </summary>
        /// <param name="obj">Object to validate.</param>
        /// <returns><c>true</c> if validation was successful, <c>false</c> otherwise.</returns>
        public bool Validate(object obj) => Validator(obj);
    }
}