// -----------------------------------------------------------------------
// <copyright file="ScpItemPickupObjective.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Objectives
{
    using Exiled.API.Enums;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Interfaces;
    using Respawning.Objectives;
    using UnityEngine;

    using BaseObjective = Respawning.Objectives.ScpItemPickupObjective;

    /// <summary>
    /// Represents an objective that is completed when a SCP item is picked up.
    /// </summary>
    public class ScpItemPickupObjective : HumanObjective<PickupObjectiveFootprint>, IWrapper<BaseObjective>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScpItemPickupObjective"/> class.
        /// </summary>
        /// <param name="objectiveFootprintBase">A <see cref="BaseObjective"/> instance.</param>
        internal ScpItemPickupObjective(BaseObjective objectiveFootprintBase)
            : base(objectiveFootprintBase)
        {
            Base = objectiveFootprintBase;
        }

        /// <inheritdoc/>
        public new BaseObjective Base { get; }

        /// <inheritdoc/>
        public override ObjectiveType Type { get; } = ObjectiveType.ScpItemPickup;

        /// <summary>
        /// Fakes picking up an item and tries to achieve this objective.
        /// </summary>
        /// <param name="target">Target player.</param>
        /// <param name="item">Item that was picked up.</param>
        /// <param name="pickup">Pickup that was picked up.</param>
        public void AddItem(Player target, Item item, Pickup pickup) => Base.OnItemAdded(target.ReferenceHub, item.Base, pickup.Base);

        /// <summary>
        /// Fakes picking up an item and tries to achieve this objective.
        /// </summary>
        /// <param name="target">Target player.</param>
        /// <param name="item">Item that was picked up.</param>
        public void AddItem(Player target, Item item) => Base.OnItemAdded(target.ReferenceHub, item.Base, item.CreatePickup(Vector3.zero, Quaternion.identity, false).Base);

        /// <summary>
        /// Fakes picking up an item and tries to achieve this objective.
        /// </summary>
        /// <param name="target">Target player.</param>
        /// <param name="pickup">Pickup that was picked up.</param>
        public void AddItem(Player target, Pickup pickup) => Base.OnItemAdded(target.ReferenceHub, Item.Create(pickup.Type, target).Base, pickup.Base);

        /// <summary>
        /// Fakes picking up an item and tries to achieve this objective.
        /// </summary>
        /// <param name="target">Target player.</param>
        /// <param name="type">Item that was picked up.</param>
        public void AddItem(Player target, ItemType type) => AddItem(target, Item.Create(type, target));
    }
}