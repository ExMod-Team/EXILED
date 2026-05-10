// -----------------------------------------------------------------------
// <copyright file="ValidateChildrenAttribute.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes
{
    using System;

    /// <summary>
    /// Checks all properties in the target object for validators.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidateChildrenAttribute : Attribute
    {
    }
}