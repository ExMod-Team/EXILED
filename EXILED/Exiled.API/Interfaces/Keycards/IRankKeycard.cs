// -----------------------------------------------------------------------
// <copyright file="IRankKeycard.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Interfaces.Keycards
{
    using Exiled.API.Features.Items.Keycards;

    /// <summary>
    /// An interface for <see cref="CustomKeycard"/>'s with the <see cref="Rank"/> property.
    /// </summary>
    public interface IRankKeycard
    {
        /// <summary>
        /// Gets or sets the rank of this <see cref="CustomKeycard"/>.
        /// </summary>
        /// <remarks>Capped from 0-3.</remarks>
        public int Rank { get; set; }
    }
}