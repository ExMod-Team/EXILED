// -----------------------------------------------------------------------
// <copyright file="CustomItem{T}.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
#pragma warning disable SA1402 // File may only contain a single type
namespace Exiled.CustomItems.API.Features
{
    using YamlDotNet.Serialization;

    /// <summary>
    /// A generic base class for <see cref="CustomItem"/> that provides a typed singleton <see cref="Instance"/>.
    /// </summary>
    /// <typeparam name="T">The concrete <see cref="CustomItem"/> type.</typeparam>
    public abstract class CustomItem<T> : CustomItem
        where T : CustomItem<T>
    {
        /// <summary>
        /// Gets the singleton instance of this <see cref="CustomItem"/>.
        /// </summary>
        [YamlIgnore]
        public static T? Instance { get; private set; }

        /// <inheritdoc/>
        public override void Init()
        {
            base.Init();
            Instance = this as T;
        }

        /// <inheritdoc/>
        public override void Destroy()
        {
            Instance = null;
            base.Destroy();
        }
    }

    /// <summary>
    /// A generic base class for <see cref="CustomWeapon"/> that provides a typed singleton <see cref="Instance"/>.
    /// </summary>
    /// <typeparam name="T">The concrete <see cref="CustomWeapon"/> type.</typeparam>
    public abstract class CustomWeapon<T> : CustomWeapon
        where T : CustomWeapon<T>
    {
        /// <summary>
        /// Gets the singleton instance of this <see cref="CustomWeapon"/>.
        /// </summary>
        [YamlIgnore]
        public static T? Instance { get; private set; }

        /// <inheritdoc/>
        public override void Init()
        {
            base.Init();
            Instance = this as T;
        }

        /// <inheritdoc/>
        public override void Destroy()
        {
            Instance = null;
            base.Destroy();
        }
    }

    /// <summary>
    /// A generic base class for <see cref="CustomKeycard"/> that provides a typed singleton <see cref="Instance"/>.
    /// </summary>
    /// <typeparam name="T">The concrete <see cref="CustomKeycard"/> type.</typeparam>
    public abstract class CustomKeycard<T> : CustomKeycard
        where T : CustomKeycard<T>
    {
        /// <summary>
        /// Gets the singleton instance of this <see cref="CustomKeycard"/>.
        /// </summary>
        [YamlIgnore]
        public static T? Instance { get; private set; }

        /// <inheritdoc/>
        public override void Init()
        {
            base.Init();
            Instance = this as T;
        }

        /// <inheritdoc/>
        public override void Destroy()
        {
            Instance = null;
            base.Destroy();
        }
    }

    /// <summary>
    /// A generic base class for <see cref="CustomGrenade"/> that provides a typed singleton <see cref="Instance"/>.
    /// </summary>
    /// <typeparam name="T">The concrete <see cref="CustomGrenade"/> type.</typeparam>
    public abstract class CustomGrenade<T> : CustomGrenade
        where T : CustomGrenade<T>
    {
        /// <summary>
        /// Gets the singleton instance of this <see cref="CustomGrenade"/>.
        /// </summary>
        [YamlIgnore]
        public static T? Instance { get; private set; }

        /// <inheritdoc/>
        public override void Init()
        {
            base.Init();
            Instance = this as T;
        }

        /// <inheritdoc/>
        public override void Destroy()
        {
            Instance = null;
            base.Destroy();
        }
    }

    /// <summary>
    /// A generic base class for <see cref="CustomArmor"/> that provides a typed singleton <see cref="Instance"/>.
    /// </summary>
    /// <typeparam name="T">The concrete <see cref="CustomArmor"/> type.</typeparam>
    public abstract class CustomArmor<T> : CustomArmor
        where T : CustomArmor<T>
    {
        /// <summary>
        /// Gets the singleton instance of this <see cref="CustomArmor"/>.
        /// </summary>
        [YamlIgnore]
        public static T? Instance { get; private set; }

        /// <inheritdoc/>
        public override void Init()
        {
            base.Init();
            Instance = this as T;
        }

        /// <inheritdoc/>
        public override void Destroy()
        {
            Instance = null;
            base.Destroy();
        }
    }

    /// <summary>
    /// A generic base class for <see cref="CustomGoggles"/> that provides a typed singleton <see cref="Instance"/>.
    /// </summary>
    /// <typeparam name="T">The concrete <see cref="CustomGoggles"/> type.</typeparam>
    public abstract class CustomGoggles<T> : CustomGoggles

        where T : CustomGoggles<T>
    {
        /// <summary>
        /// Gets the singleton instance of this <see cref="CustomGoggles"/>.
        /// </summary>
        [YamlIgnore]
        public static T? Instance { get; private set; }

        /// <inheritdoc/>
        public override void Init()
        {
            base.Init();
            Instance = this as T;
        }

        /// <inheritdoc/>
        public override void Destroy()
        {
            Instance = null;
            base.Destroy();
        }
    }
}
#pragma warning restore SA1402 // File may only contain a single type