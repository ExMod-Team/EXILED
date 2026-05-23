// -----------------------------------------------------------------------
// <copyright file="WearableElementType.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using System;

    /// <summary>
    /// An enum containing all types of Wearable elements.
    /// </summary>
    [Flags]
    public enum WearableElementType
    {
        /// <summary>
        /// No wearable elements.
        /// </summary>
        None = 0,

        /// <summary>
        /// SCP-268 wearable element.
        /// </summary>
        Scp268Hat = 1,

        /// <summary>
        /// SCP-1344 wearable element.
        /// </summary>
        Scp1344Goggles = 2,

        /// <summary>
        /// Armor wearable element.
        /// <remarks>if armor is not specified it's will choose the one from Inventory</remarks>
        /// </summary>
        ArmorDefault = 4,

        /// <summary>
        /// Force the Light armor wearable element.
        /// </summary>
        ArmorLight = ArmorDefault | 8,

        /// <summary>
        /// Force the Combat armor wearable element.
        /// </summary>
        ArmorCombat = ArmorDefault | 16,

        /// <summary>
        /// Force the Heavy armor wearable element.
        /// </summary>
        ArmorHeavy = ArmorDefault | 32,
    }
}