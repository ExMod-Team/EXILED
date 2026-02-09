// -----------------------------------------------------------------------
// <copyright file="IStructureSync.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Interfaces
{
    using MapGeneration.Distributors;

    /// <summary>
    /// Represents an object with a <see cref="StructurePositionSync"/>.
    /// </summary>
    public interface IStructureSync
    {
        /// <summary>
        /// Gets the <see cref="StructurePositionSync"/> of this structure.
        /// </summary>
        public StructurePositionSync PositionSync { get; }
    }
}