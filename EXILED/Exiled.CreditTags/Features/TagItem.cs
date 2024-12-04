// -----------------------------------------------------------------------
// <copyright file="TagItem.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CreditTags.Features
{
    using Serialization;

    /// <summary>
    /// Represents a tag item.
    /// </summary>
    public class TagItem
    {
        /// <summary>
        /// Gets or sets the SHA256 hashed user id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the type of rank based on <see cref="Enums.RankType"/>.
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// Gets all the tag items from a yaml string.
        /// </summary>
        /// <param name="yaml">The yaml string.</param>
        /// <returns>Returns an array of <see cref="TagItem"/>.</returns>
        public static TagItem[] FromYaml(string yaml) => YamlParser.Deserializer.Deserialize<TagItem[]>(yaml);
    }
}