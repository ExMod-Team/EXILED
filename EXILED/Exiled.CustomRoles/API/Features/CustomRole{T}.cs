// -----------------------------------------------------------------------
// <copyright file="CustomRole{T}.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.API.Features
{
    using YamlDotNet.Serialization;

    /// <summary>
    /// A generic base class for <see cref="CustomRole"/> that provides a typed singleton <see cref="Instance"/>.
    /// </summary>
    /// <typeparam name="T">The concrete <see cref="CustomRole"/> type.</typeparam>
    public abstract class CustomRole<T> : CustomRole
        where T : CustomRole<T>, new()
    {
        /// <summary>
        /// Gets the singleton instance of this <see cref="CustomRole"/>.
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