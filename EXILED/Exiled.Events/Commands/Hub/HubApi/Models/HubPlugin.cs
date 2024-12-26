// -----------------------------------------------------------------------
// <copyright file="HubPlugin.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.Hub.HubApi.Models
{
    using System.Runtime.Serialization;

    using Utf8Json;

    /// <summary>
    /// A struct containing all hub plugin data.
    /// </summary>
    public readonly struct HubPlugin : IJsonSerializable
    {
        /// <summary>
        /// The repository id.
        /// </summary>
        [DataMember(Name = "repositoryId")]
        public readonly long RepositoryId;

        /// <summary>
        /// Initializes a new instance of the <see cref="HubPlugin"/> struct.
        /// </summary>
        /// <param name="repositoryId"><inheritdoc cref="RepositoryId"/></param>
        [SerializationConstructor]
        public HubPlugin(long repositoryId)
        {
            RepositoryId = repositoryId;
        }
    }
}