// -----------------------------------------------------------------------
// <copyright file="ISerialNumberKeycard.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Interfaces.Keycards
{
    using Exiled.API.Features.Items.Keycards;

    /// <summary>
    /// An interface for <see cref="CustomKeycardItem"/>'s with the <see cref="SerialNumber"/> property.
    /// </summary>
    public interface ISerialNumberKeycard
    {
        /// <summary>
        /// Gets or sets the serial number of this <see cref="CustomKeycardItem"/>.
        /// </summary>
        /// <remarks>Can only hold 12 numbers. Non-numerical chars will be replaced with "-".</remarks>
        public string SerialNumber { get; set; }
    }
}