// -----------------------------------------------------------------------
// <copyright file="CustomItemAttribute.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes
{
    using System;

    using InventorySystem.Items;

    /// <summary>
    /// An attribute to easily manage CustomItem initialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class CustomItemAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomItemAttribute"/> class.
        /// </summary>
        /// <param name="type">The <see cref="InventorySystem.Items.ItemType"/> to serialize.</param>
        public CustomItemAttribute(ItemType type)
        {
            ItemType = type;
        }

        /// <summary>
        /// Gets the attribute's <see cref="InventorySystem.Items.ItemType"/>.
        /// </summary>
        public ItemType ItemType { get; }
    }
}