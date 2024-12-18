// -----------------------------------------------------------------------
// <copyright file="Objective.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Objectives
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features.Core;
    using Exiled.API.Interfaces;
    using PlayerRoles;
    using Respawning;
    using Respawning.Objectives;

    using BaseHumanDamageObjective = Respawning.Objectives.HumanDamageObjective;
    using BaseHumanKillObjective = Respawning.Objectives.HumanKillObjective;
    using BaseScpPickupObjective = Respawning.Objectives.ScpItemPickupObjective;

    /// <summary>
    /// A wrapper for Faction objective.
    /// </summary>
    public class Objective : TypeCastObject<FactionObjectiveBase>, IWrapper<FactionObjectiveBase>
    {
        /// <summary>
        /// A dictionary of all objectives.
        /// </summary>
        private static readonly Dictionary<ObjectiveType, Objective> Objectives = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Objective"/> class.
        /// </summary>
        /// <param name="objectiveFootprintBase"><inheritdoc cref="Base"/></param>
        public Objective(FactionObjectiveBase objectiveFootprintBase)
        {
            Base = objectiveFootprintBase;
        }

        /// <summary>
        /// Gets all objectives.
        /// </summary>
        public static IReadOnlyCollection<Objective> List => Objectives.Values;

        /// <inheritdoc/>
        public FactionObjectiveBase Base { get; }

        /// <summary>
        /// Gets the type of objective.
        /// </summary>
        public virtual ObjectiveType Type { get; } = ObjectiveType.None;

        /// <summary>
        /// Gets the objective by its type.
        /// </summary>
        /// <param name="type">Type of objective.</param>
        /// <returns>An <see cref="Objective"/> instance if found, <c>null</c> otherwise.</returns>
        public static Objective Get(ObjectiveType type)
        {
            if (Objectives.TryGetValue(type, out Objective objective))
                return objective;

            objective = type switch
            {
                ObjectiveType.ScpItemPickup => new ScpItemPickupObjective(FactionInfluenceManager.Objectives.OfType<BaseScpPickupObjective>().First()),
                ObjectiveType.GeneratorActivation => new GeneratorActivatedObjective(FactionInfluenceManager.Objectives.OfType<Respawning.Objectives.GeneratorActivatedObjective>().First()),
                ObjectiveType.HumanDamage => new HumanDamageObjective(FactionInfluenceManager.Objectives.OfType<BaseHumanDamageObjective>().First()),
                ObjectiveType.HumanKill => new HumanKillObjective(FactionInfluenceManager.Objectives.OfType<BaseHumanKillObjective>().First()),
                _ => null
            };

            Objectives.Add(type, objective);
            return objective;
        }

        /// <summary>
        /// Reduces timer for faction.
        /// </summary>
        /// <param name="faction">Faction to affect.</param>
        /// <param name="seconds">Time to reduce in seconds.</param>
        public void ReduceTimer(Faction faction, float seconds) => Base.ReduceTimer(faction, seconds);

        /// <summary>
        /// Grants influence to faction.
        /// </summary>
        /// <param name="faction">Faction to affect.</param>
        /// <param name="amount">Amount of influence to grant.</param>
        public void GrantInfluence(Faction faction, float amount) => Base.GrantInfluence(faction, amount);

        /// <summary>
        /// Achieves objective.
        /// </summary>
        public void Achieve() => Base.ServerSendUpdate();

        /// <summary>
        /// Checks if faction has this objective.
        /// </summary>
        /// <param name="faction">Faction to check.</param>
        /// <returns><c>true</c> if faction has this objective, <c>false</c> otherwise.</returns>
        public bool IsValidFaction(Faction faction) => Base.IsValidFaction(faction);

        /// <summary>
        /// Checks if player has this objective.
        /// </summary>
        /// <param name="player">Player to check.</param>
        /// <returns><c>true</c> if player has this objective, <c>false</c> otherwise.</returns>
        public bool IsValidFaction(Player player) => Base.IsValidFaction(player.ReferenceHub);
    }
}