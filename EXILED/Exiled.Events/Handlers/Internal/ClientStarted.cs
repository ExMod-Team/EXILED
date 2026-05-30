// -----------------------------------------------------------------------
// <copyright file="ClientStarted.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using AdminToys;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Toys;
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
            // required for logging
            try
            {
                PrefabHelper.Prefabs.Clear();

                Dictionary<uint, (GameObject, Component)> prefabs = new();

                Log.Debug(string.Empty);
                Log.Debug("//////// ALL PREFABS ////////");
                Log.Debug(string.Empty);

                foreach (KeyValuePair<uint, GameObject> prefab in NetworkClient.prefabs)
                {
                    if (!prefabs.ContainsKey(prefab.Key))
                    {
                        if (!prefab.Value.TryGetComponent(out NetworkBehaviour component))
                        {
                            Log.Error($"Failed to get component for prefab: {prefab.Value.name} ({prefab.Key})");
                            continue;
                        }

                        Log.Debug($"{prefab.Key}: {component.name}");

                        prefabs.Add(prefab.Key, (prefab.Value, component));
                    }
                }

                Log.Debug(string.Empty);

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

                HashSet<uint> found = new();

                for (int i = 0; i < EnumUtils<PrefabType>.Values.Length; i++)
                {
                    PrefabType prefabType = EnumUtils<PrefabType>.Values[i];

                    // skip obsolete prefabs
                    if (typeof(PrefabType).GetField(prefabType.ToString()).GetCustomAttribute<ObsoleteAttribute>() != null)
                        continue;

                    PrefabAttribute attribute = prefabType.GetPrefabAttribute();

                    if (prefabs.TryGetValue(attribute.AssetId, out (GameObject, Component) tuple))
                    {
                        if (attribute.Name != tuple.Item1.name)
                            Log.Warn($"Invalid Name {prefabType}: {attribute.Name} ({attribute.AssetId}) -> {tuple.Item1.name} ({attribute.AssetId})");

                        PrefabHelper.Prefabs.Add(prefabType, prefabs.FirstOrDefault(prefab => prefab.Key == attribute.AssetId || prefab.Value.Item1.name.Contains(attribute.Name)).Value);

                        if (!found.Add(attribute.AssetId))
                            Log.Debug($"Duplicate entry! {prefabType}: {attribute.Name} ({attribute.AssetId})");

                        continue;
                    }

                    if (TryGet(attribute.Name, out KeyValuePair<uint, (GameObject, Component)> value))
                    {
                        Log.Warn($"Invalid AssetId {prefabType}: {attribute.Name} ({attribute.AssetId}) -> {value.Value.Item1.name} ({value.Key})");

                        PrefabHelper.Prefabs.Add(prefabType, prefabs.FirstOrDefault(prefab => prefab.Key == attribute.AssetId || (prefab.Value.Item1?.name.Contains(attribute.Name) ?? false)).Value);

                        if (!found.Add(value.Key))
                            Log.Warn($"Duplicate entry! {prefabType}: {attribute.Name} ({attribute.AssetId})");

                        continue;
                    }

                    Log.Warn($"Useless prefab {prefabType}: {attribute.Name} ({attribute.AssetId})");
                }

                foreach (KeyValuePair<uint, (GameObject, Component)> missing in prefabs)
                {
                    if (found.Contains(missing.Key))
                        continue;

                    Log.Warn($"Missing prefab in {nameof(PrefabType)}: {missing.Value.Item1.name} ({missing.Key})");
                }

                // set all admin toy prefab caches and log any missing prefabs
                CameraToy.EzArmCameraPrefab = PrefabHelper.GetPrefab<Scp079CameraToy>(PrefabType.EzArmCameraToy);
                if (!CameraToy.EzArmCameraPrefab)
                    Log.Warn("EzArmCamera prefab could not be found!");

                CameraToy.EzCameraPrefab = PrefabHelper.GetPrefab<Scp079CameraToy>(PrefabType.EzCameraToy);
                if (!CameraToy.EzCameraPrefab)
                    Log.Warn("EzCamera prefab could not be found!");

                CameraToy.HczCameraPrefab = PrefabHelper.GetPrefab<Scp079CameraToy>(PrefabType.HczCameraToy);
                if (!CameraToy.HczCameraPrefab)
                    Log.Warn("HczCamera prefab could not be found!");

                CameraToy.LczCameraPrefab = PrefabHelper.GetPrefab<Scp079CameraToy>(PrefabType.LczCameraToy);
                if (!CameraToy.LczCameraPrefab)
                    Log.Warn("LczCamera prefab could not be found!");

                CameraToy.SzCameraPrefab = PrefabHelper.GetPrefab<Scp079CameraToy>(PrefabType.SzCameraToy);
                if (!CameraToy.SzCameraPrefab)
                    Log.Warn("SzCamera prefab could not be found!");

                Capybara.Prefab = PrefabHelper.GetPrefab<CapybaraToy>(PrefabType.CapybaraToy);
                if (!Capybara.Prefab)
                    Log.Warn("Capybara prefab could not be found!");

                InteractableToy.Prefab = PrefabHelper.GetPrefab<InvisibleInteractableToy>(PrefabType.InvisibleInteractableToy);
                if (!InteractableToy.Prefab)
                    Log.Warn("InteractableToy prefab could not be found!");

                API.Features.Toys.Light.Prefab = PrefabHelper.GetPrefab<LightSourceToy>(PrefabType.LightSourceToy);
                if (!API.Features.Toys.Light.Prefab)
                    Log.Warn("Light prefab could not be found!");

                Primitive.Prefab = PrefabHelper.GetPrefab<PrimitiveObjectToy>(PrefabType.PrimitiveObjectToy);
                if (!Primitive.Prefab)
                    Log.Warn("Primitive prefab could not be found!");

                ShootingTargetToy.SportShootingTargetPrefab = PrefabHelper.GetPrefab<ShootingTarget>(PrefabType.SportTarget);
                if (!ShootingTargetToy.SportShootingTargetPrefab)
                    Log.Warn("SportShootingTarget prefab could not be found!");

                ShootingTargetToy.DboyShootingTargetPrefab = PrefabHelper.GetPrefab<ShootingTarget>(PrefabType.DBoyTarget);
                if (!ShootingTargetToy.DboyShootingTargetPrefab)
                    Log.Warn("DboyShootingTarget prefab could not be found!");

                ShootingTargetToy.BinaryShootingTargetPrefab = PrefabHelper.GetPrefab<ShootingTarget>(PrefabType.BinaryTarget);
                if (!ShootingTargetToy.BinaryShootingTargetPrefab)
                    Log.Warn("BinaryShootingTarget prefab could not be found!");

                Speaker.Prefab = PrefabHelper.GetPrefab<SpeakerToy>(PrefabType.SpeakerToy);
                if (!Speaker.Prefab)
                    Log.Warn("Speaker prefab could not be found!");

                Text.Prefab = PrefabHelper.GetPrefab<TextToy>(PrefabType.TextToy);
                if (!Text.Prefab)
                    Log.Warn("Text prefab could not be found!");

                Waypoint.Prefab = PrefabHelper.GetPrefab<WaypointToy>(PrefabType.WaypointToy);
                if (!Waypoint.Prefab)
                    Log.Warn("Waypoint prefab could not be found!");

                bool TryGet(string name, out KeyValuePair<uint, (GameObject, Component)> tuple)
                {
                    foreach (KeyValuePair<uint, (GameObject, Component)> kvp in prefabs.Where(kvp => kvp.Value.Item1.name == name))
                    {
                        tuple = kvp;
                        return true;
                    }

                    tuple = default;
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}