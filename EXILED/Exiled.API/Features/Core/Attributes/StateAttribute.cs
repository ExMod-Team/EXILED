// -----------------------------------------------------------------------
// <copyright file="StateAttribute.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.Attributes
{
    using System;

    /// <summary>
    /// An attribute to easily initialize states.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class StateAttribute : Attribute
    {
    }
}