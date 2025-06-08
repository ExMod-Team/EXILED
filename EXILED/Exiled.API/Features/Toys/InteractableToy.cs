// -----------------------------------------------------------------------
// <copyright file="InteractableToy.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Toys
{
    using AdminToys;
    using Exiled.API.Enums;
    using Exiled.API.Interfaces;
    using UnityEngine;

    using static AdminToys.InvisibleInteractableToy;

    /// <summary>
    /// A wrapper class for <see cref="InvisibleInteractableToy"/>.
    /// </summary>
    internal class InteractableToy : AdminToy, IWrapper<InvisibleInteractableToy>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InteractableToy"/> class.
        /// </summary>
        /// <param name="invisibleInteractableToy">The <see cref="InvisibleInteractableToy"/> of the toy.</param>
        internal InteractableToy(InvisibleInteractableToy invisibleInteractableToy)
            : base(invisibleInteractableToy, AdminToyType.InvisibleInteractableToy) => Base = invisibleInteractableToy;

        /// <summary>
        /// Gets the base <see cref="InvisibleInteractableToy"/>.
        /// </summary>
        public InvisibleInteractableToy Base { get; }

        /// <summary>
        /// Gets or sets the Shape of the Interactable.
        /// </summary>
        public ColliderShape Shape
        {
            get => Base.NetworkShape;
            set => Base.NetworkShape = value;
        }

        /// <summary>
        /// Gets or sets the time to interact with the Interactable.
        /// </summary>
        public float InteractionDuration
        {
            get => Base.NetworkInteractionDuration;
            set => Base.NetworkInteractionDuration = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the interactable is locked.
        /// </summary>
        public bool IsLocked
        {
            get => Base.NetworkIsLocked;
            set => Base.NetworkIsLocked = value;
        }
    }
}
