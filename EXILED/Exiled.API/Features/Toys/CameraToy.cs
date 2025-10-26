// -----------------------------------------------------------------------
// <copyright file="CameraToy.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Toys
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using AdminToys;
    using Exiled.API.Enums;
    using Exiled.API.Interfaces;
    using UnityEngine;

    /// <summary>
    /// A wrapper class for <see cref="AdminToys.AdminToyBase"/>.
    /// </summary>
    public class CameraToy : AdminToy, IWrapper<Scp079CameraToy>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CameraToy"/> class.
        /// </summary>
        /// <param name="scp079CameraToy">The <see cref="Scp079CameraToy"/> of the toy.</param>
        internal CameraToy(Scp079CameraToy scp079CameraToy)
            : base(scp079CameraToy, AdminToyType.CameraToy) => Base = scp079CameraToy;

        /// <summary>
        /// Gets the base <see cref="Scp079CameraToy"/>.
        /// </summary>
        public Scp079CameraToy Base { get; }

        /// <summary>
        /// Gets or sets the Vertical Restriction.
        /// </summary>
        public Vector2 VerticalConstraint
        {
            get => Base.NetworkVerticalConstraint;
            set => Base.NetworkVerticalConstraint = value;
        }

        /// <summary>
        /// Gets or sets the Horizontal restriction.
        /// </summary>
        public Vector2 HorizontalConstraint
        {
            get => Base.NetworkHorizontalConstraint;
            set => Base.NetworkHorizontalConstraint = value;
        }

        /// <summary>
        /// Gets or sets the Zoom restriction.
        /// </summary>
        public Vector2 ZoomConstraint
        {
            get => Base.NetworkZoomConstraint;
            set => Base.NetworkZoomConstraint = value;
        }

        /// <summary>
        /// Gets or sets the Room where the Camera is associated with.
        /// </summary>
        public Room Room
        {
            get => Room.Get(Base.NetworkRoom);
            set => Base.NetworkRoom = value.Identifier;
        }

        /// <summary>
        /// Gets or sets the Name of the Camera.
        /// </summary>
        public string Name
        {
            get => Base.NetworkLabel;
            set => Base.NetworkLabel = value;
        }
    }
}
