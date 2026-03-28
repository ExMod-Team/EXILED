// -----------------------------------------------------------------------
// <copyright file="CustomValidatorAttribute.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes.Validators
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Interfaces;

    /// <summary>
    /// Check a value with custom function.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class CustomValidatorAttribute : Attribute, IValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomValidatorAttribute"/> class.
        /// </summary>
        /// <param name="customFunctionType">The type of the custom check validator.</param>
        /// <remarks>
        /// The <see cref="Type"/> from customFunctionType must be a class inheriting IValidator with a parameterless constructor.
        /// </remarks>
        public CustomValidatorAttribute(Type customFunctionType)
        {
            if (!customFunctionType.IsClass || customFunctionType.IsAbstract || !customFunctionType.GetInterfaces().Contains(typeof(IValidator)))
                throw new ArgumentException($"{nameof(customFunctionType)} must be a type inheriting IValidator!");

            CustomFunctionType = customFunctionType;
        }

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey,TValue}"/> from a type inheriting <see cref="IValidator"/>, to an instance of that class.
        /// </summary>
        public static Dictionary<Type, IValidator> ValidatorInstances { get; } = new();

        /// <summary>
        /// Gets the type of the custom check validator.
        /// </summary>
        public Type CustomFunctionType { get; }

        /// <inheritdoc/>
        public bool Check(object other) => ValidatorInstances.GetOrAdd(CustomFunctionType, () => (IValidator)Activator.CreateInstance(CustomFunctionType)).Check(other);
    }
}