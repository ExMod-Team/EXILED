// -----------------------------------------------------------------------
// <copyright file="CustomUnit.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomUnits.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Spawn;
    using Exiled.API.Interfaces;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomUnits.API.Features.Enums;
    using Exiled.Events.EventArgs.Player;
    using MEC;
    using PlayerRoles;
    using Respawning;
    using YamlDotNet.Serialization;

    /// <summary>
    /// A generic class for all custom units.
    /// </summary>
    public abstract class CustomUnit
    {
        private static Dictionary<Type, CustomUnit?> typeLookupTable = new();
        private static Dictionary<string, CustomUnit?> stringLookupTable = new();
        private static Dictionary<uint, CustomUnit?> idLookupTable = new();

        /// <summary>
        /// Gets a list of all registered custom units.
        /// </summary>
        public static HashSet<CustomUnit> Registered { get; } = new();

        /// <summary>
        /// Gets or sets the custom ID of the unit.
        /// </summary>
        public abstract uint Id { get; set; }

        /// <summary>
        /// Gets or sets the name of this unit.
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of this unit.
        /// </summary>
        public abstract string Description { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="RoleTypeId"/> to spawn this unit as.
        /// </summary>
        public abstract List<UnitRole> Roles { get; set; }

        /// <summary>
        /// Gets or sets the spawn type for this unit.
        /// </summary>
        public virtual SpawnType SpawnType { get; set; } = SpawnType.None;

        /// <summary>
        /// Gets all of the players currently set to this unit.
        /// </summary>
        [YamlIgnore]
        public HashSet<Player> TrackedPlayers { get; } = new();

        /// <summary>
        /// Gets or sets the amount of current tickets.
        /// </summary>
        /// <remarks>Will be <c>-1</c> if <see cref="SpawnType"/> is not <see cref="SpawnType.Ticket"/>.</remarks>
        [YamlIgnore]
        public float CurrentTickets { get; set; } = -1f;

        /// <summary>
        /// Gets or sets the amount of maximum tickets that this unit requires to spawn.
        /// </summary>
        public virtual float MinimumTickets { get; set; } = 50f;

        /// <summary>
        /// Gets or sets the amount of maximum tickets that will be subtracted from <see cref="CurrentTickets"/> when this unit spawns..
        /// </summary>
        public virtual float ReduceAmount { get; set; } = 30f;

        /// <summary>
        /// Gets or sets the spawn chance for this unit.
        /// </summary>
        /// <remarks>Will be <c>-1</c> if <see cref="SpawnType"/> is not <see cref="SpawnType.Chance"/>.</remarks>
        public virtual float SpawnChance { get; set; } = -1f;

        /// <summary>
        /// Gets or sets the starting inventory for the unit.
        /// </summary>
        public virtual List<string> Inventory { get; set; } = new();

        /// <summary>
        /// Gets or sets the starting ammo for the unit.
        /// </summary>
        public virtual Dictionary<AmmoType, ushort> Ammo { get; set; } = new();

        /// <summary>
        /// Gets or sets the possible spawn locations for this unit.
        /// </summary>
        public virtual SpawnProperties? SpawnProperties { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether players keep their current position when gaining this unit.
        /// </summary>
        public virtual bool KeepPositionOnSpawn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether players keep their current inventory when gaining this unit.
        /// </summary>
        public virtual bool KeepInventoryOnSpawn { get; set; }

        /// <summary>
        /// Gets or sets an announcement that will be played when unit is spawned.
        /// </summary>
        public virtual string? CassieAnnouncement { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey, TValue}"/> containing custom friendly fire multipliers for every role type.
        /// </summary>
        public virtual Dictionary<RoleTypeId, float> CustomUnitFFMultiplier { get; set; } = new();

        /// <summary>
        /// Gets or sets a <see cref="List{T}"/> of <see cref="Team"/>, <see cref="RoleTypeId"/> or <see cref="CustomUnit.Id"/> that couldn't be damaged by this unit.
        /// </summary>
        public virtual List<string> Friends { get; set; } = new();

        /// <summary>
        /// Gets or sets a minimum amount of players that will be spawned.
        /// </summary>
        public virtual int MinimumToSpawn { get; set; } = 1;

        /// <summary>
        /// Gets or sets a maximum amount of players that will be spawned.
        /// </summary>
        public virtual int MaximumToSpawn { get; set; } = 10;

        /// <summary>
        /// Gets a <see cref="CustomUnit"/> by ID.
        /// </summary>
        /// <param name="id">The ID of the unit to get.</param>
        /// <returns>The unit, or <see langword="null"/> if it doesn't exist.</returns>
        public static CustomUnit? Get(uint id)
        {
            if (!idLookupTable.ContainsKey(id))
                idLookupTable.Add(id, Registered?.FirstOrDefault(r => r.Id == id));
            return idLookupTable[id];
        }

        /// <summary>
        /// Gets a <see cref="CustomUnit"/> by type.
        /// </summary>
        /// <param name="t">The <see cref="Type"/> to get.</param>
        /// <returns>The unit, or <see langword="null"/> if it doesn't exist.</returns>
        public static CustomUnit? Get(Type t)
        {
            if (!typeLookupTable.ContainsKey(t))
                typeLookupTable.Add(t, Registered?.FirstOrDefault(r => r.GetType() == t));
            return typeLookupTable[t];
        }

        /// <summary>
        /// Gets a <see cref="CustomUnit"/> by name.
        /// </summary>
        /// <param name="name">The name of the unit to get.</param>
        /// <returns>The unit, or <see langword="null"/> if it doesn't exist.</returns>
        public static CustomUnit? Get(string name)
        {
            if (!stringLookupTable.ContainsKey(name))
                stringLookupTable.Add(name, Registered?.FirstOrDefault(r => r.Name == name));
            return stringLookupTable[name];
        }

        /// <summary>
        /// Tries to get a <see cref="CustomUnit"/> by <inheritdoc cref="Id"/>.
        /// </summary>
        /// <param name="id">The ID of the unit to get.</param>
        /// <param name="customUnit">The custom unit.</param>
        /// <returns>True if the unit exists.</returns>
        public static bool TryGet(uint id, out CustomUnit? customUnit)
        {
            customUnit = Get(id);

            return customUnit is not null;
        }

        /// <summary>
        /// Tries to get a <see cref="CustomUnit"/> by name.
        /// </summary>
        /// <param name="name">The name of the unit to get.</param>
        /// <param name="customUnit">The custom unit.</param>
        /// <returns>True if the unit exists.</returns>
        /// <exception cref="ArgumentNullException">If the name is <see langword="null"/> or an empty string.</exception>
        public static bool TryGet(string name, out CustomUnit? customUnit)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            customUnit = uint.TryParse(name, out uint id) ? Get(id) : Get(name);

            return customUnit is not null;
        }

        /// <summary>
        /// Tries to get a <see cref="CustomUnit"/> by name.
        /// </summary>
        /// <param name="t">The <see cref="Type"/> of the unit to get.</param>
        /// <param name="customUnit">The custom unit.</param>
        /// <returns>True if the unit exists.</returns>
        /// <exception cref="ArgumentNullException">If the name is <see langword="null"/> or an empty string.</exception>
        public static bool TryGet(Type t, out CustomUnit? customUnit)
        {
            customUnit = Get(t);

            return customUnit is not null;
        }

        /// <summary>
        /// Register all custom units present in the assembly.
        /// </summary>
        /// <param name="targetTypes">Types that will be registered if inheriting <see cref="CustomUnit"/>.</param>
        /// <param name="skipReflection">Whether reflection should be skipped. It's used for creating custom unit from a properties.</param>
        /// <param name="overrideClass">The class where reflection would be used. If <c>null</c> plugin's config will be used instead.</param>
        /// <param name="ignoreAttributes">Whether types without <see cref="CustomTeamAttribute"/> can be registered.</param>
        /// <param name="assembly">Assembly from which types for registration will be taken if <paramref name="targetTypes"/> is <c>null</c>.</param>
        /// <returns>A collection of all registered custom units.</returns>
        public static IEnumerable<CustomUnit> RegisterUnits(IEnumerable<Type>? targetTypes = null, bool skipReflection = false, object? overrideClass = null, bool ignoreAttributes = false, Assembly? assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();
            targetTypes ??= assembly.GetTypes();

            foreach (Type type in targetTypes)
            {
                if (type.BaseType != typeof(CustomUnit) || (!ignoreAttributes && type.GetCustomAttribute<CustomTeamAttribute>() == null))
                    continue;

                CustomUnit? customUnit = null;

                if (!skipReflection && Server.PluginAssemblies.TryGetValue(assembly, out IPlugin<IConfig> plugin))
                {
                    foreach (PropertyInfo property in overrideClass?.GetType().GetProperties() ??
                                                      plugin.Config.GetType().GetProperties())
                    {
                        if (property.PropertyType != type)
                            continue;

                        customUnit = property.GetValue(overrideClass ?? plugin.Config) as CustomUnit;
                    }
                }

                customUnit ??= (CustomUnit)Activator.CreateInstance(type);

                if (customUnit.TryRegister())
                    yield return customUnit;
            }
        }

        /// <summary>
        /// Forces a <see cref="CustomUnit"/> to spawn.
        /// </summary>
        public void Spawn()
        {
            IList<Player> playerToSpawn = Player.List.Where(x => RespawnManager.Singleton.CheckSpawnable(x.ReferenceHub)).ToArray();

            if (playerToSpawn.Count < MinimumToSpawn)
                return;

            playerToSpawn.ShuffleList();

            IList<UnitRole> units = new UnitRole[] { };

            foreach (UnitRole unit in Roles)
            {
                for (int i = 0; i < unit.MaximumAmount; i++)
                    units.Add(unit);
            }

            for (int i = 0; i < Math.Min(MaximumToSpawn, playerToSpawn.Count); i++)
            {
                UnitRole unit = units.Any(x => x.MustSpawn) ? units.GetRandomValue(x => x.MustSpawn) : units.GetRandomValue();

                GrantRole(playerToSpawn[i], unit);
                units.Remove(unit);

                TrackedPlayers.Add(playerToSpawn[i]);
            }

            if (CassieAnnouncement != null)
                Cassie.Message(CassieAnnouncement);
        }

        /// <summary>
        /// Grants a unit to a player.
        /// </summary>
        /// <param name="player">Target player.</param>
        /// <param name="unit">Role to grant.</param>
        public void GrantRole(Player player, UnitRole unit)
        {
            if (unit.CustomRole != null)
                unit.CustomRole.AddRole(player);
            else if (unit.RoleTypeId != RoleTypeId.None)
                player.Role.Set(unit.RoleTypeId, SpawnReason.Respawn, RoleSpawnFlags.All);
            else
                throw new ArgumentException("Role must have a custom unit or a unit type.", nameof(unit));

            Timing.CallDelayed(0.5f, () =>
            {
                if (SpawnProperties != null && !KeepPositionOnSpawn)
                {
                    SpawnPoint spawnPoint = SpawnProperties.GetRandomSpawnPoint();

                    player.Position = spawnPoint.Position;
                }

                if (!KeepInventoryOnSpawn)
                {
                    player.ClearAmmo();
                    player.ClearItems();

                    player.SetAmmo(Ammo);

                    foreach (string str in Inventory)
                    {
                        if (CustomItem.TryGet(str, out CustomItem? customItem) && customItem != null)
                        {
                            customItem.Give(player);
                            continue;
                        }

                        if (Enum.TryParse(str, out ItemType itemType))
                        {
                            player.AddItem(itemType);
                            continue;
                        }

                        Log.Warn($"{Name}: {str} is not a valid ItemType or Custom Item name.");
                    }
                }

                player.FriendlyFireMultiplier = CustomUnitFFMultiplier;

                OnGrantedRole(player, unit);
            });
        }

        /// <summary>
        /// Tries to register a <see cref="CustomUnit"/>.
        /// </summary>
        /// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
        public bool TryRegister()
        {
            if (!CustomRoles.CustomRoles.Instance.Config.IsEnabled)
                return false;

            if (!Registered.Contains(this))
            {
                if (Registered.Any(r => r.Id == Id))
                {
                    Log.Warn($"{Name} has tried to register with the same Role ID as another unit: {Id}. It will not be registered!");

                    return false;
                }

                Registered.Add(this);
                Init();

                Log.Debug($"{Name} ({Id}) has been successfully registered.");

                return true;
            }

            Log.Warn($"Couldn't register {Name} ({Id}) as it already exists.");

            return false;
        }

        /// <summary>
        /// Checks if <see cref="Player"/> is tracked by <see cref="CustomUnit"/>.
        /// </summary>
        /// <param name="player">Target to check.</param>
        /// <returns><c>true</c> if tracked, <c>false</c> otherwise.</returns>
        public bool Check(Player player) => TrackedPlayers.Contains(player);

        /// <summary>
        /// Grants specified amount of tickets to unit.
        /// </summary>
        /// <param name="amount">Amount of tickets to grant.</param>
        public void GrantTickets(float amount)
        {
            if (SpawnType != SpawnType.Ticket)
                return;

            CurrentTickets += amount;
        }

        /// <summary>
        /// Initializes <see cref="CustomUnit"/>.
        /// </summary>
        protected virtual void Init()
        {
            Friends.Add(Id.ToString());

            SubscribeEvents();

            typeLookupTable[GetType()] = this;
            idLookupTable[Id] = this;
            stringLookupTable[Name] = this;
        }

        /// <summary>
        /// Destroys <see cref="CustomUnit"/>.
        /// </summary>
        protected virtual void Destroy()
        {
            UnsubscribeEvents();

            typeLookupTable.Remove(GetType());
            idLookupTable.Remove(Id);
            stringLookupTable.Remove(Name);
        }

        /// <summary>
        /// Subscribes to events.
        /// </summary>
        protected virtual void SubscribeEvents()
        {
        }

        /// <summary>
        /// Unsubscribes from events.
        /// </summary>
        protected virtual void UnsubscribeEvents()
        {
        }

        /// <summary>
        /// Fired when <see cref="GrantRole"/> method is finished.
        /// </summary>
        /// <param name="player">Player that received the unit.</param>
        /// <param name="unit">Role that was granted.</param>
        protected virtual void OnGrantedRole(Player player, UnitRole unit)
        {
        }

        /// <summary>
        /// Fired when unit operative is shooting.
        /// </summary>
        /// <param name="ev"><see cref="ShotEventArgs"/> instance.</param>
        /// <param name="canDamage">Whether or not <see cref="ShotEventArgs.Target"/> can be damaged.</param>
        protected virtual void OnShot(ShotEventArgs ev, bool canDamage)
        {
        }

        private void OnInternalShot(ShotEventArgs ev)
        {
            if (ev.Target == null || !Check(ev.Player))
                return;

            bool canDamage = !(Friends.Contains(ev.Target.Role.Type.ToString()) || Friends.Contains(ev.Target.Role.Team.ToString()));
            CustomUnit[] targetUnits = ev.Target.GetCustomUnits().ToArray();

            if (targetUnits.Length > 0 && Friends.Exists(x => Array.Exists(targetUnits, y => y.Id.ToString() == x)))
                canDamage = false;

            OnShot(ev, canDamage);

            if (!canDamage)
                ev.Damage = 0;
        }
    }
}