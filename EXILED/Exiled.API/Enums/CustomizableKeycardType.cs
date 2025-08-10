// -----------------------------------------------------------------------
// <copyright file="CustomizableKeycardType.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    ///  Customizable Keycard types present in the game.
    /// </summary>
    /// <seealso cref="Extensions.ItemExtensions.GetItemType(CustomizableKeycardType)"/>
    public enum CustomizableKeycardType
    {
        /// <summary>
        /// Not Customizable Keycard.
        /// </summary>
        None,

        /// <summary>
        /// Used by <see cref="ItemType.KeycardCustomManagement"/>.
        /// </summary>
        Vertical,

        /// <summary>
        /// Used by <see cref="ItemType.KeycardCustomMetalCase"/>.
        /// </summary>
        MetalFramed,

        /// <summary>
        /// Used by <see cref="ItemType.KeycardCustomTaskForce"/>.
        /// </summary>
        Perforated,

        /// <summary>
        /// Used by <see cref="ItemType.KeycardCustomSite02"/>.
        /// </summary>
        Standart,
    }
}
