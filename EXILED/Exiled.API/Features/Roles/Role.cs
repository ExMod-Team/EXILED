// -----------------------------------------------------------------------
// <copyright file="Role.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Pools;
    using Exiled.API.Features.Spawn;
    using Exiled.API.Interfaces;
    using Mirror;

    using PlayerRoles;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.PlayableScps.Scp049.Zombies;
    using UnityEngine;

    using DestroyedGameRole = PlayerRoles.DestroyedRole;
    using FilmmakerGameRole = PlayerRoles.Filmmaker.FilmmakerRole;
    using HumanGameRole = PlayerRoles.HumanRole;
    using NoneGameRole = PlayerRoles.NoneRole;
    using OverwatchGameRole = PlayerRoles.Spectating.OverwatchRole;
    using Scp049GameRole = PlayerRoles.PlayableScps.Scp049.Scp049Role;
    using Scp079GameRole = PlayerRoles.PlayableScps.Scp079.Scp079Role;
    using Scp096GameRole = PlayerRoles.PlayableScps.Scp096.Scp096Role;
    using Scp106GameRole = PlayerRoles.PlayableScps.Scp106.Scp106Role;
    using Scp1507GameRole = PlayerRoles.PlayableScps.Scp1507.Scp1507Role;
    using Scp173GameRole = PlayerRoles.PlayableScps.Scp173.Scp173Role;
    using Scp3114GameRole = PlayerRoles.PlayableScps.Scp3114.Scp3114Role;
    using Scp939GameRole = PlayerRoles.PlayableScps.Scp939.Scp939Role;
    using SpectatorGameRole = PlayerRoles.Spectating.SpectatorRole;

    /// <summary>
    /// Defines the class for role-related classes.
    /// </summary>
    public abstract class Role : TypeCastObject<Role>, IWrapper<PlayerRoleBase>
    {
        private RoleTypeId fakeAppearance;
        private Dictionary<Player, RoleTypeId> individualAppearances = DictionaryPool<Player, RoleTypeId>.Pool.Get();
        private Dictionary<Team, RoleTypeId> teamAppearances = DictionaryPool<Team, RoleTypeId>.Pool.Get();

        /// <summary>
        /// Initializes a new instance of the <see cref="Role"/> class.
        /// </summary>
        /// <param name="baseRole">the base <see cref="PlayerRoleBase"/>.</param>
        protected Role(PlayerRoleBase baseRole)
        {
            if (baseRole.TryGetOwner(out ReferenceHub hub))
                Owner = Player.Get(hub);

            Base = baseRole;
            fakeAppearance = baseRole.RoleTypeId;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Role"/> class.
        /// </summary>
        ~Role()
        {
            DictionaryPool<Player, RoleTypeId>.Pool.Return(individualAppearances);
            DictionaryPool<Team, RoleTypeId>.Pool.Return(teamAppearances);
        }

        /// <summary>
        /// Gets the <see cref="Player"/> this role is referring to.
        /// </summary>
        public virtual Player Owner { get; }

        /// <summary>
        /// Gets the <see cref="RoleTypeId"/> of this <see cref="Player"/>.
        /// </summary>
        public abstract RoleTypeId Type { get; }

        /// <summary>
        /// Gets the base <see cref="PlayerRoleBase"/>.
        /// </summary>
        public PlayerRoleBase Base { get; }

        /// <summary>
        /// Gets the <see cref="RoleChangeReason"/>.
        /// </summary>
        public RoleChangeReason SpawnReason => Base.ServerSpawnReason;

        /// <summary>
        /// Gets the <see cref="RoleSpawnFlags"/>.
        /// </summary>
        public RoleSpawnFlags SpawnFlags => Base.ServerSpawnFlags;

        /// <summary>
        /// Gets the <see cref="PlayerRoles.Team"/> of this <see cref="Role"/>.
        /// </summary>
        public Team Team => Base.Team;

        /// <summary>
        /// Gets the <see cref="Enums.Side"/> of this <see cref="Role"/>.
        /// </summary>
        public Side Side => Base.Team.GetSide();

        /// <summary>
        /// Gets the <see cref="UnityEngine.Color"/> of this <see cref="Role"/>.
        /// </summary>
        public Color Color => Base.RoleColor;

        /// <summary>
        /// Gets the <see cref="Role"/> full name.
        /// </summary>
        public string Name => Base.RoleName;

        /// <summary>
        /// Gets the last time the <see cref="Role"/> was active.
        /// </summary>
        public TimeSpan ActiveTime => TimeSpan.FromSeconds(Base.ActiveTime);

        /// <summary>
        /// Gets a value indicating whether this role represents a dead role.
        /// </summary>
        public bool IsDead => Team is Team.Dead;

        /// <summary>
        /// Gets a value indicating whether this role represents a living role.
        /// </summary>
        public bool IsAlive => !IsDead;

        /// <summary>
        /// Gets a value indicating whether this role is still valid. This will only ever be <see langword="false"/> if the Role is stored and accessed at a later date.
        /// </summary>
        public bool IsValid => Owner != null && Owner.IsConnected && Base == Owner.RoleManager.CurrentRole;

        /// <summary>
        /// Gets the life identifier for the role.
        /// </summary>
        public int LifeIdentifier => Base.UniqueLifeIdentifier;

        /// <summary>
        /// Gets an overriden global <see cref="RoleTypeId"/> appearance.
        /// </summary>
        public RoleTypeId GlobalAppearance => fakeAppearance;

        /// <summary>
        /// Gets an overriden <see cref="RoleTypeId"/> appearance for specific <see cref="Team"/>'s.
        /// </summary>
        public IReadOnlyDictionary<Team, RoleTypeId> TeamAppearances => teamAppearances;

        /// <summary>
        /// Gets an overriden <see cref="RoleTypeId"/> appearance for specific <see cref="Player"/>'s.
        /// </summary>
        public IReadOnlyDictionary<Player, RoleTypeId> IndividualAppearances => individualAppearances;

        /// <summary>
        /// Gets a random spawn position of this role.
        /// </summary>
        /// <returns>The spawn position.</returns>
        public virtual SpawnLocation RandomSpawnLocation => Type.GetRandomSpawnLocation();

        /// <summary>
        /// Converts a role to its appropriate <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="role">The role.</param>
        public static implicit operator RoleTypeId(Role role) => role?.Type ?? RoleTypeId.None;

        /// <summary>
        /// Returns whether 2 roles are the same.
        /// </summary>
        /// <param name="left">The role.</param>
        /// <param name="right">The other role.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(Role left, Role right) => left?.Equals(right) ?? right is null;

        /// <summary>
        /// Returns whether the two roles are different.
        /// </summary>
        /// <param name="left">The role.</param>
        /// <param name="right">The other role.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(Role left, Role right) => !(left == right);

        /// <summary>
        /// Returns whether the role has the same RoleTypeId as the given <paramref name="typeId"/>.
        /// </summary>
        /// <param name="role">The <see cref="Role"/>.</param>
        /// <param name="typeId">The <see cref="RoleTypeId"/>.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(Role role, RoleTypeId typeId) => role?.Type == typeId;

        /// <summary>
        /// Returns whether the role has a different RoleTypeId as the given <paramref name="typeId"/>.
        /// </summary>
        /// <param name="role">The <see cref="Role"/>.</param>
        /// <param name="typeId">The <see cref="RoleTypeId"/>.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(Role role, RoleTypeId typeId) => !(role == typeId);

        /// <summary>
        /// Returns whether the role has the same RoleTypeId as the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="RoleTypeId"/>.</param>
        /// <param name="role">The <see cref="Role"/>.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(RoleTypeId type, Role role) => role == type;

        /// <summary>
        /// Returns whether the role has a different RoleTypeId as the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="RoleTypeId"/>.</param>
        /// <param name="role">The <see cref="Role"/>.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(RoleTypeId type, Role role) => role != type;

        /// <inheritdoc/>
        public override bool Equals(object obj) => base.Equals(obj);

        /// <summary>
        /// Returns the role in a human-readable format.
        /// </summary>
        /// <returns>A string containing role-related data.</returns>
        public override string ToString() => $"{Side} {Team} {Type} {IsValid}";

        /// <inheritdoc/>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>
        /// Sets the player's <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="newRole">The new <see cref="RoleTypeId"/> to be set.</param>
        /// <param name="reason">The <see cref="Enums.SpawnReason"/> defining why the player's role was changed.</param>
        public virtual void Set(RoleTypeId newRole, SpawnReason reason = Enums.SpawnReason.ForceClass) => Set(newRole, reason, RoleSpawnFlags.All);

        /// <summary>
        /// Sets the player's <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="newRole">The new <see cref="RoleTypeId"/> to be set.</param>
        /// <param name="spawnFlags">The <see cref="RoleSpawnFlags"/> defining player spawn logic.</param>
        public virtual void Set(RoleTypeId newRole, RoleSpawnFlags spawnFlags) => Owner.RoleManager.ServerSetRole(newRole, (RoleChangeReason)Enums.SpawnReason.ForceClass, spawnFlags);

        /// <summary>
        /// Sets the player's <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="newRole">The new <see cref="RoleTypeId"/> to be set.</param>
        /// <param name="reason">The <see cref="Enums.SpawnReason"/> defining why the player's role was changed.</param>
        /// <param name="spawnFlags">The <see cref="RoleSpawnFlags"/> defining player spawn logic.</param>
        public virtual void Set(RoleTypeId newRole, SpawnReason reason, RoleSpawnFlags spawnFlags) =>
            Owner.RoleManager.ServerSetRole(newRole, (RoleChangeReason)reason, spawnFlags);

        /// <summary>
        /// Try-set a new global appearance for current <see cref="Role"/>.
        /// </summary>
        /// <param name="newAppearance">New global <see cref="RoleTypeId"/> appearance.</param>
        /// <param name="update">Whether or not the change-role requect should sent imidiately.</param>
        /// <returns>A boolean indicating whether or not a target <see cref="RoleTypeId"/> will be used as new appearance.</returns>
        public bool TrySetGlobalAppearance(RoleTypeId newAppearance, bool update = true)
        {
            if (!CheckAppearanceCompatibility(newAppearance))
            {
                Log.Error($"Prevent Seld-Desync of {Owner.Nickname} ({Type}) with {newAppearance}");
                return false;
            }

            fakeAppearance = newAppearance;

            if (update)
            {
                UpdateAppearance();
            }

            return true;
        }

        /// <summary>
        /// Try-set a new team appearance for current <see cref="Role"/>.
        /// </summary>
        /// <param name="team">Target <see cref="Team"/>.</param>
        /// <param name="newAppearance">New team specific <see cref="RoleTypeId"/> appearance.</param>
        /// <param name="update">Whether or not the change-role requect should sent imidiately.</param>
        /// <returns>A boolean indicating whether or not a target <see cref="RoleTypeId"/> will be used as new appearance.</returns>
        public bool TrySetTeamAppearance(Team team, RoleTypeId newAppearance, bool update = true)
        {
            if (!CheckAppearanceCompatibility(newAppearance))
            {
                Log.Error($"Prevent Seld-Desync of {Owner.Nickname} ({Type}) with {newAppearance}");
                return false;
            }

            teamAppearances[team] = newAppearance;

            if (update)
            {
                UpdateAppearance();
            }

            return true;
        }

        /// <summary>
        /// Try-set a new individual appearance for current <see cref="Role"/>.
        /// </summary>
        /// <param name="player">Target <see cref="Player"/>.</param>
        /// <param name="newAppearance">New individual <see cref="RoleTypeId"/> appearance.</param>
        /// <param name="update">Whether or not the change-role requect should sent imidiately.</param>
        /// <returns>A boolean indicating whether or not a target <see cref="RoleTypeId"/> will be used as new appearance.</returns>
        public bool TrySetIndividualAppearance(Player player, RoleTypeId newAppearance, bool update = true)
        {
            if (!CheckAppearanceCompatibility(newAppearance))
            {
                Log.Error($"Prevent Seld-Desync of {Owner.Nickname} ({Type}) with {newAppearance}");
                return false;
            }

            individualAppearances[player] = newAppearance;

            if (update)
            {
                UpdateAppearanceFor(player);
            }

            return true;
        }

        /// <summary>
        /// resets <see cref="GlobalAppearance"/> to current <see cref="Role.Type"/>.
        /// </summary>
        /// <param name="update">Whether or not the change-role requect should sent imidiately.</param>
        public void ClearGlobalAppearance(bool update = true)
        {
            fakeAppearance = Type;

            if (update)
            {
                UpdateAppearance();
            }
        }

        /// <summary>
        /// Clears all custom <see cref="TeamAppearances"/>.
        /// </summary>
        /// <param name="update">Whether or not the change-role requect should sent imidiately.</param>
        public void ClearTeamAppearances(bool update = true)
        {
            teamAppearances.Clear();

            if (update)
            {
                UpdateAppearance();
            }
        }

        /// <summary>
        /// Clears all custom <see cref="IndividualAppearances"/>.
        /// </summary>
        /// <param name="update">Whether or not the change-role requect should sent imidiately.</param>
        public void ClearIndividualAppearances(bool update = true)
        {
            individualAppearances.Clear();

            if (update)
            {
                UpdateAppearance();
            }
        }

        /// <summary>
        /// Resets current appearance to a real player <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="update">Whether or not the change-role requect should sent imidiately.</param>
        /// <remarks>Clears <see cref="IndividualAppearances"/>, <see cref="TeamAppearances"/> and <see cref="GlobalAppearance"/>.</remarks>
        public void ResetAppearance(bool update = true)
        {
            ClearGlobalAppearance(false);
            ClearTeamAppearances(false);
            ClearIndividualAppearances(false);

            if (update)
            {
                UpdateAppearance();
            }
        }

        /// <summary>
        /// Updates current player appearance.
        /// </summary>
        public void UpdateAppearance()
        {
            if (Owner != null)
                Owner.RoleManager._sendNextFrame = true;
        }

        /// <summary>
        /// Updates current player visibility, for target <see cref="Player"/>.
        /// </summary>
        /// <param name="player">Target <see cref="Player"/>.</param>
        public void UpdateAppearanceFor(Player player)
        {
            RoleTypeId roleTypeId = Type;
            if (Base is IObfuscatedRole obfuscatedRole)
            {
                roleTypeId = obfuscatedRole.GetRoleForUser(player.ReferenceHub);
            }

            player.Connection.Send(new RoleSyncInfo(Owner.ReferenceHub, roleTypeId, player.ReferenceHub));
            Owner.RoleManager.PreviouslySentRole[player.NetId] = roleTypeId;
        }

        /// <summary>
        /// Creates a role from <see cref="RoleTypeId"/> and <see cref="Player"/>.
        /// </summary>
        /// <param name="role">The <see cref="PlayerRoleBase"/>.</param>
        /// <returns>The created <see cref="Role"/> instance.</returns>
        internal static Role Create(PlayerRoleBase role) => role switch
        {
            Scp049GameRole scp049Role => new Scp049Role(scp049Role),
            ZombieRole scp0492Role => new Scp0492Role(scp0492Role),
            Scp079GameRole scp079Role => new Scp079Role(scp079Role),
            Scp096GameRole scp096Role => new Scp096Role(scp096Role),
            Scp106GameRole scp106Role => new Scp106Role(scp106Role),
            Scp173GameRole scp173Role => new Scp173Role(scp173Role),
            Scp3114GameRole scp3114Role => new Scp3114Role(scp3114Role),
            Scp939GameRole scp939Role => new Scp939Role(scp939Role),
            OverwatchGameRole overwatchRole => new OverwatchRole(overwatchRole),
            SpectatorGameRole spectatorRole => new SpectatorRole(spectatorRole),
            HumanGameRole humanRole => new HumanRole(humanRole),
            FilmmakerGameRole filmmakerRole => new FilmMakerRole(filmmakerRole),
            NoneGameRole noneRole => new NoneRole(noneRole),
            DestroyedGameRole destroyedRole => new DestroyedRole(destroyedRole),
#pragma warning disable CS0618
            Scp1507GameRole scp1507 => new Scp1507Role(scp1507),
#pragma warning restore CS0618
            _ => throw new Exception($"Missing role found in Exiled.API.Features.Roles.Role::Create ({role?.RoleTypeId}). Please contact an Exiled developer."),
        };

        /// <summary>
        /// Overrides change role sever message, to implement fake appearance, using basic <see cref="PlayerRoleBase"/>.
        /// </summary>
        /// <param name="writer"><see cref="NetworkWriter"/> to write message.</param>
        /// <param name="basicRole">Original (not fake) <see cref="PlayerRoleBase"/>.</param>
        /// <remarks>Not for public usage. Called on fake <see cref="Role"/> class, not on real <see cref="Role"/> class.</remarks>
        internal virtual void SendAppearanceSpawnMessage(NetworkWriter writer, PlayerRoleBase basicRole)
        {
        }

        /// <summary>
        /// Checks compatibility for target <see cref="RoleTypeId"/> appearance using <see cref="PlayerRoleBase"/>.
        /// </summary>
        /// <param name="newAppearance">New <see cref="RoleTypeId"/>.</param>
        /// <returns>A boolean indicating whether or not a target <see cref="RoleTypeId"/> can be used as new appearance.</returns>
        internal virtual bool CheckAppearanceCompatibility(RoleTypeId newAppearance)
        {
            if (!RoleExtensions.TryGetRoleBase(newAppearance, out PlayerRoleBase roleBase))
                return false;

            return roleBase is not(FpcStandardRoleBase or NoneGameRole);
        }
    }
}
