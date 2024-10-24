// -----------------------------------------------------------------------
// <copyright file="Marshmallow.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using Exiled.API.Interfaces;
    using InventorySystem.Items.MarshmallowMan;

    /// <summary>
    /// A wrapper for <see cref="MarshmallowItem"/>.
    /// </summary>
    public class Marshmallow : Item, IWrapper<MarshmallowItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Marshmallow"/> class.
        /// </summary>
        /// <param name="itemBase">A <see cref="MarshmallowItem"/> instance.</param>
        public Marshmallow(MarshmallowItem itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Marshmallow"/> class.
        /// </summary>
        internal Marshmallow()
            : this((MarshmallowItem)Server.Host.Inventory.CreateItemInstance(new(ItemType.Marshmallow, 0), false))
        {
        }

        /// <inheritdoc/>
        public new MarshmallowItem Base { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the evil mode is enabled.
        /// </summary>
        public bool EvilMode
        {
            get => Base.EvilMode;
            set
            {
                Base.EvilMode = value;

                if (value)
                    ReleaseEvil();
            }
        }

        /// <summary>
        /// Gets or sets the attack cooldown.
        /// </summary>
        public float AttackCooldown
        {
            get => Base._attackCooldown;
            set => Base._attackCooldown = value;
        }

        /// <summary>
        /// Releases the evil mode.
        /// </summary>
        public void ReleaseEvil() => Base.ReleaseEvil();
    }
}