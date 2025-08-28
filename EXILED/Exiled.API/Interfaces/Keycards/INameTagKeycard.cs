// -----------------------------------------------------------------------
// <copyright file="INameTagKeycard.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Interfaces.Keycards
{
    using Exiled.API.Features.Items.Keycards;

    /// <summary>
    /// An interface for <see cref="CustomKeycardItem"/>'s with the <see cref="NameTag"/> property.
    /// </summary>
    public interface INameTagKeycard
    {
        /// <summary>
        /// Gets or sets the name of the owner of this <see cref="CustomKeycardItem"/>.
        /// </summary>
        public string NameTag { get; set; }
    }
}