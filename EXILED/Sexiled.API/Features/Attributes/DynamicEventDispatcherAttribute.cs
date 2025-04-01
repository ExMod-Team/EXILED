// -----------------------------------------------------------------------
// <copyright file="DynamicEventDispatcherAttribute.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Sexiled.API.Features.Attributes
{
    using System;

    using Sexiled.API.Features.DynamicEvents;

    /// <summary>
    /// An attribute to easily manage <see cref="DynamicEventDispatcher"/> initialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DynamicEventDispatcherAttribute : Attribute
    {
    }
}