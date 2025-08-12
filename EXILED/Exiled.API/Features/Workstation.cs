// -----------------------------------------------------------------------
// <copyright file="Workstation.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Interfaces;
    using InventorySystem.Items.Firearms.Attachments;
    using Mirror;
    using UnityEngine;

    /// <summary>
    /// A wrapper class for <see cref="WorkstationController"/>.
    /// </summary>
    public class Workstation : IWrapper<WorkstationController>, IWorldSpace
    {
        /// <summary>
        /// A dictionary mapping <see cref="WorkstationController"/> to <see cref="Workstation"/>.
        /// </summary>
        internal static readonly Dictionary<WorkstationController, Workstation> WorkstationControllerToWorkstation = new(new ComponentsEqualityComparer());

        /// <summary>
        /// Initializes a new instance of the <see cref="Workstation"/> class.
        /// </summary>
        /// <param name="workstationController">The <see cref="WorkstationController"/> to wrap.</param>
        internal Workstation(WorkstationController workstationController)
        {
            WorkstationControllerToWorkstation.Add(workstationController, this);
            Base = workstationController;
        }

        /// <summary>
        /// Gets a read-only collection of all <see cref="Workstation"/> instances.
        /// </summary>
        public static IReadOnlyCollection<Workstation> List => WorkstationControllerToWorkstation.Values;

        /// <summary>
        /// Gets the underlying <see cref="WorkstationController"/> instance.
        /// </summary>
        public WorkstationController Base { get; }

        /// <summary>
        /// Gets the <see cref="GameObject"/> of the workstation.
        /// </summary>
        public GameObject GameObject => Base.gameObject;

        /// <summary>
        /// Gets the <see cref="Transform"/> of the workstation.
        /// </summary>
        public Transform Transform => Base.transform;

        /// <summary>
        /// Gets the <see cref="Room"/> the workstation is located in.
        /// </summary>
        public Room Room => Room.Get(Position);

        /// <summary>
        /// Gets the <see cref="ZoneType"/> of the workstation's room.
        /// </summary>
        public ZoneType Zone => Room.Zone;

        /// <summary>
        /// Gets or sets the position of the workstation.
        /// </summary>
        public Vector3 Position
        {
            get => Transform.position;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                Transform.position = value;
                NetworkServer.Spawn(GameObject);
            }
        }

        /// <summary>
        /// Gets or sets the rotation of the workstation.
        /// </summary>
        public Quaternion Rotation
        {
            get => Transform.rotation;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                Transform.rotation = value;
                NetworkServer.Spawn(GameObject);
            }
        }

        /// <summary>
        /// Gets or sets the status of the workstation.
        /// </summary>
        public WorkstationController.WorkstationStatus Status
        {
            get => (WorkstationController.WorkstationStatus)Base.Status;
            set => Base.NetworkStatus = (byte)value;
        }

        /// <summary>
        /// Gets the <see cref="Stopwatch"/> used by the workstation.
        /// </summary>
        public Stopwatch Stopwatch => Base.ServerStopwatch;

        /// <summary>
        /// Gets or sets the player known to be using the workstation.
        /// </summary>
        public Player KnownUser
        {
            get => Player.Get(Base.KnownUser);
            set => Base.KnownUser = value.ReferenceHub;
        }

        /// <summary>
        /// Gets a <see cref="Workstation"/> given a <see cref="WorkstationController"/> instance.
        /// </summary>
        /// <param name="workstationController">The <see cref="WorkstationController"/> instance.</param>
        /// <returns>The <see cref="Workstation"/> instance.</returns>
        public static Workstation Get(WorkstationController workstationController) => WorkstationControllerToWorkstation.TryGetValue(workstationController, out Workstation workstation) ? workstation : new(workstationController);

        /// <summary>
        /// Gets all <see cref="Workstation"/> instances that match the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to filter workstations.</param>
        /// <returns>An <see cref="IEnumerable{Workstation}"/> of matching workstations.</returns>
        public static IEnumerable<Workstation> Get(Func<Workstation, bool> predicate) => List.Where(predicate);

        /// <summary>
        /// Tries to get all <see cref="Workstation"/> instances that match the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to filter workstations.</param>
        /// <param name="workstations">The matching workstations, if any.</param>
        /// <returns><c>true</c> if any workstations were found; otherwise, <c>false</c>.</returns>
        public static bool TryGet(Func<Workstation, bool> predicate, out IEnumerable<Workstation> workstations)
        {
            workstations = Get(predicate);
            return workstations.Any();
        }

        /// <summary>
        /// Determines whether the specified player is in range of the workstation.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <returns><c>true</c> if the player is in range; otherwise, <c>false</c>.</returns>
        public bool IsInRange(Player player) => Base.IsInRange(player.ReferenceHub);

        /// <summary>
        /// Interacts with the workstation as the specified player.
        /// </summary>
        /// <param name="player">The player to interact as.</param>
        public void Interact(Player player) => Base.ServerInteract(player.ReferenceHub, Base.ActivateCollider.ColliderId);

        /// <summary>
        /// Returns the Room in a human-readable format.
        /// </summary>
        /// <returns>A string containing Workstation-related data.</returns>
        public override string ToString() => $"{GameObject.name} ({Zone}) [{Room}]";
    }
}