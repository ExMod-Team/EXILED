// -----------------------------------------------------------------------
// <copyright file="AvailableValuesAttribute.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes.Validators
{
    using System;

    using Exiled.API.Interfaces;

    /// <summary>
    /// Checks if value is in list of available values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AvailableValuesAttribute : Attribute, IValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AvailableValuesAttribute"/> class.
        /// </summary>
        /// <param name="values"><inheritdoc cref="Values"/></param>
        public AvailableValuesAttribute(params object[] values)
        {
            Values = values;
        }

        /// <summary>
        /// Gets the array of possible values.
        /// </summary>
        public object[] Values { get; }

        /// <inheritdoc/>
        public bool Check(object value) => Values.Contains(value);
    }
}