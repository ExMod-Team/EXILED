// -----------------------------------------------------------------------
// <copyright file="Capybara.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Toys
{
    using AdminToys;
    using Enums;
    using Exiled.API.Interfaces;

    /// <summary>
    /// A wrapper class for <see cref="CapybaraToy"/>.
    /// </summary>
    public class Capybara : AdminToy, IWrapper<CapybaraToy>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Capybara"/> class.
        /// </summary>
        /// <param name="capybaraToy">The <see cref="CapybaraToy"/> of the toy.</param>
        internal Capybara(CapybaraToy capybaraToy)
            : base(capybaraToy, AdminToyType.Capybara) => Base = capybaraToy;

        /// <summary>
        /// Gets the prefab.
        /// </summary>
        public static CapybaraToy Prefab => PrefabHelper.GetPrefab<CapybaraToy>(PrefabType.CapybaraToy);

        /// <summary>
        /// Gets the base <see cref="CapybaraToy"/>.
        /// </summary>
        public CapybaraToy Base { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the capybara can be collided with.
        /// </summary>
        public bool Collidable
        {
            get => Base.Network_collisionsEnabled;
            set => Base.Network_collisionsEnabled = value;
        }
    }
}
