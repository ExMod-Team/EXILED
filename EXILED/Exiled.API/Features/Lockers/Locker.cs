// -----------------------------------------------------------------------
// <copyright file="Locker.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
namespace Exiled.API.Features.Lockers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Interfaces;

    using InventorySystem.Items.Pickups;
    using MapGeneration.Distributors;

    using Mirror;
    using UnityEngine;

    using BaseLocker = MapGeneration.Distributors.Locker;
#nullable enable
    /// <summary>
    /// The in-game Locker.
    /// </summary>
    public class Locker : IWrapper<BaseLocker>, IWorldSpace
    {
        /// <summary>
        /// A <see cref="Dictionary{TKey,TValue}"/> containing all known <see cref="BaseLocker"/>s and their corresponding <see cref="Locker"/>.
        /// </summary>
        internal static readonly Dictionary<BaseLocker, Locker> BaseToExiledLockers = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Locker"/> class.
        /// </summary>
        /// <param name="locker">The encapsulated <see cref="BaseLocker"/>.</param>
        public Locker(BaseLocker locker)
        {
            Base = locker;
            BaseToExiledLockers.Add(locker, this);

            Chambers = locker.Chambers.Select(x => new Chamber(x, this)).ToList();
            Type = locker.GetLockerType();
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Locker"/> which contains all the <see cref="Locker"/> instances.
        /// </summary>
        public static IReadOnlyCollection<Locker> List => BaseToExiledLockers.Values;

        /// <inheritdoc/>
        public BaseLocker Base { get; }

        /// <summary>
        /// Gets the <see cref="LockerType"/> of the <see cref="Locker"/>.
        /// </summary>
        public LockerType Type { get; }

        /// <summary>
        /// Gets the <see cref="Locker"/> <see cref="UnityEngine.GameObject"/>.
        /// </summary>
        public GameObject GameObject => Base.gameObject;

        /// <summary>
        /// Gets the <see cref="Locker"/> <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public Transform Transform => Base.transform;

        /// <inheritdoc/>
        public Vector3 Position => Base.transform.position;

        /// <inheritdoc/>
        public Quaternion Rotation => Base.transform.rotation;

        /// <summary>
        /// Gets the <see cref="Features.Room"/> in which the <see cref="Locker"/> is located.
        /// </summary>
        public Room? Room => Room.Get(Position);

        /// <summary>
        /// Gets the <see cref="ZoneType"/> in which the locker is located.
        /// </summary>
        public ZoneType Zone => Room?.Zone ?? ZoneType.Unspecified;

        /// <summary>
        /// Gets the all <see cref="Chambers"/> in this locker.
        /// </summary>
        public IReadOnlyCollection<Chamber> Chambers { get; }

        /// <summary>
        /// Gets or sets an id for manipulating opened chambers.
        /// </summary>
        public ushort OpenedChambers
        {
            get => Base.OpenedChambers;
            set => Base.NetworkOpenedChambers = value;
        }

        /// <summary>
        /// Gets a random position from one of the <see cref="Chambers"/>.
        /// </summary>
        public Vector3 RandomChamberPosition
        {
            get
            {
                Chamber randomChamber = Chambers.GetRandomValue();

                // Determine if the chamber uses multiple spawn points and has at least one available spawn point.
                if (randomChamber.UseMultipleSpawnpoints && randomChamber.Spawnpoints.Count() > 0)
                {
                    // Return the position of a random spawn point within the chamber.
                    return randomChamber.Spawnpoints.GetRandomValue().position;
                }

                // Return the position of the main spawn point for the chamber.
                return randomChamber.Spawnpoint.position;
            }
        }

        /// <summary>
        /// Gets the <see cref="Locker"/> belonging to the <see cref="BaseLocker"/>, if any.
        /// </summary>
        /// <param name="locker">The <see cref="BaseLocker"/> to get.</param>
        /// <returns>A <see cref="Locker"/> or <see langword="null"/> if not found.</returns>
        public static Locker? Get(BaseLocker locker) => locker == null ? null :
            BaseToExiledLockers.TryGetValue(locker, out Locker supply) ? supply : new Locker(locker);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Locker"/> given the specified <see cref="ZoneType"/>.
        /// </summary>
        /// <param name="zoneType">The <see cref="ZoneType"/> to search for.</param>
        /// <returns>The <see cref="Locker"/> with the given <see cref="ZoneType"/> or <see langword="null"/> if not found.</returns>
        public static IEnumerable<Locker> Get(ZoneType zoneType) => Get(room => room.Zone.HasFlag(zoneType));

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Locker"/> filtered based on a predicate.
        /// </summary>
        /// <param name="predicate">The condition to satify.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Locker"/> which contains elements that satify the condition.</returns>
        public static IEnumerable<Locker> Get(Func<Locker, bool> predicate) => List.Where(predicate);

        /// <summary>
        /// Gets a random <see cref="Locker"/> based on the specified filters.
        /// </summary>
        /// <param name="zone">The <see cref="ZoneType"/> to filter by. If unspecified, all zones are considered.</param>
        /// <param name="lockerType">The <see cref="LockerType"/> to filter by. If unspecified, all locker types are considered.</param>
        /// <returns>A random <see cref="Locker"/> object, or  <see langword="null"/> if no matching locker is found.</returns>
        public static Locker? Random(ZoneType zone = ZoneType.Unspecified, LockerType lockerType = LockerType.Unknow)
        {
            IEnumerable<Locker> filteredLockers = List;

            if (lockerType != LockerType.Unknow)
                filteredLockers = filteredLockers.Where(l => l.Type == lockerType);

            if (zone != ZoneType.Unspecified)
                filteredLockers = filteredLockers.Where(l => l.Zone == zone);

            return filteredLockers.GetRandomValue();
        }

        /// <summary>
        /// Adds an item to a randomly selected locker chamber.
        /// </summary>
        /// <param name="item">The <see cref="Pickup"/> to be added to the locker chamber.</param>
        public void AddItem(Pickup item)
        {
            // Select a random chamber from the available locker chambers.
            Chamber chamber = Chambers.GetRandomValue();

            // Determine the parent transform where the item will be placed.
            Transform parentTransform = chamber.UseMultipleSpawnpoints && chamber.Spawnpoints.Count() > 0
                ? chamber.Spawnpoints.GetRandomValue()
                : chamber.Spawnpoint;

            // If the chamber is open, immediately set the item's parent and spawn it.
            if (chamber.Base.IsOpen)
            {
                item.Transform.SetParent(parentTransform);
                item.Spawn();
            }
            else
            {
                // If the item is already spawned on the network, unspawn it before proceeding.
                if (NetworkServer.spawned.ContainsKey(item.Base.netId))
                    NetworkServer.UnSpawn(item.GameObject);

                // Set the item's parent transform.
                item.Transform.SetParent(parentTransform);

                // Lock the item in place.
                item.IsLocked = true;

                // Notify any pickup distributor triggers.
                (item.Base as IPickupDistributorTrigger)?.OnDistributed();

                // If the item has a Rigidbody component, make it kinematic and reset its position and rotation.
                if (item.Rigidbody != null)
                {
                    item.Rigidbody.isKinematic = true;
                    item.Rigidbody.transform.localPosition = Vector3.zero;
                    item.Rigidbody.transform.localRotation = Quaternion.identity;

                    // Add the Rigidbody to the list of bodies to be unfrozen later.
                    SpawnablesDistributorBase.BodiesToUnfreeze.Add(item.Rigidbody);
                }

                // If the chamber is configured to spawn items on the first opening, add the item to the list of items to be spawned.
                // Otherwise, spawn the item immediately.
                if (chamber.InitiallySpawn)
                    chamber.Base._toBeSpawned.Add(item.Base);
                else
                    ItemDistributor.SpawnPickup(item.Base);
            }
        }

        /// <summary>
        /// Spawns an item of the specified <see cref="ItemType"/> to the locker by creating a new <see cref="Pickup"/>.
        /// </summary>
        /// <param name="type">The type of item to be added.</param>
        public void AddItem(ItemType type) => AddItem(Pickup.Create(type));

        /// <summary>
        /// Clears the cached lockers in the <see cref="BaseToExiledLockers"/> dictionary.
        /// </summary>
        internal static void ClearCache()
        {
            BaseToExiledLockers.Clear();
            Chamber.Chambers.Clear();
        }
    }
}
