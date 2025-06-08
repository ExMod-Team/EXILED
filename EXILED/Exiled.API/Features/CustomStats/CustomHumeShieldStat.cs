// -----------------------------------------------------------------------
// <copyright file="CustomHumeShieldStat.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.CustomStats
{
    using Mirror;
    using PlayerRoles.PlayableScps.HumeShield;
    using PlayerStatsSystem;
    using UnityEngine;
    using Utils.Networking;

    /// <summary>
    /// A custom version of <see cref="HumeShieldStat"/> which allows the player's max amount of HumeShield to be changed.
    /// </summary>
    public class CustomHumeShieldStat : HumeShieldStat
    {
        /// <summary>
        /// Gets or sets the multiplier for gaining HumeShield.
        /// </summary>
        public float ShieldRegenerationMultiplier { get; set; } = 1;

        private float ShieldRegeneration
        {
            get
            {
                IHumeShieldProvider.GetForHub(Hub, out _, out _, out float hsRegen, out _);
                return hsRegen * ShieldRegenerationMultiplier;
            }
        }

        /// <inheritdoc/>
        public override void Update()
        {
            if (ShieldRegenerationMultiplier is 1)
            {
                base.Update();
                return;
            }

            if (!NetworkServer.active)
                return;

            if (ValueDirty)
            {
                new SyncedStatMessages.StatMessage()
                {
                    Stat = this,
                    SyncedValue = CurValue,
                }.SendToHubsConditionally(CanReceive);
                _lastSent = CurValue;
                ValueDirty = false;
            }

            if (ShieldRegeneration == 0)
                return;

            float delta = ShieldRegeneration * Time.deltaTime;

            if (delta > 0)
            {
                if (CurValue >= MaxValue)
                    return;

                CurValue = Mathf.MoveTowards(CurValue, MaxValue, delta);
                return;
            }

            if (CurValue <= 0)
                return;

            CurValue += delta;
        }
    }
}