// -----------------------------------------------------------------------
// <copyright file="Marshmallow.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using CustomPlayerEffects;
    using Exiled.API.Interfaces;
    using InventorySystem.Items.MarshmallowMan;
    using PlayerStatsSystem;
    using UnityEngine;

    /// <summary>
    /// A wrapper class for <see cref="MarshmallowItem"/>.
    /// </summary>
    public class Marshmallow : Item, IWrapper<MarshmallowItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Marshmallow"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="MarshmallowItem"/> class.</param>
        public Marshmallow(MarshmallowItem itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Marshmallow"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the marshmallow item.</param>
        /// <param name="owner">The owner of the marshmallow item. Leave <see langword="null"/> for no owner.</param>
        internal Marshmallow(ItemType type, Player owner = null)
            : base((MarshmallowItem)(owner ?? Server.Host).Inventory.CreateItemInstance(new(type, 0), false))
        {
        }

        /// <summary>
        /// Gets the <see cref="MarshmallowItem"/> that this class is encapsulating.
        /// </summary>
        public new MarshmallowItem Base { get; }

        /// <summary>
        /// Gets a value indicating whether this marshmallow man is evil.
        /// </summary>
        /// <remarks>See <see cref="MakeEvil"/> in regards to making a marshmallow evil.</remarks>
        public bool Evil => Base.EvilMode;

        /// <summary>
        /// Gets or sets the <see cref="AhpStat.AhpProcess"/> of the marshmallow man that would be used if he was evil.
        /// </summary>
        public AhpStat.AhpProcess EvilAhpProcess
        {
            get => Base.EvilAHPProcess;
            set
            {
                if (Evil && value is null)
                    return;

                Base.EvilAHPProcess = value;
            }
        }

        /// <summary>
        /// Cackles for the owner even if they are not evil.
        /// </summary>
        /// <param name="cooldown">How long until the player can cackle again (negative values do not affect current cooldown).</param>
        /// <param name="duration">How long players near the marshmallow man get effected by <see cref="TraumatizedByEvil"/>.</param>
        public void Cackle(double cooldown = -1, float duration = 5)
        {
            if (cooldown >= 0)
                Base._cackleCooldown.Trigger(cooldown);

            Base.ServerSendPublicRpc(writer =>
            {
                writer.WriteByte(4);
                Base._cackleCooldown.WriteCooldown(writer);
            });

            foreach (Player player in Player.List)
            {
                if (Vector3.Distance(player.Position, Owner.Position) <= 5F && player.CurrentItem is not Marshmallow { Evil: true })
                    player.EnableEffect<TraumatizedByEvil>(duration);
            }
        }

        /// <summary>
        /// Makes the owner of this marshmallow evil. You CANNOT undo this without resetting the player.
        /// </summary>
        /// <param name="evilProcess">The <see cref="AhpStat.AhpProcess"/> of the new evil player.</param>
        public void MakeEvil(AhpStat.AhpProcess evilProcess = null)
        {
            if (Evil)
                return;

            Base.ReleaseEvil(evilProcess ?? EvilAhpProcess ?? Owner.GetModule<AhpStat>().ServerAddProcess(450F, 450F, 0F, 1F, 0F, true));
        }
    }
}