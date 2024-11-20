// -----------------------------------------------------------------------
// <copyright file="Map.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Decals;
    using Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Hazards;
    using Exiled.API.Features.Lockers;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Toys;
    using global::Hazards;
    using InventorySystem;
    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.BasicMessages;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.ThrowableProjectiles;
    using Items;
    using LightContainmentZoneDecontamination;
    using MapGeneration;
    using MapGeneration.Distributors;
    using PlayerRoles.PlayableScps.Scp939;
    using PlayerRoles.Ragdolls;
    using RelativePositioning;
    using UnityEngine;
    using Utils;
    using Utils.Networking;

    using Object = UnityEngine.Object;

    /// <summary>
    /// A set of tools to easily handle the in-game map.
    /// </summary>
    public static class Map
    {
        /// <summary>
        /// A list of <see cref="PocketDimensionTeleport"/>s on the map.
        /// </summary>
        internal static readonly List<PocketDimensionTeleport> TeleportsValue = new(8);

        private static AmbientSoundPlayer ambientSoundPlayer;

        private static SqueakSpawner squeakSpawner;

        /// <summary>
        /// Gets the tantrum prefab.
        /// </summary>
        public static TantrumEnvironmentalHazard TantrumPrefab => TantrumHazard.TantrumPrefab; // TODO: Remove this.

        /// <summary>
        /// Gets the amnestic cloud prefab.
        /// </summary>
        public static Scp939AmnesticCloudInstance AmnesticCloudPrefab => AmnesticCloudHazard.AmnesticCloudPrefab; // TODO: Remove this.

        /// <summary>
        /// Gets a value indicating whether decontamination has begun in the light containment zone.
        /// </summary>
        public static bool IsLczDecontaminated => DecontaminationController.Singleton.IsDecontaminating;

        /// <summary>
        /// Gets a value indicating whether decontamination phase is in the light containment zone.
        /// </summary>
        public static DecontaminationState DecontaminationState =>
            DecontaminationController.Singleton.NetworkDecontaminationOverride is DecontaminationController.DecontaminationStatus.Disabled ?
            DecontaminationState.Disabled : (DecontaminationState)DecontaminationController.Singleton._nextPhase;

        /// <summary>
        /// Gets all <see cref="PocketDimensionTeleport"/> objects.
        /// </summary>
        public static ReadOnlyCollection<PocketDimensionTeleport> PocketDimensionTeleports { get; } = TeleportsValue.AsReadOnly();

        /// <summary>
        /// Gets all <see cref="MapGeneration.Distributors.Locker"/> objects in the current map.
        /// </summary>
        /// <remarks>
        /// This property is obsolete. Use <see cref="Lockers.Locker.List"/> instead to retrieve a collection of all <see cref="Locker"/> instances.
        /// </remarks>
        [Obsolete("Use Locker.List instead.")]
        public static ReadOnlyCollection<MapGeneration.Distributors.Locker> Lockers { get; } = Features.Lockers.Locker.BaseToExiledLockers.Keys.ToList().AsReadOnly();

        /// <summary>
        /// Gets all <see cref="AdminToy"/> objects.
        /// </summary>
        public static ReadOnlyCollection<AdminToy> Toys => AdminToy.BaseToAdminToy.Values.ToList().AsReadOnly(); // TODO: Obsolete it and make people use AdminToy.List

        /// <summary>
        /// Gets or sets the current seed of the map.
        /// </summary>
        public static int Seed
        {
            get => SeedSynchronizer.Seed;
            set
            {
                if (!SeedSynchronizer.MapGenerated)
                    SeedSynchronizer._singleton.Network_syncSeed = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether decontamination is enabled.
        /// </summary>
        public static bool IsDecontaminationEnabled
        {
            get => DecontaminationController.Singleton.NetworkDecontaminationOverride == DecontaminationController.DecontaminationStatus.None;
            set =>
                DecontaminationController.Singleton.NetworkDecontaminationOverride = value
                    ? DecontaminationController.DecontaminationStatus.None
                    : DecontaminationController.DecontaminationStatus.Disabled;
        }

        /// <summary>
        /// Gets the <see cref="global::AmbientSoundPlayer"/>.
        /// </summary>
        public static AmbientSoundPlayer AmbientSoundPlayer => ambientSoundPlayer ??= ReferenceHub.HostHub.GetComponent<AmbientSoundPlayer>();

        /// <summary>
        /// Gets the <see cref="global::SqueakSpawner"/>.
        /// </summary>
        public static SqueakSpawner SqueakSpawner => squeakSpawner ??= Object.FindObjectOfType<SqueakSpawner>();

        /// <summary>
        /// Broadcasts a message to all <see cref="Player">players</see>.
        /// </summary>
        /// <param name="broadcast">The <see cref="Features.Broadcast"/> to be broadcasted.</param>
        /// <param name="shouldClearPrevious">Clears all players' broadcasts before sending the new one.</param>
        public static void Broadcast(Broadcast broadcast, bool shouldClearPrevious = false)
        {
            if (broadcast.Show)
                Broadcast(broadcast.Duration, broadcast.Content, broadcast.Type, shouldClearPrevious);
        }

        /// <summary>
        /// Broadcasts a message to all <see cref="Player">players</see>.
        /// </summary>
        /// <param name="duration">The duration in seconds.</param>
        /// <param name="message">The message that will be broadcast (supports Unity Rich Text formatting).</param>
        /// <param name="type">The broadcast type.</param>
        /// <param name="shouldClearPrevious">Clears all players' broadcasts before sending the new one.</param>
        public static void Broadcast(ushort duration, string message, global::Broadcast.BroadcastFlags type = global::Broadcast.BroadcastFlags.Normal, bool shouldClearPrevious = false)
        {
            if (shouldClearPrevious)
                ClearBroadcasts();

            Server.Broadcast.RpcAddElement(message, duration, type);
        }

        /// <summary>
        /// Shows a hint to all <see cref="Player">players</see>.
        /// </summary>
        /// <param name="message">The message that will be broadcasted (supports Unity Rich Text formatting).</param>
        /// <param name="duration">The duration in seconds.</param>
        public static void ShowHint(string message, float duration = 3f)
        {
            foreach (Player player in Player.List)
                player.ShowHint(message, duration);
        }

        /// <summary>
        /// Clears all <see cref="Player">players</see>' broadcasts.
        /// </summary>
        public static void ClearBroadcasts() => Server.Broadcast.RpcClearElements();

        /// <summary>
        /// Forces the light containment zone decontamination process.
        /// </summary>
        public static void StartDecontamination() => DecontaminationController.Singleton.ForceDecontamination();

        /// <summary>
        /// Turns on all lights in the facility.
        /// </summary>
        /// <param name="zoneTypes">The <see cref="ZoneType"/>s to affect.</param>
        public static void TurnOnAllLights(IEnumerable<ZoneType> zoneTypes) => TurnOffAllLights(0, zoneTypes);

        /// <summary>
        /// Turns off all lights in the facility.
        /// </summary>
        /// <param name="duration">The duration of the blackout.</param>
        /// <param name="zoneTypes">The <see cref="ZoneType"/>s to affect.</param>
        public static void TurnOffAllLights(float duration, ZoneType zoneTypes = ZoneType.Unspecified)
        {
            foreach (RoomLightController controller in RoomLightController.Instances)
            {
                Room room = controller.GetComponentInParent<Room>();
                if (room == null)
                    continue;

                if (zoneTypes == ZoneType.Unspecified || room.Zone.HasFlag(zoneTypes))
                    controller.ServerFlickerLights(duration);
            }
        }

        /// <summary>
        /// Turns off all lights in the facility.
        /// </summary>
        /// <param name="duration">The duration of the blackout.</param>
        /// <param name="zoneTypes">The <see cref="ZoneType"/>s to affect.</param>
        public static void TurnOffAllLights(float duration, IEnumerable<ZoneType> zoneTypes)
        {
            foreach (ZoneType zone in zoneTypes)
                TurnOffAllLights(duration, zone);
        }

        /// <summary>
        /// Changes the <see cref="Color"/> of all lights in the facility.
        /// </summary>
        /// <param name="color">The new <see cref="Color"/> of the lights.</param>
        public static void ChangeLightsColor(Color color)
        {
            foreach (RoomLightController light in RoomLightController.Instances)
                light.NetworkOverrideColor = color;
        }

        /// <summary>
        /// Resets the <see cref="Color">color</see> of all lights in the facility.
        /// </summary>
        public static void ResetLightsColor()
        {
            foreach (RoomLightController light in RoomLightController.Instances)
                light.NetworkOverrideColor = Color.clear;
        }

        /// <summary>
        /// Gets a random <see cref="MapGeneration.Distributors.Locker"/> object from the current map.
        /// </summary>
        /// <remarks>
        /// This method is obsolete. Use <see cref="Features.Lockers.Locker.Random"/> instead to get a random <see cref="Locker"/> instance.
        /// </remarks>
        /// <returns>A randomly selected <see cref="MapGeneration.Distributors.Locker"/> object.</returns>
        [Obsolete("Use Locker.Random() instead.")]
        public static MapGeneration.Distributors.Locker GetRandomLocker() => Lockers.GetRandomValue();

        /// <summary>
        /// Gets a random <see cref="Pickup"/>.
        /// </summary>
        /// <param name="type">Filters by <see cref="ItemType"/>.</param>
        /// <returns><see cref="Pickup"/> object.</returns>
        public static Pickup GetRandomPickup(ItemType type = ItemType.None)
        {
            List<Pickup> pickups = (type != ItemType.None ? Pickup.List.Where(p => p.Type == type) : Pickup.List).ToList();
            return pickups.GetRandomValue();
        }

        /// <summary>
        /// Plays a random ambient sound.
        /// </summary>
        public static void PlayAmbientSound() => AmbientSoundPlayer.GenerateRandom();

        /// <summary>
        /// Plays an ambient sound.
        /// </summary>
        /// <param name="id">The id of the sound to play.</param>
        public static void PlayAmbientSound(int id)
        {
            if (id >= AmbientSoundPlayer.clips.Length)
                throw new IndexOutOfRangeException($"There are only {AmbientSoundPlayer.clips.Length} sounds available.");

            AmbientSoundPlayer.RpcPlaySound(AmbientSoundPlayer.clips[id].index);
        }

        /// <summary>
        /// Places a Tantrum (SCP-173's ability) in the indicated position.
        /// </summary>
        /// <param name="position">The position where you want to spawn the Tantrum.</param>
        /// <param name="isActive">Whether the tantrum will apply the <see cref="EffectType.Stained"/> effect.</param>
        /// <remarks>If <paramref name="isActive"/> is <see langword="true"/>, the tantrum is moved slightly up from its original position. Otherwise, the collision will not be detected and the slowness will not work.</remarks>
        /// <returns>The <see cref="TantrumHazard"/> instance.</returns>
        public static TantrumHazard PlaceTantrum(Vector3 position, bool isActive = true) => TantrumHazard.PlaceTantrum(position, isActive); // TODO: Remove this.

        /// <summary>
        /// Destroy all <see cref="ItemPickupBase"/> objects.
        /// </summary>
        public static void CleanAllItems()
        {
            foreach (Pickup pickup in Pickup.List.ToList())
                pickup.Destroy();
        }

        /// <summary>
        /// Destroy all the <see cref="Pickup"/> objects from the specified list.
        /// </summary>
        /// <param name="pickups">The List of pickups to destroy.</param>
        public static void CleanAllItems(IEnumerable<Pickup> pickups)
        {
            foreach (Pickup pickup in pickups)
                pickup.Destroy();
        }

        /// <summary>
        /// Destroy all <see cref="BasicRagdoll"/> objects.
        /// </summary>
        public static void CleanAllRagdolls()
        {
            foreach (Ragdoll ragDoll in Ragdoll.List.ToList())
                ragDoll.Destroy();
        }

        /// <summary>
        /// Destroy all <see cref="Ragdoll"/> objects from the specified list.
        /// </summary>
        /// <param name="ragDolls">The List of RagDolls to destroy.</param>
        public static void CleanAllRagdolls(IEnumerable<Ragdoll> ragDolls)
        {
            foreach (Ragdoll ragDoll in ragDolls)
                ragDoll.Destroy();
        }

        /// <summary>
        /// Places a blood decal.
        /// </summary>
        /// <param name="position">The position of the blood decal.</param>
        /// <param name="direction">The direction of the blood decal.</param>
        public static void PlaceBlood(Vector3 position, Vector3 direction) => new GunDecalMessage(position, direction, DecalPoolType.Blood).SendToAuthenticated(0);

        /// <summary>
        /// Gets all the near cameras.
        /// </summary>
        /// <param name="position">The position from which starting to search cameras.</param>
        /// <param name="toleration">The maximum toleration to define the radius from which get the cameras.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Camera"/> which contains all the found cameras.</returns>
        public static IEnumerable<Camera> GetNearCameras(Vector3 position, float toleration = 15f)
            => Camera.Get(cam => (position - cam.Position).sqrMagnitude <= toleration * toleration);

        /// <summary>
        /// Explode.
        /// </summary>
        /// <param name="position">The position where explosion will be created.</param>
        /// <param name="projectileType">The projectile that will create the explosion.</param>
        /// <param name="attacker">The player who create the explosion.</param>
        public static void Explode(Vector3 position, ProjectileType projectileType, Player attacker = null)
        {
            ItemType item;
            if ((item = projectileType.GetItemType()) is ItemType.None)
                return;
            attacker ??= Server.Host;
            if (!InventoryItemLoader.TryGetItem(item, out ThrowableItem throwableItem))
                return;

            if (Object.Instantiate(throwableItem.Projectile) is not TimeGrenade timedGrenadePickup)
                return;

            if (timedGrenadePickup is Scp018Projectile scp018Projectile)
                scp018Projectile.SetupModule();
            else
                ExplodeEffect(position, projectileType);

            timedGrenadePickup.Position = position;
            timedGrenadePickup.PreviousOwner = (attacker ?? Server.Host).Footprint;
            timedGrenadePickup.ServerFuseEnd();
        }

        /// <summary>
        /// Spawn projectile effect.
        /// </summary>
        /// <param name="position">The position where effect will be created.</param>
        /// <param name="projectileType">The projectile that will create the effect.</param>
        public static void ExplodeEffect(Vector3 position, ProjectileType projectileType)
        {
            ItemType item;
            if ((item = projectileType.GetItemType()) is ItemType.None)
                return;
            ExplosionUtils.ServerSpawnEffect(position, item);
        }

        /// <summary>
        /// Plays a gun sound at the specified position.
        /// </summary>
        /// <param name="position">Position to play the sound at.</param>
        /// <param name="firearmType">The type of firearm to play the sound of.</param>
        /// <param name="maxDistance">The maximum distance the sound can be heard from.</param>
        /// <param name="audioClipId">The audio clip ID to play.</param>
        public static void PlayGunSound(Vector3 position, ItemType firearmType, byte maxDistance = 45, byte audioClipId = 0)
        {
            GunAudioMessage msg = new()
            {
                Weapon = firearmType,
                AudioClipId = audioClipId,
                MaxDistance = maxDistance,
                ShooterHub = ReferenceHub.HostHub,
                ShooterPosition = new RelativePosition(position),
            };
            msg.SendToAuthenticated();
        }

        /// <summary>
        /// Spawns mice inside the <see cref="RoomType.EzShelter"/>.
        /// </summary>
        /// <param name="mice">The type of mice you want to spawn..</param>
        public static void SpawnMice(byte mice = 1)
        {
            if (mice > SqueakSpawner.mice.Length)
                throw new ArgumentOutOfRangeException($"Mouse type must be between 1 and {SqueakSpawner.mice.Length}.");

            SqueakSpawner.NetworksyncSpawn = mice;
            SqueakSpawner.SyncMouseSpawn(0, SqueakSpawner.NetworksyncSpawn);
        }

        /// <summary>
        /// Clears the lazy loading game object cache.
        /// </summary>
        internal static void ClearCache()
        {
            Item.BaseToItem.Clear();

            Ragdoll.BasicRagdollToRagdoll.Clear();

            Items.Firearm.ItemTypeToFirearmInstance.Clear();
            Items.Firearm.BaseCodesValue.Clear();
            Items.Firearm.AvailableAttachmentsValue.Clear();
        }
    }
}
