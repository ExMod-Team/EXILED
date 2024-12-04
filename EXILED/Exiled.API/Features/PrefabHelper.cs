// -----------------------------------------------------------------------
// <copyright file="PrefabHelper.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Exiled.API.Enums;
    using Exiled.API.Features.Attributes;
    using Mirror;
    using UnityEngine;

    /// <summary>
    /// Helper for Prefabs.
    /// </summary>
    public static class PrefabHelper
    {
        /// <summary>
        /// A <see cref="Dictionary{TKey,TValue}"/> containing all <see cref="PrefabType"/> and their corresponding <see cref="GameObject"/>.
        /// </summary>
        internal static readonly Dictionary<PrefabType, GameObject> Prefabs = new(Enum.GetValues(typeof(PrefabType)).Length);

        /// <summary>
        /// Gets a <see cref="IReadOnlyDictionary{TKey,TValue}"/> of <see cref="PrefabType"/> and their corresponding <see cref="GameObject"/>.
        /// </summary>
        public static IReadOnlyDictionary<PrefabType, GameObject> PrefabToGameObject => Prefabs;

        /// <summary>
        /// Gets the <see cref="PrefabAttribute"/> from a <see cref="PrefabType"/>.
        /// </summary>
        /// <param name="prefabType">The <see cref="PrefabType"/>.</param>
        /// <returns>The corresponding <see cref="PrefabAttribute"/>.</returns>
        public static PrefabAttribute GetPrefabAttribute(this PrefabType prefabType)
        {
            Type type = prefabType.GetType();
            return type.GetField(Enum.GetName(type, prefabType)).GetCustomAttribute<PrefabAttribute>();
        }

        /// <summary>
        /// Gets the <see cref="GameObject"/> of the specified <see cref="PrefabType"/>.
        /// </summary>
        /// <param name="prefabType">The <see cref="PrefabType"/>.</param>
        /// <returns>Returns the <see cref="GameObject"/>.</returns>
        public static GameObject GetPrefab(PrefabType prefabType)
        {
            if (Prefabs.TryGetValue(prefabType, out GameObject prefab))
                return prefab;

            return null;
        }

        /// <summary>
        /// Tries to get the <see cref="GameObject"/> of the specified <see cref="PrefabType"/>.
        /// </summary>
        /// <param name="prefabType">The <see cref="PrefabType"/>.</param>
        /// <param name="gameObject">The <see cref="GameObject"/> of the .</param>
        /// <returns>Returns true if the <see cref="GameObject"/> was found.</returns>
        public static bool TryGetPrefab(PrefabType prefabType, out GameObject gameObject)
        {
            gameObject = GetPrefab(prefabType);
            return gameObject is not null;
        }

        /// <summary>
        /// Gets a <see cref="Component"/> from the <see cref="GameObject"/> of the specified <see cref="PrefabType"/>.
        /// </summary>
        /// <param name="prefabType">The <see cref="PrefabType"/>.</param>
        /// <typeparam name="T">The <see cref="Component"/> type.</typeparam>
        /// <returns>Returns the <see cref="Component"/>.</returns>
        public static T GetPrefab<T>(PrefabType prefabType)
            where T : Component
        {
            if (Prefabs.TryGetValue(prefabType, out GameObject prefab) && prefab.TryGetComponent(out T component))
                return component;

            return null;
        }

        /// <summary>
        /// Spawns the <see cref="GameObject"/> of the specified <see cref="PrefabType"/>.
        /// </summary>
        /// <param name="prefabType">The <see cref="PrefabType"/>.</param>
        /// <param name="position">The <see cref="Vector3"/> position where the <see cref="GameObject"/> will spawn.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> rotation of the <see cref="GameObject"/>.</param>
        /// <returns>Returns the <see cref="GameObject"/> instantied.</returns>
        public static GameObject Spawn(PrefabType prefabType, Vector3 position = default, Quaternion rotation = default)
        {
            if (!TryGetPrefab(prefabType, out GameObject gameObject))
                return null;

            GameObject newGameObject = UnityEngine.Object.Instantiate(gameObject, position, rotation);
            NetworkServer.Spawn(newGameObject);
            return newGameObject;
        }

        /// <summary>
        /// Spawns the <see cref="GameObject"/> of the specified <see cref="PrefabType"/>.
        /// </summary>
        /// <param name="prefabType">The <see cref="PrefabType"/>.</param>
        /// <param name="position">The <see cref="Vector3"/> position where the <see cref="GameObject"/> will spawn.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> rotation of the <see cref="GameObject"/>.</param>
        /// <typeparam name="T">The <see cref="Component"/> type.</typeparam>
        /// <returns>Returns the <see cref="Component"/> of the <see cref="GameObject"/>.</returns>
        public static T Spawn<T>(PrefabType prefabType, Vector3 position = default, Quaternion rotation = default)
            where T : Component
        {
            GameObject gameObject = Spawn(prefabType, position, rotation);
            return gameObject?.GetComponent<T>();
        }
    }
}