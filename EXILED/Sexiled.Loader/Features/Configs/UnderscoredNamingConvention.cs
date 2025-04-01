// -----------------------------------------------------------------------
// <copyright file="UnderscoredNamingConvention.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Sexiled.Loader.Features.Configs
{
    using System.Collections.Generic;

    using Sexiled.API.Extensions;
    using YamlDotNet.Serialization;

    /// <inheritdoc cref="YamlDotNet.Serialization.NamingConventions.UnderscoredNamingConvention"/>
    public class UnderscoredNamingConvention : INamingConvention
    {
        /// <inheritdoc cref="YamlDotNet.Serialization.NamingConventions.UnderscoredNamingConvention.Instance"/>
        public static UnderscoredNamingConvention Instance { get; } = new();

        /// <summary>
        /// Gets the list.
        /// </summary>
        public List<object> Properties { get; } = new();

        /// <inheritdoc/>
        public string Apply(string value)
        {
            string newValue = value.ToSnakeCase();
            Properties.Add(newValue);
            return newValue;
        }
    }
}