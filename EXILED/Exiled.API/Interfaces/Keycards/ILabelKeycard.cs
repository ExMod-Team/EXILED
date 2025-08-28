// -----------------------------------------------------------------------
// <copyright file="ILabelKeycard.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Interfaces.Keycards
{
    using Exiled.API.Features.Items.Keycards;
    using UnityEngine;

    /// <summary>
    /// An interface for <see cref="CustomKeycardItem"/>'s with the <see cref="Label"/> property.
    /// </summary>
    public interface ILabelKeycard
    {
        /// <summary>
        /// Gets or sets the label of this <see cref="CustomKeycardItem"/>.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Color"/> of this <see cref="CustomKeycardItem"/>.
        /// </summary>
        public Color LabelColor { get; set; }
    }
}