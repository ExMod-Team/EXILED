// -----------------------------------------------------------------------
// <copyright file="Map.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
#pragma warning disable SA1401

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using CommandSystem.Commands.RemoteAdmin.Cleanup;
    using Decals;
    using Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Hazards;
    using Exiled.API.Features.Items.Keycards;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Toys;
    using InventorySystem;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.ThrowableProjectiles;
    using Items;
    using LightContainmentZoneDecontamination;
    using MapGeneration;
    using PlayerRoles.Ragdolls;
    using RemoteAdmin;
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
        /// Gets a list of <see cref="PocketDimensionTeleport"/>s on the map.
        /// </summary>
        internal static List<PocketDimensionTeleport> TeleportsValue = new();

        private static AmbientSoundPlayer ambientSoundPlayer;

        private static SqueakSpawner squeakSpawner;

        /// <summary>
        /// Gets a value indicating whether decontamination has begun in the light containment zone.
        /// </summary>
        public static bool IsLczDecontaminated => DecontaminationController.Singleton.IsDecontaminating;

        /// <summary>
        /// Gets a value indicating whether decontamination phase is in the light containment zone.
        /// </summary>
        public static DecontaminationState DecontaminationState =>
            DecontaminationController.Singleton.DecontaminationOverride is DecontaminationController.DecontaminationStatus.Disabled ?
            DecontaminationState.Disabled : (DecontaminationState)DecontaminationController.Singleton._nextPhase;

        /// <summary>
        /// Gets the remaining time for the decontamination process.
        /// </summary>
        /// <returns>
        /// The remaining time in seconds for the decontamination process.
        /// </returns>
        public static float RemainingDecontaminationTime => Mathf.Max(0, (float)(DecontaminationController.Singleton.DecontaminationPhases[DecontaminationController.Singleton.DecontaminationPhases.Length - 1].TimeTrigger - DecontaminationController.GetServerTime));

        /// <summary>
        /// Gets all <see cref="PocketDimensionTeleport"/> objects.
        /// </summary>
        public static ReadOnlyCollection<PocketDimensionTeleport> PocketDimensionTeleports { get; } = TeleportsValue.AsReadOnly();

        /// <summary>
        /// Gets or sets the current seed of the map.
        /// </summary>
        public static int Seed
        {
            get => SeedSynchronizer.Seed;
            set
            {
                if (!SeedSynchronizer.MapGenerated)
                    SeedSynchronizer.Seed = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether decontamination is enabled.
        /// </summary>
        public static bool IsDecontaminationEnabled
        {
            get => DecontaminationController.Singleton.DecontaminationOverride == DecontaminationController.DecontaminationStatus.None;
            set =>
                DecontaminationController.Singleton.DecontaminationOverride = value
                    ? DecontaminationController.DecontaminationStatus.None
                    : DecontaminationController.DecontaminationStatus.Disabled;
        }

        /// <summary>
        /// Gets the <see cref="Escape.EscapeZones"/> that will trigger Escape for player.
        /// </summary>
        public static List<Bounds> EscapeZones => Escape.EscapeZones;

        /// <summary>
        /// Gets the <see cref="global::AmbientSoundPlayer"/>.
        /// </summary>
        public static AmbientSoundPlayer AmbientSoundPlayer => ambientSoundPlayer ??= ReferenceHub._hostHub.GetComponent<AmbientSoundPlayer>();

        /// <summary>
        /// Gets the <see cref="global::SqueakSpawner"/>.
        /// </summary>
        public static SqueakSpawner SqueakSpawner => squeakSpawner ??= Object.FindFirstObjectByType<SqueakSpawner>();

        /// <summary>
        /// Sends a staff message to all players online with <see cref="PlayerPermissions.AdminChat"/> permission.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="player">The player to send message as, null will use Server Host.</param>
        public static void StaffMessage(string message, Player player = null)
        {
            player ??= Server.Host;
            foreach (Player target in Player.List)
            {
                if (!CommandProcessor.CheckPermissions(target.Sender, PlayerPermissions.AdminChat))
                    continue;

                target.ReferenceHub.encryptedChannelManager.TrySendMessageToClient(player.NetId + "!" + message, EncryptedChannelManager.EncryptedChannel.AdminChat);
            }
        }

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
        /// Broadcasts delegate invocation result to all <see cref="Player">players</see>.
        /// </summary>
        /// <param name="duration">The duration in seconds.</param>
        /// <param name="func">The delegate whose invocation result will be the message.</param>
        public static void Broadcast(ushort duration, Func<Player, string> func)
        {
            foreach (Player player in Player.List)
                player.Broadcast(duration, func.Invoke(player));
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
        public static void StartDecontamination() => DecontaminationController.Singleton.FinishDecontamination();

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
        /// Destroy specified amount of specified <see cref="DecalPoolType"/> object.
        /// </summary>
        /// <param name="decalType">Decal type to destroy.</param>
        /// <param name="amount">Amount of decals to destroy.</param>
        public static void Clean(DecalPoolType decalType, int amount) => new DecalCleanupMessage(decalType, amount).SendToAuthenticated();

        /// <summary>
        /// Destroy all specified <see cref="DecalPoolType"/> objects.
        /// </summary>
        /// <param name="decalType">Decal type to destroy.</param>
        public static void Clean(DecalPoolType decalType) => Clean(decalType, int.MaxValue);

        /// <summary>
        /// Places a blood decal.
        /// </summary>
        /// <param name="position">The position of the blood decal.</param>
        /// <param name="direction">The direction of the blood decal.</param>
        [Obsolete("Use PlaceBlood(this Player, Vector3, Vector3, RoleTypeId, int) instead.")]
        public static void PlaceBlood(Vector3 position, Vector3 direction) => _ = 0;

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
        [Obsolete("This method is not working. Use PlayGunSound(Player, Vector3, FirearmType, float, int, bool) overload instead.")]
        public static void PlayGunSound(Vector3 position, ItemType firearmType, byte maxDistance = 45, byte audioClipId = 0)
        {
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

            Firearm.ItemTypeToFirearmInstance.Clear();
            Firearm.BaseCodesValue.Clear();
            Firearm.AvailableAttachmentsValue.Clear();

            CustomKeycardItem.DataDict.Clear();

            SingleUseKeycard.RemainingUses.Clear();

#pragma warning disable CS0618
            Scp559.CakeToWrapper.Clear();

            Coffee.BaseToWrapper.Clear();
#pragma warning restore CS0618
        }
    }
}
