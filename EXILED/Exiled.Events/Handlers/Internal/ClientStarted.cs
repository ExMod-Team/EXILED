// -----------------------------------------------------------------------
// <copyright file="ClientStarted.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.Internal
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Mirror;
    using PlayerRoles.Ragdolls;
    using UnityEngine;

    /// <summary>
    /// Handles on client started event.
    /// </summary>
    internal static class ClientStarted
    {
        /// <summary>
        /// Called once when the client is started.
        /// </summary>
        public static void OnClientStarted()
        {
            PrefabHelper.Prefabs.Clear();

            Dictionary<uint, GameObject> prefabs = new();

            foreach (KeyValuePair<uint, GameObject> prefab in NetworkClient.prefabs)
            {
                if (!prefabs.ContainsKey(prefab.Key))
                    prefabs.Add(prefab.Key, prefab.Value);
            }

            foreach (NetworkIdentity ragdollPrefab in RagdollManager.AllRagdollPrefabs)
            {
                if (!prefabs.ContainsKey(ragdollPrefab.assetId))
                    prefabs.Add(ragdollPrefab.assetId, ragdollPrefab.gameObject);
            }

            for (int i = 0; i < EnumUtils<PrefabType>.Values.Length; i++)
            {
                PrefabType prefabType = EnumUtils<PrefabType>.Values[i];
                PrefabAttribute attribute = prefabType.GetPrefabAttribute();
                if (prefabs.TryGetValue(attribute.AssetId, out GameObject gameObject))
                {
                    PrefabHelper.Prefabs.Add(prefabType, gameObject);
                    prefabs.Remove(attribute.AssetId);
                    continue;
                }

                KeyValuePair<uint, GameObject>? value = prefabs.FirstOrDefault(x => x.Value.name == attribute.Name);
                if (value.HasValue)
                {
                    PrefabHelper.Prefabs.Add(prefabType, gameObject);
                    prefabs.Remove(value.Value.Key);
                    continue;
                }
            }

            foreach (KeyValuePair<uint, GameObject> missing in prefabs)
                Log.Warn($"Missing prefab in {nameof(PrefabType)}: {missing.Value.name} ({missing.Key})");
        }
    }
}