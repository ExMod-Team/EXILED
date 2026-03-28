// -----------------------------------------------------------------------
// <copyright file="RoleExtensions.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Enums;
    using Features.Spawn;
    using Footprinting;
    using InventorySystem;
    using InventorySystem.Configs;
    using PlayerRoles;
    using PlayerRoles.FirstPersonControl;
    using Respawning;
    using Respawning.Waves;
    using UnityEngine;

    using Team = PlayerRoles.Team;

    /// <summary>
    /// A set of extensions for <see cref="RoleTypeId"/>.
    /// </summary>
    public static class RoleExtensions
    {
        /// <summary>
        /// Compares LifeIdentifier.
        /// </summary>
        /// <param name="footprint">The footprint to compare.</param>
        /// <param name="other">The other footprint.</param>
        /// <returns>If LifeIdentifier is the same (same role).</returns>
        public static bool CompareLife(this Footprint footprint, Footprint other) => footprint.LifeIdentifier == other.LifeIdentifier;

        /// <summary>
        /// Compares LifeIdentifier.
        /// </summary>
        /// <param name="footprint">The footprint to compare.</param>
        /// <param name="other">The hub to compare to.</param>
        /// <returns>If LifeIdentifier is the same (same role).</returns>
        public static bool CompareLife(this Footprint footprint, ReferenceHub other) => footprint.LifeIdentifier == other.roleManager.CurrentRole.UniqueLifeIdentifier;

        /// <summary>
        /// Gets a <see cref="RoleTypeId">role's</see> <see cref="Color"/>.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/> to get the color of.</param>
        /// <returns>The <see cref="Color"/> of the role.</returns>
        public static Color GetColor(this RoleTypeId roleType) => roleType == RoleTypeId.None ? Color.white : roleType.GetRoleBase().RoleColor;

        /// <summary>
        /// Gets a <see cref="RoleTypeId">role's</see> <see cref="Side"/>.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/> to check the side of.</param>
        /// <returns><see cref="Side"/>.</returns>
        public static Side GetSide(this RoleTypeId roleType) => PlayerRolesUtils.GetTeam(roleType).GetSide();

        /// <summary>
        /// Gets a <see cref="Team">team's</see> <see cref="Side"/>.
        /// </summary>
        /// <param name="team">The <see cref="Team"/> to get the <see cref="Side"/> of.</param>
        /// <returns><see cref="Side"/>.</returns>.
        public static Side GetSide(this Team team) => team switch
        {
            Team.SCPs => Side.Scp,
            Team.FoundationForces or Team.Scientists => Side.Mtf,
            Team.ChaosInsurgency or Team.ClassD => Side.ChaosInsurgency,
            Team.OtherAlive => Side.Tutorial,
            Team.Flamingos => Side.Flamingos,
            _ => Side.None,
        };

        /// <summary>
        /// Gets the <see cref="Team"/> of the given <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/>.</param>
        /// <returns><see cref="Team"/>.</returns>
        public static Team GetTeam(this RoleTypeId roleType) => roleType switch
        {
            RoleTypeId.ChaosConscript or RoleTypeId.ChaosMarauder or RoleTypeId.ChaosRepressor or RoleTypeId.ChaosRifleman or RoleTypeId.ChaosFlamingo => Team.ChaosInsurgency,
            RoleTypeId.Scientist => Team.Scientists,
            RoleTypeId.ClassD => Team.ClassD,
            RoleTypeId.Scp049 or RoleTypeId.Scp939 or RoleTypeId.Scp0492 or RoleTypeId.Scp079 or RoleTypeId.Scp096 or RoleTypeId.Scp106 or RoleTypeId.Scp173 or RoleTypeId.Scp3114 or RoleTypeId.ZombieFlamingo=> Team.SCPs,
            RoleTypeId.FacilityGuard or RoleTypeId.NtfCaptain or RoleTypeId.NtfPrivate or RoleTypeId.NtfSergeant or RoleTypeId.NtfSpecialist or RoleTypeId.NtfFlamingo => Team.FoundationForces,
            RoleTypeId.Tutorial => Team.OtherAlive,
            RoleTypeId.Flamingo or RoleTypeId.AlphaFlamingo => Team.Flamingos,
            _ => Team.Dead,
        };

        /// <summary>
        /// Gets the full name of the given <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="typeId">The <see cref="RoleTypeId"/>.</param>
        /// <returns>The full name.</returns>
        public static string GetFullName(this RoleTypeId typeId) => typeId.GetRoleBase().RoleName;

        /// <summary>
        /// Gets the base <see cref="PlayerRoleBase"/> of the given <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/>.</param>
        /// <returns>The <see cref="PlayerRoleBase"/>.</returns>
        public static PlayerRoleBase GetRoleBase(this RoleTypeId roleType) => roleType.TryGetRoleBase(out PlayerRoleBase roleBase) ? roleBase : null;

        /// <summary>
        /// Tries to get the base <see cref="PlayerRoleBase"/> of the given <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/>.</param>
        /// <param name="roleBase">The <see cref="PlayerRoleBase"/> to return.</param>
        /// <returns>The <see cref="PlayerRoleBase"/>.</returns>
        public static bool TryGetRoleBase(this RoleTypeId roleType, out PlayerRoleBase roleBase) => PlayerRoleLoader.TryGetRoleTemplate(roleType, out roleBase);

        /// <summary>
        /// Tries to get the base <see cref="PlayerRoleBase"/> of the given <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/>.</param>
        /// <param name="roleBase">The <see cref="PlayerRoleBase"/> to return.</param>
        /// <typeparam name="T">The type to cast the <see cref="PlayerRoleBase"/> to.</typeparam>
        /// <returns>The <see cref="PlayerRoleBase"/>.</returns>
        public static bool TryGetRoleBase<T>(this RoleTypeId roleType, out T roleBase)
            where T : PlayerRoleBase => PlayerRoleLoader.TryGetRoleTemplate(roleType, out roleBase);

        /// <summary>
        /// Gets the <see cref="LeadingTeam"/>.
        /// </summary>
        /// <param name="team">Team.</param>
        /// <returns><see cref="LeadingTeam"/>.</returns>
        public static LeadingTeam GetLeadingTeam(this Team team) => team switch
        {
            Team.ClassD or Team.ChaosInsurgency => LeadingTeam.ChaosInsurgency,
            Team.FoundationForces or Team.Scientists => LeadingTeam.FacilityForces,
            Team.SCPs => LeadingTeam.Anomalies,
            Team.Flamingos => LeadingTeam.Flamingo,
            _ => LeadingTeam.Draw,
        };

        /// <summary>
        /// Checks whether a <see cref="RoleTypeId"/> is an <see cref="IFpcRole"/> or not.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/>.</param>
        /// <returns>Returns whether <paramref name="roleType"/> is an <see cref="IFpcRole"/> or not.</returns>
        public static bool IsFpcRole(this RoleTypeId roleType) => roleType.GetRoleBase() is IFpcRole;

        /// <summary>
        ///  Checks if the role is an SCP role.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/>.</param>
        /// <returns>A boolean which is true when the role is an SCP role.</returns>
        public static bool IsScp(this RoleTypeId roleType) => roleType.GetTeam() == Team.SCPs;

        /// <summary>
        /// Checks if the role is a dead role.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/>.</param>
        /// <returns>A boolean which is true when the role is a dead role.</returns>
        public static bool IsDead(this RoleTypeId roleType) => roleType.GetTeam() == Team.Dead;

        /// <summary>
        /// Checks if the role is an NTF role.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/>.</param>
        /// <returns>A boolean which is true when the role is an NTF role. Does not include Facility Guards.</returns>
        public static bool IsNtf(this RoleTypeId roleType) => roleType.GetTeam() == Team.FoundationForces && roleType != RoleTypeId.FacilityGuard;

        /// <summary>
        /// Checks if the role is a Chaos role.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/>.</param>
        /// <returns>A boolean which is true when the role is a Chaos role.</returns>
        public static bool IsChaos(this RoleTypeId roleType) => roleType.GetTeam() == Team.ChaosInsurgency;

        /// <summary>
        /// Checks if the role is a military role (Chaos Insurgency or NTF).
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/>.</param>
        /// <returns>A boolean which is true when the role is a military role.</returns>
        public static bool IsMilitary(this RoleTypeId roleType) => roleType.IsNtf() || roleType.IsChaos() || roleType == RoleTypeId.FacilityGuard;

        /// <summary>
        /// Checks if the role is a civilian role (Scientists and Class-D).
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/>.</param>
        /// <returns>A boolean which is true when the role is a civilian role.</returns>
        public static bool IsCivilian(this RoleTypeId roleType) => roleType == RoleTypeId.ClassD || roleType == RoleTypeId.Scientist;

        /// <summary>
        /// Gets a random spawn point of a <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/> to get the spawn point from.</param>
        /// <returns>Returns a <see cref="SpawnLocation"/> representing the spawn, or <see langword="null"/> if no spawns were found.</returns>
        public static SpawnLocation GetRandomSpawnLocation(this RoleTypeId roleType)
        {
            if (roleType.TryGetRoleBase(out FpcStandardRoleBase fpcRole) &&
                fpcRole.SpawnpointHandler != null &&
                fpcRole.SpawnpointHandler.TryGetSpawnpoint(out Vector3 position, out float horizontalRotation))
            {
                return new(roleType, position, horizontalRotation);
            }

            return null;
        }

        /// <summary>
        /// Gets the starting <see cref="InventoryRoleInfo"/> of a <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="role">The <see cref="RoleTypeId"/>.</param>
        /// <returns>The <see cref="InventoryRoleInfo"/> that the role receives on spawn. </returns>
        public static InventoryRoleInfo GetInventory(this RoleTypeId role)
            => StartingInventories.DefinedInventories.TryGetValue(role, out InventoryRoleInfo info)
                ? info
                : new(Array.Empty<ItemType>(), new());

        /// <summary>
        /// Gets the starting items of a <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/>.</param>
        /// <returns>An <see cref="Array"/> of <see cref="ItemType"/> that the role receives on spawn. Will be empty for classes that do not spawn with items.</returns>
        public static ItemType[] GetStartingInventory(this RoleTypeId roleType) => GetInventory(roleType).Items;

        /// <summary>
        /// Gets the starting ammo of a <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/>.</param>
        /// <returns>An <see cref="Array"/> of <see cref="ItemType"/> that the role receives on spawn. Will be empty for classes that do not spawn with ammo.</returns>
        public static Dictionary<AmmoType, ushort> GetStartingAmmo(this RoleTypeId roleType)
        {
            InventoryRoleInfo info = roleType.GetInventory();

            return info.Ammo.ToDictionary(kvp => kvp.Key.GetAmmoType(), kvp => kvp.Value);
        }

        /// <summary>
        /// Gets the <see cref="SpawnableFaction"/> of a <see cref="SpawnableWaveBase"/>.
        /// </summary>
        /// <param name="waveBase">A <see cref="SpawnableWaveBase"/> instance.</param>
        /// <returns><see cref="SpawnableFaction"/> associated with the wave.</returns>
        public static SpawnableFaction GetSpawnableFaction(this SpawnableWaveBase waveBase) => waveBase switch
        {
            NtfSpawnWave => SpawnableFaction.NtfWave,
            NtfMiniWave => SpawnableFaction.NtfMiniWave,
            ChaosSpawnWave => SpawnableFaction.ChaosWave,
            ChaosMiniWave => SpawnableFaction.ChaosMiniWave,
            _ => SpawnableFaction.None
        };

        /// <summary>
        /// Gets the <see cref="Faction"/> associated with the provided <see cref="SpawnableFaction"/>.
        /// </summary>
        /// <param name="spawnableFaction">A member of the <see cref="SpawnableFaction"/> enum.</param>
        /// <returns><see cref="Faction"/> associated with the provided <paramref name="spawnableFaction"/>.</returns>
        public static Faction GetFaction(this SpawnableFaction spawnableFaction) => spawnableFaction switch
        {
            SpawnableFaction.ChaosWave or SpawnableFaction.ChaosMiniWave => Faction.FoundationEnemy,
            SpawnableFaction.NtfWave or SpawnableFaction.NtfMiniWave => Faction.FoundationStaff,
            _ => Faction.Unclassified,
        };

        /// <summary>
        /// Tries to get the <see cref="SpawnableFaction"/> associated with the provided <see cref="Faction"/> and <see cref="bool"/>.
        /// </summary>
        /// <param name="faction">A member of the <see cref="Faction"/>enum.</param>
        /// <param name="spawnableFaction">The <see cref="SpawnableFaction"/> to return.</param>
        /// <param name="mini">A <see cref="bool"/> determining whether to get a normal spawn wave or a mini one.</param>
        /// <returns><see cref="Faction"/> associated with the provided <paramref name="faction"/> influenced by <paramref name="mini"/>.</returns>
        public static bool TryGetSpawnableFaction(this Faction faction, out SpawnableFaction spawnableFaction, bool mini = false)
        {
            switch (faction)
            {
                case Faction.FoundationStaff:
                    spawnableFaction = mini ? SpawnableFaction.NtfMiniWave : SpawnableFaction.NtfWave;
                    break;
                case Faction.FoundationEnemy:
                    spawnableFaction = mini ? SpawnableFaction.ChaosMiniWave : SpawnableFaction.ChaosWave;
                    break;
                default:
                    spawnableFaction = SpawnableFaction.None;
                    return false;
            }

            return true;
        }
    }
}
