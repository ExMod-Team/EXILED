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
                if(!prefabs.ContainsKey(prefab.Key))
                    prefabs.Add(prefab.Key, prefab.Value);
            }

            foreach (NetworkIdentity ragdollPrefab in RagdollManager.AllRagdollPrefabs)
            {
                if(!prefabs.ContainsKey(ragdollPrefab.assetId))
                    prefabs.Add(ragdollPrefab.assetId, ragdollPrefab.gameObject);
            }

            foreach (PrefabType prefabType in EnumUtils<PrefabType>.Values)
            {
                PrefabAttribute attribute = prefabType.GetPrefabAttribute();
                PrefabHelper.Prefabs.Add(prefabType, prefabs.FirstOrDefault(prefab => prefab.Key == attribute.AssetId || prefab.Value.name.Contains(attribute.Name)).Value);
            }
        }
    }
}