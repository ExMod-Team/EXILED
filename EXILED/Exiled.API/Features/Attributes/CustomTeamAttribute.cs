// -----------------------------------------------------------------------
// <copyright file="CustomTeamAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes
{
    using System;

    /// <summary>
    /// An attribute to easily manage custom teams initialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CustomTeamAttribute : Attribute
    {
    }
}