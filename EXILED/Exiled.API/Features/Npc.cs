// -----------------------------------------------------------------------
// <copyright file="Npc.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
#nullable enable
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using CentralAuth;
    using CommandSystem;
    using CommandSystem.Commands.RemoteAdmin.Dummies;
    using Exiled.API.Enums;
    using Exiled.API.Features.Components;
    using Exiled.API.Features.Roles;
    using Footprinting;
    using GameCore;
    using MEC;
    using Mirror;
    using PlayerRoles;
    using UnityEngine;

    using Object = UnityEngine.Object;

    /// <summary>
    /// Wrapper class for handling NPC players.
    /// </summary>
    public class Npc : Player
    {
        /// <inheritdoc cref="Player" />
        public Npc(ReferenceHub referenceHub)
            : base(referenceHub)
        {
        }

        /// <inheritdoc cref="Player" />
        public Npc(GameObject gameObject)
            : base(gameObject)
        {
        }

        /// <summary>
        /// Gets a list of Npcs.
        /// </summary>
        public static new IReadOnlyCollection<Npc> List => Dictionary.Values.OfType<Npc>().ToList();

        /// <summary>
        /// Gets or sets the player's position.
        /// </summary>
        public override Vector3 Position
        {
            get => base.Position;
            set
            {
                base.Position = value;
                if (Role is FpcRole fpcRole)
                    fpcRole.ClientRelativePosition = new(value);
            }
        }

        /// <summary>
        /// Gets or sets the player being followed.
        /// </summary>
        /// <remarks>The npc must have <see cref="PlayerFollower"/>.</remarks>
        public Player? FollowedPlayer
        {
            get => !GameObject.TryGetComponent(out PlayerFollower follower) ? null : Player.Get(follower._hubToFollow);

            set
            {
                if (!GameObject.TryGetComponent(out PlayerFollower follower))
                {
                    GameObject.AddComponent<PlayerFollower>()._hubToFollow = value?.ReferenceHub;
                    return;
                }

                follower._hubToFollow = value?.ReferenceHub;
            }
        }

        /// <summary>
        /// Gets or sets the Max Distance of the npc.
        /// </summary>
        /// <remarks>The npc must have <see cref="PlayerFollower"/>.</remarks>
        public float? MaxDistance
        {
            get
            {
                if (!GameObject.TryGetComponent(out PlayerFollower follower))
                    return null;

                return follower._maxDistance;
            }

            set
            {
                if(!value.HasValue)
                    return;

                if (!GameObject.TryGetComponent(out PlayerFollower follower))
                {
                    GameObject.AddComponent<PlayerFollower>()._maxDistance = value.Value;
                    return;
                }

                follower._maxDistance = value.Value;
            }
        }

        /// <summary>
        /// Gets or sets the Min Distance of the npc.
        /// </summary>
        /// <remarks>The npc must have <see cref="PlayerFollower"/>.</remarks>
        public float? MinDistance
        {
            get
            {
                if (!GameObject.TryGetComponent(out PlayerFollower follower))
                    return null;

                return follower._minDistance;
            }

            set
            {
                if(!value.HasValue)
                    return;

                if (!GameObject.TryGetComponent(out PlayerFollower follower))
                {
                    GameObject.AddComponent<PlayerFollower>()._minDistance = value.Value;
                    return;
                }

                follower._minDistance = value.Value;
            }
        }

        /// <summary>
        /// Gets or sets the Speed of the npc.
        /// </summary>
        /// <remarks>The npc must have <see cref="PlayerFollower"/>.</remarks>
        public float? Speed
        {
            get
            {
                if (!GameObject.TryGetComponent(out PlayerFollower follower))
                    return null;

                return follower._speed;
            }

            set
            {
                if(!value.HasValue)
                    return;

                if (!GameObject.TryGetComponent(out PlayerFollower follower))
                {
                    GameObject.AddComponent<PlayerFollower>()._speed = value.Value;
                    return;
                }

                follower._speed = value.Value;
            }
        }

        /// <summary>
        /// Retrieves the NPC associated with the specified ReferenceHub.
        /// </summary>
        /// <param name="rHub">The ReferenceHub to retrieve the NPC for.</param>
        /// <returns>The NPC associated with the ReferenceHub, or <c>null</c> if not found.</returns>
        public static new Npc? Get(ReferenceHub rHub) => Player.Get(rHub) as Npc;

        /// <summary>
        /// Retrieves the NPC associated with the specified GameObject.
        /// </summary>
        /// <param name="gameObject">The GameObject to retrieve the NPC for.</param>
        /// <returns>The NPC associated with the GameObject, or <c>null</c> if not found.</returns>
        public static new Npc? Get(GameObject gameObject) => Player.Get(gameObject) as Npc;

        /// <summary>
        /// Retrieves the NPC associated with the specified user ID.
        /// </summary>
        /// <param name="userId">The user ID to retrieve the NPC for.</param>
        /// <returns>The NPC associated with the user ID, or <c>null</c> if not found.</returns>
        public static new Npc? Get(string userId) => Player.Get(userId) as Npc;

        /// <summary>
        /// Retrieves the NPC associated with the specified ID.
        /// </summary>
        /// <param name="id">The ID to retrieve the NPC for.</param>
        /// <returns>The NPC associated with the ID, or <c>null</c> if not found.</returns>
        public static new Npc? Get(int id) => Player.Get(id) as Npc;

        /// <summary>
        /// Retrieves the NPC associated with the specified ICommandSender.
        /// </summary>
        /// <param name="sender">The ICommandSender to retrieve the NPC for.</param>
        /// <returns>The NPC associated with the ICommandSender, or <c>null</c> if not found.</returns>
        public static new Npc? Get(ICommandSender sender) => Player.Get(sender) as Npc;

        /// <summary>
        /// Retrieves the NPC associated with the specified Footprint.
        /// </summary>
        /// <param name="footprint">The Footprint to retrieve the NPC for.</param>
        /// <returns>The NPC associated with the Footprint, or <c>null</c> if not found.</returns>
        public static new Npc? Get(Footprint footprint) => Player.Get(footprint) as Npc;

        /// <summary>
        /// Retrieves the NPC associated with the specified CommandSender.
        /// </summary>
        /// <param name="sender">The CommandSender to retrieve the NPC for.</param>
        /// <returns>The NPC associated with the CommandSender, or <c>null</c> if not found.</returns>
        public static new Npc? Get(CommandSender sender) => Player.Get(sender) as Npc;

        /// <summary>
        /// Retrieves the NPC associated with the specified Collider.
        /// </summary>
        /// <param name="collider">The Collider to retrieve the NPC for.</param>
        /// <returns>The NPC associated with the Collider, or <c>null</c> if not found.</returns>
        public static new Npc? Get(Collider collider) => Player.Get(collider) as Npc;

        /// <summary>
        /// Retrieves the NPC associated with the specified net ID.
        /// </summary>
        /// <param name="netId">The net ID to retrieve the NPC for.</param>
        /// <returns>The NPC associated with the net ID, or <c>null</c> if not found.</returns>
        public static new Npc? Get(uint netId) => Player.Get(netId) as Npc;

        /// <summary>
        /// Retrieves the NPC associated with the specified NetworkConnection.
        /// </summary>
        /// <param name="conn">The NetworkConnection to retrieve the NPC for.</param>
        /// <returns>The NPC associated with the NetworkConnection, or <c>null</c> if not found.</returns>
        public static new Npc? Get(NetworkConnection conn) => Player.Get(conn) as Npc;

        /// <summary>
        /// Spawns an NPC based on the given parameters.
        /// </summary>
        /// <param name="name">The name of the NPC.</param>
        /// <param name="role">The RoleTypeId of the NPC.</param>
        /// <param name="position">The position where the NPC should spawn.</param>
        /// <returns>Docs4.</returns>
        public static Npc Spawn(string name, RoleTypeId role, Vector3 position)
        {
            Npc npc = new(DummyUtils.SpawnDummy(name));

            Timing.CallDelayed(0.5f, () =>
            {
                npc.Role.Set(role);
                npc.Position = position;
            });

            Dictionary.Add(npc.GameObject, npc);
            return npc;
        }

        /// <summary>
        /// Spawns an NPC based on the given parameters.
        /// </summary>
        /// <param name="name">The name of the NPC.</param>
        /// <param name="role">The RoleTypeId of the NPC, defaulting to None.</param>
        /// <param name="ignored">Whether the NPC should be ignored by round ending checks.</param>
        /// <param name="position">The position where the NPC should spawn. If null, the default spawn location is used.</param>
        /// <returns>The <see cref="Npc"/> spawned.</returns>
        public static Npc Spawn(string name, RoleTypeId role = RoleTypeId.None, bool ignored = false, Vector3? position = null)
        {
            Npc npc = new(DummyUtils.SpawnDummy(name));

            Timing.CallDelayed(0.5f, () =>
            {
                npc.Role.Set(role, SpawnReason.ForceClass, position is null ? RoleSpawnFlags.All : RoleSpawnFlags.AssignInventory);

                if (position is not null)
                    npc.Position = position.Value;
            });

            if (ignored)
                Round.IgnoredPlayers.Add(npc.ReferenceHub);

            Dictionary.Add(npc.GameObject, npc);
            return npc;
        }

        /// <summary>
        /// Destroys all NPCs currently spawned.
        /// </summary>
        public static void DestroyAll() => DummyUtils.DestroyAllDummies();

        /// <summary>
        /// Follow a specific player.
        /// </summary>
        /// <param name="player">the Player to follow.</param>
        public void Follow(Player player)
        {
            PlayerFollower follow = !GameObject.TryGetComponent(out PlayerFollower follower) ? GameObject.AddComponent<PlayerFollower>() : follower;

            follow.Init(player.ReferenceHub);
        }

        /// <summary>
        /// Follow a specific player.
        /// </summary>
        /// <param name="player">the Player to follow.</param>
        /// <param name="maxDistance">the max distance the npc will go.</param>
        /// <param name="minDistance">the min distance the npc will go.</param>
        /// <param name="speed">the speed the npc will go.</param>
        public void Follow(Player player, float maxDistance, float minDistance, float speed = 30f)
        {
            PlayerFollower follow = !GameObject.TryGetComponent(out PlayerFollower follower) ? GameObject.AddComponent<PlayerFollower>() : follower;

            follow.Init(player.ReferenceHub, maxDistance, minDistance, speed);
        }

        /// <summary>
        /// Destroys the NPC.
        /// </summary>
        public void Destroy()
        {
            try
            {
                Round.IgnoredPlayers.Remove(ReferenceHub);
                Dictionary.Remove(ReferenceHub.gameObject);
                NetworkServer.Destroy(ReferenceHub.gameObject);
            }
            catch (Exception e)
            {
                Log.Error($"Error while destroying a NPC: {e.Message}");
            }
        }

        /// <summary>
        /// Schedules the destruction of the NPC after a delay.
        /// </summary>
        /// <param name="time">The delay in seconds before the NPC is destroyed.</param>
        public void LateDestroy(float time)
        {
            Timing.CallDelayed(time, () =>
            {
                this?.Destroy();
            });
        }
    }
}
