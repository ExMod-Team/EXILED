// -----------------------------------------------------------------------
// <copyright file="CustomCandy.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API.Features
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Scp330;
    using InventorySystem.Items;
    using InventorySystem.Items.Usables.Scp330;
    using UnityEngine;
    using YamlDotNet.Serialization;

    using Item = Exiled.API.Features.Items.Item;
    using Player = Exiled.API.Features.Player;
    using Scp330Pickup = Exiled.API.Features.Pickups.Scp330Pickup;

    /// <summary>
    /// The Custom Candy base class.
    /// </summary>
    public abstract class CustomCandy : CustomItem
    {
        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException">Set operation cannot be performed.</exception>
        public override ItemType Type
        {
            get => ItemType.SCP330;
            set => throw new InvalidOperationException("Cannot set Type of CustomItem in CustomCandy.");
        }

        /// <summary>
        /// Gets or sets the candy type.
        /// </summary>
        public abstract CandyKindID CandyType { get; set; }

        /// <summary>
        /// Gets the collection of tracked candies.
        /// </summary>
        [YamlIgnore]
        public Dictionary<ushort, List<int>> TrackedIds { get; } = new();

        /// <summary>
        /// Checks if the candy is tracked by the player.
        /// </summary>
        /// <param name="serial">Bag's serial.</param>
        /// <param name="id">Candy Id.</param>
        /// <returns><c>true</c> if candy is tracked, <c>false</c> otherwise.</returns>
        public bool Check(ushort serial, int id) => TrackedIds.ContainsKey(serial) && TrackedIds[serial].Contains(id);

        /// <summary>
        /// Checks if the candy is tracked by the player.
        /// </summary>
        /// <param name="item">Bag.</param>
        /// <param name="id">Candy Id.</param>
        /// <returns><c>true</c> if candy is tracked, <c>false</c> otherwise.</returns>
        public bool Check(Item item, int id) => Check(item.Serial, id);

        /// <summary>
        /// Adds candy id to tracked serials.
        /// </summary>
        /// <param name="serial">Bag's serial.</param>
        /// <param name="id">Candy Id.</param>
        public void Track(ushort serial, int id)
        {
            if (!TrackedIds.TryGetValue(serial, out List<int> ids))
                TrackedIds.Add(serial, new() { id });
            else
                ids.Add(id);
        }

        /// <inheritdoc />
        public override void Give(Player player, Item item, bool displayMessage = true)
        {
            if (!item.Is(out Scp330 scp330))
                return;

            TrackedSerials.Add(item.Serial);
            Track(item.Serial, scp330.Candies.Count);

            scp330.AddCandy(CandyType);
        }

        /// <summary>
        /// Applies the effects to the player.
        /// </summary>
        /// <param name="target">Target to affect.</param>
        protected abstract void ApplyEffects(Player target);

        private void OnInternalDroppingScp330(DroppingScp330EventArgs ev)
        {
            if (!TrackedIds.TryGetValue(ev.Item.Serial, out List<int> ids))
                return;

            int remove = -1;

            for (int i = 0; i < ids.Count; i++)
            {
                if (ids[i] < ev.Index)
                    ids[i]--;

                if (ids[i] == ev.Index)
                    remove = i;
            }

            if (remove != -1)
            {
                ev.IsAllowed = false;

                ids.RemoveAt(remove);
                ev.Scp330.Base.TryRemove(ev.Index);
                Scp330Pickup scp330Pickup = Pickup.CreateAndSpawn<Scp330Pickup>(ItemType.SCP330, ev.Player.Position, Quaternion.identity, ev.Player).As<Scp330Pickup>();

                scp330Pickup.ExposedCandy = ev.Candy;
                scp330Pickup.Candies.Add(ev.Candy);
                TrackedSerials.Add(scp330Pickup.Serial);
            }
        }

        private void OnInternalEatingScp330(EatingScp330EventArgs ev)
        {
            if (!TrackedIds.TryGetValue(ev.Item.Serial, out List<int> ids))
                return;

            int index = ev.Scp330.Base.SelectedCandyId;

            for (int i = 0; i < ids.Count; i++)
            {
                if (ids[i] < index)
                    ids[i]--;
            }

            ids.Remove(index);
            ApplyEffects(ev.Player);
            ev.IsAllowed = false;
        }

        private void OnInternalPickupScp330(PickingUpItemEventArgs ev)
        {
            if (!Check(ev.Pickup) || !ev.Pickup.Is(out Scp330Pickup pickup))
                return;

            if (TrackedIds.ContainsKey(pickup.Serial))
                return;

            ev.IsAllowed = false;

            if (!ev.Player.HasItem(ItemType.SCP330))
            {
                Scp330 scp330 = ev.Player.AddItem(pickup, ItemAddReason.PickedUp).As<Scp330>();

                if (scp330.AddCandy(pickup.ExposedCandy))
                {
                    TrackedSerials.Remove(pickup.Serial);
                    pickup.Destroy();
                }
            }
        }
    }
}