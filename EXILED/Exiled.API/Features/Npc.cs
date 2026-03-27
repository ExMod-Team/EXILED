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

    using CommandSystem;
    using CommandSystem.Commands.RemoteAdmin.Dummies;
    using Exiled.API.Enums;
    using Exiled.API.Features.CustomStats;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Roles;
    using Footprinting;
    using InventorySystem;
    using InventorySystem.Items.Usables;
    using InventorySystem.Items.Usables.Scp330;
    using MEC;
    using Mirror;
    using NetworkManagerUtils.Dummies;
    using PlayerRoles;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.Subroutines;
    using PlayerStatsSystem;
    using RelativePositioning;
    using UnityEngine;

    /// <summary>
    /// Wrapper class for handling NPC players.
    /// </summary>
    public class Npc : Player
    {
        /// <summary>
        /// The time it takes for the NPC to receive its <see cref="CustomHumeShieldStat"/> and <see cref="Role"/>.
        /// </summary>
        public const float SpawnSetRoleDelay = 0.5f;

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

            Timing.CallDelayed(SpawnSetRoleDelay, () =>
            {
                npc.Role.Set(role, SpawnReason.ForceClass);
                npc.Position = position;
                npc.CustomHealthStat = (HealthStat)npc.ReferenceHub.playerStats._dictionarizedTypes[typeof(HealthStat)];
                npc.Health = npc.MaxHealth; // otherwise the npc will spawn with 0 health
                npc.ReferenceHub.playerStats._dictionarizedTypes[typeof(HumeShieldStat)] = npc.ReferenceHub.playerStats.StatModules[Array.IndexOf(PlayerStats.DefinedModules, typeof(HumeShieldStat))] = npc.CustomHumeShieldStat = new CustomHumeShieldStat { Hub = npc.ReferenceHub };
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

            Timing.CallDelayed(SpawnSetRoleDelay, () =>
            {
                npc.Role.Set(role, SpawnReason.ForceClass, position is null ? RoleSpawnFlags.All : RoleSpawnFlags.AssignInventory);
                npc.ReferenceHub.playerStats._dictionarizedTypes[typeof(HealthStat)] = npc.ReferenceHub.playerStats.StatModules[Array.IndexOf(PlayerStats.DefinedModules, typeof(HealthStat))] = npc.CustomHealthStat = new HealthStat { Hub = npc.ReferenceHub };
                npc.Health = npc.MaxHealth; // otherwise the npc will spawn with 0 health
                npc.ReferenceHub.playerStats._dictionarizedTypes[typeof(HumeShieldStat)] = npc.ReferenceHub.playerStats.StatModules[Array.IndexOf(PlayerStats.DefinedModules, typeof(HumeShieldStat))] = npc.CustomHumeShieldStat = new CustomHumeShieldStat { Hub = npc.ReferenceHub };

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

        /// <summary>
        /// Moves Npc by direction times distance.
        /// For future maintainer: <see cref="FpcMotor"/> implements <see cref="IDummyActionProvider"/>.
        /// </summary>
        /// <param name="dir">Direction where Npc should move.</param>
        /// <param name="distance">The distance that the Npc should move by.</param>
        /// <returns>true if moved.</returns>
        public bool MoveRelative(Vector3 dir, float distance)
        {
            if (Role is not FpcRole fpcRole)
            {
                return false;
            }

            Vector3 vector = ReferenceHub.PlayerCameraReference.TransformDirection(dir).NormalizeIgnoreY();
            fpcRole.FirstPersonController.FpcModule.Motor.ReceivedPosition = new RelativePosition(Position + vector * distance);
            return true;
        }

        /// <summary>
        /// Makes the Npc look horizontal. Right or Left.
        /// </summary>
        /// <param name="amount">Amount that will be added to horizontal.</param>
        /// <returns>True if successful.</returns>
        public bool LookHorizontal(float amount)
        {
            if (Role is not FpcRole fpcRole)
            {
                return false;
            }

            fpcRole.FirstPersonController.FpcModule.MouseLook.CurrentHorizontal += amount;
            return true;
        }

        /// <summary>
        /// Makes the Npc look vertical. Up or Down.
        /// </summary>
        /// <param name="amount">Amount that will be added to vertical.</param>
        /// <returns>True if successful.</returns>
        public bool LookVertical(float amount)
        {
            if (Role is not FpcRole fpcRole)
            {
                return false;
            }

            fpcRole.FirstPersonController.FpcModule.MouseLook.CurrentVertical += amount;
            return true;
        }

        /// <summary>
        /// Forces Npc to look at certain point.
        /// </summary>
        /// <param name="position">Position to look at.</param>
        /// <param name="lerp">The amount in percentage how much to look at the position, 1 is full and will immediately look at point.</param>
        /// <returns>True if successful.</returns>
        public bool LookAtPoint(Vector3 position, float lerp = 1)
        {
            if (Role is not FpcRole fpcRole)
            {
                return false;
            }

            fpcRole.FirstPersonController.LookAtPoint(position, lerp);
            return true;
        }

        /// <summary>
        /// Forces Npc to look at certain point.
        /// </summary>
        /// <param name="dir">Position to look at.</param>
        /// <param name="lerp">The amount in percentage how much to look at the position, 1 is full and will immediately look at point.</param>
        /// <returns>True if successful.</returns>
        public bool LookAtDirection(Vector3 dir, float lerp = 1)
        {
            if (Role is not FpcRole fpcRole)
            {
                return false;
            }

            fpcRole.FirstPersonController.LookAtDirection(dir, lerp);
            return true;
        }

        /// <summary>
        /// Makes the Npc jump by amount of strength.
        /// </summary>
        /// <param name="jumpStrength">The strength used to jump. Null will choose the default one.</param>
        /// <returns>True if successful.</returns>
        public bool Jump(float? jumpStrength = null)
        {
            if (Role is not FpcRole fpcRole)
            {
                return false;
            }

            fpcRole.Jump(jumpStrength);
            return true;
        }

        /// <summary>
        /// Makes the Npc eat certain kind of candy.
        /// </summary>
        /// <param name="candyKind">The kind of candy to eat.</param>
        /// <returns>True if successful.</returns>
        public bool EatCandy(CandyKindID candyKind)
        {
            foreach(Item? item in Items)
            {
                if (item is not Scp330 scp330)
                {
                    continue;
                }

                return EatCandy(scp330, candyKind);
            }

            return false;
        }

        /// <summary>
        /// Makes the Npc eat certain kind of candy.
        /// </summary>
        /// <param name="from">The <see cref="Scp330"/> bag.</param>
        /// <param name="candyKind">The kind of candy to eat.</param>
        /// <returns>True if successful.</returns>
        public bool EatCandy(Scp330 from, CandyKindID candyKind)
        {
            for (int i = 0; i < from.Candies.Count; i++)
            {
                if (from.Base.Candies[i] == candyKind)
                {
                    from.Base.ServerSelectCandy(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets actions that can be done by the Dummy.
        /// </summary>
        /// <returns><see cref="IReadOnlyList{DummyAction}"/>.</returns>
        public IReadOnlyList<DummyAction> GetActions()
        {
            return DummyActionCollector.ServerGetActions(ReferenceHub);
        }

        /// <summary>
        /// Equips Item.
        /// <see cref="Inventory.PopulateDummyAction"/>.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to equip.</param>
        public void EquipItem(Item item)
        {
            Inventory?.ServerSelectItem(item.Serial);
        }

        /// <summary>
        /// Equips nothing.
        /// <see cref="Inventory.PopulateDummyAction"/>.
        /// </summary>
        public void HolsterItem()
        {
            Inventory?.ServerSelectItem(0);
        }

        /// <summary>
        /// Forces the Npc to stop using item.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to cancel usage of.</param>
        public void CancelUseItem(Item item)
        {
            UsableItemsController.ServerEmulateMessage(item.Serial, StatusMessage.StatusType.Cancel);
        }

        /// <summary>
        /// Forces the Npc to shoot.
        /// </summary>
        /// <param name="hold">Specifies if the shooting is to be held.</param>
        /// <returns>True if successful.</returns>
        public bool Shoot(bool hold) =>
            RunItemAction(CurrentItem, ActionName.Shoot, hold);

        /// <summary>
        /// Forces the Npc to reload.
        /// </summary>
        /// <param name="hold">Specifies if the reloading is to be held.</param>
        /// <returns>True if successful.</returns>
        public bool Reload(bool hold) =>
            RunItemAction(CurrentItem, ActionName.Reload, hold);

        /// <summary>
        /// Forces the Npc to zoom.
        /// </summary>
        /// <param name="hold">Specifies if the zooming is to be held.</param>
        /// <returns>True if successful.</returns>
        public bool Zoom(bool hold) =>
            RunItemAction(CurrentItem, ActionName.Zoom, hold);

        /// <summary>
        /// Forces the Npc to run generic action with item.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to run action for.</param>
        /// <param name="name">The name of action to force.</param>
        /// <param name="hold">Specifies if the action is to be held.</param>
        /// <returns>True if successful.</returns>
        public bool RunItemAction(Item item, ActionName name, bool hold = true) =>
            RunAction(item?.DummyEmulator, name, hold);

        /// <summary>
        /// Forces the Npc to stop generic action with item.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to stop action for.</param>
        /// <param name="name">The name of action to stop.</param>
        /// <returns>True if successful.</returns>
        public bool StopItemAction(Item item, ActionName name) =>
            StopAction(item?.DummyEmulator, name);

        /// <summary>
        /// Checks if certain action is currently active.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to check action for.</param>
        /// <param name="name">The name of action to check.</param>
        /// <returns>True if action is actively executed and emulator is not null.</returns>
        public bool IsBeingDone(Item item, ActionName name) =>
            IsBeingDone(item?.DummyEmulator, name);

        /// <summary>
        /// Forces the Npc to run generic action with subroutine.
        /// </summary>
        /// <param name="subroutine">The <see cref="SubroutineBase"/> to run action for.</param>
        /// <param name="name">The name of action to force.</param>
        /// <param name="hold">Specifies if the action is to be held.</param>
        /// <typeparam name="T"><see cref="SubroutineBase"/>.</typeparam>
        /// <returns>True if successful.</returns>
        public bool RunSubroutineAction<T>(T subroutine, ActionName name, bool hold = true)
            where T : SubroutineBase =>
            RunAction(subroutine?.DummyEmulator, name, hold);

        /// <summary>
        /// Forces the Npc to stop generic action with subroutine.
        /// </summary>
        /// <param name="subroutine">The <see cref="SubroutineBase"/> to stop action for.</param>
        /// <param name="name">The name of action to stop.</param>
        /// <typeparam name="T"><see cref="SubroutineBase"/>.</typeparam>
        /// <returns>True if successful.</returns>
        public bool StopSubroutineAction<T>(T subroutine, ActionName name)
            where T : SubroutineBase =>
            StopAction(subroutine?.DummyEmulator, name);

        /// <summary>
        /// Checks if certain action is currently active.
        /// </summary>
        /// <param name="subroutine">The <see cref="SubroutineBase"/> to check action for.</param>
        /// <param name="name">The name of action to check.</param>
        /// <typeparam name="T"><see cref="SubroutineBase"/>.</typeparam>
        /// <returns>True if action is actively executed and emulator is not null.</returns>
        public bool IsBeingDone<T>(T subroutine, ActionName name)
            where T : SubroutineBase =>
            IsBeingDone(subroutine?.DummyEmulator, name);

        /// <summary>
        /// Forces the Npc to run generic action with emulator.
        /// </summary>
        /// <param name="emulator">The <see cref="DummyKeyEmulator"/> to run action for.</param>
        /// <param name="name">The name of action to force.</param>
        /// <param name="hold">Specifies if the action is to be held.</param>
        /// <returns>True if successful.</returns>
        public bool RunAction(DummyKeyEmulator? emulator, ActionName name, bool hold)
        {
            if (emulator == null)
            {
                return false;
            }

            emulator.AddEntry(name, !hold);
            return true;
        }

        /// <summary>
        /// Forces the Npc to stop generic action with emulator.
        /// </summary>
        /// <param name="emulator">The <see cref="DummyKeyEmulator"/> to stop action for.</param>
        /// <param name="name">The name of action to stop.</param>
        /// <returns>True if successful.</returns>
        public bool StopAction(DummyKeyEmulator? emulator, ActionName name)
        {
            if (emulator == null)
            {
                return false;
            }

            emulator.RemoveEntry(name);
            return true;
        }

        /// <summary>
        /// Checks if certain action is currently active.
        /// </summary>
        /// <param name="emulator">The <see cref="DummyKeyEmulator"/> to check action for.</param>
        /// <param name="name">The name of action to check.</param>
        /// <returns>True if action is actively executed and emulator is not null.</returns>
        public bool IsBeingDone(DummyKeyEmulator? emulator, ActionName name)
        {
            if (emulator == null)
            {
                return false;
            }

            return emulator.GetAction(name, false);
        }
    }
}
