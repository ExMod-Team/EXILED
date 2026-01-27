// -----------------------------------------------------------------------
// <copyright file="IWearKeycard.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Interfaces.Keycards
{
    using Exiled.API.Features.Items.Keycards;

    /// <summary>
    /// An interface for <see cref="CustomKeycardItem"/>'s with the <see cref="Wear"/> property.
    /// </summary>
    public interface IWearKeycard
    {
        /// <summary>
        /// Gets or sets the wear on this <see cref="CustomKeycardItem"/>.
        /// </summary>
        public byte Wear { get; set; }
    }
}