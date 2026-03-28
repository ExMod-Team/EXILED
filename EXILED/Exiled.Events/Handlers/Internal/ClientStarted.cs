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

            Dictionary<uint, (GameObject, Component)> prefabs = new();

            foreach (KeyValuePair<uint, GameObject> prefab in NetworkClient.prefabs)
            {
                if (!prefabs.ContainsKey(prefab.Key))
                {
                    if (!prefab.Value.TryGetComponent(out NetworkBehaviour component))
                    {
                        Log.Error($"Failed to get component for prefab: {prefab.Value.name} ({prefab.Key})");
                        continue;
                    }

                    prefabs.Add(prefab.Key, (prefab.Value, component));
                }
            }

            foreach (NetworkIdentity ragdollPrefab in RagdollManager.AllRagdollPrefabs)
            {
                if (!prefabs.ContainsKey(ragdollPrefab.assetId))
                {
                    if (!ragdollPrefab.TryGetComponent(out BasicRagdoll component))
                    {
                        Log.Error($"Failed to get component for ragdoll prefab: {ragdollPrefab.name}");
                        continue;
                    }

                    prefabs.Add(ragdollPrefab.assetId, (ragdollPrefab.gameObject, component));
                }
            }

            for (int i = 0; i < EnumUtils<PrefabType>.Values.Length; i++)
            {
                PrefabType prefabType = EnumUtils<PrefabType>.Values[i];
                PrefabAttribute attribute = prefabType.GetPrefabAttribute();
                if (prefabs.TryGetValue(attribute.AssetId, out (GameObject, Component) tuple))
                {
                    GameObject gameObject = tuple.Item1;
                    PrefabHelper.Prefabs.Add(prefabType, prefabs.FirstOrDefault(prefab => prefab.Key == attribute.AssetId || prefab.Value.Item1.name.Contains(attribute.Name)).Value);
                    prefabs.Remove(attribute.AssetId);
                    continue;
                }

                KeyValuePair<uint, (GameObject, Component)>? value = prefabs.FirstOrDefault(x => x.Value.Item1.name == attribute.Name);
                if (value.HasValue)
                {
                    PrefabHelper.Prefabs.Add(prefabType, prefabs.FirstOrDefault(prefab => prefab.Key == attribute.AssetId || prefab.Value.Item1.name.Contains(attribute.Name)).Value);
                    prefabs.Remove(value.Value.Key);
                    continue;
                }

                Log.Warn($"Useless prefab {prefabType}: {attribute.Name} ({attribute.AssetId})");
            }

            foreach (KeyValuePair<uint, (GameObject, Component)> missing in prefabs)
                Log.Warn($"Missing prefab in {nameof(PrefabType)}: {missing.Value.Item1.name} ({missing.Key})");
        }
    }
}