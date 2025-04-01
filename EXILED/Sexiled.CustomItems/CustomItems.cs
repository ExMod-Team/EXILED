// -----------------------------------------------------------------------
// <copyright file="CustomItems.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Sexiled.CustomItems
{
    using System;

    using Sexiled.API.Features;
    using Sexiled.CustomItems.Events;

    using HarmonyLib;

    /// <summary>
    /// Handles all CustomItem API.
    /// </summary>
    public class CustomItems : Plugin<Config>
    {
        private MapHandler? roundHandler;
        private PlayerHandler? playerHandler;
        private Harmony? harmony;

        /// <summary>
        /// Gets the static reference to this <see cref="CustomItems"/> class.
        /// </summary>
        public static CustomItems? Instance { get; private set; }

        /// <inheritdoc />
        public override void OnEnabled()
        {
            Instance = this;
            roundHandler = new MapHandler();
            playerHandler = new PlayerHandler();

            Sexiled.Events.Handlers.Server.WaitingForPlayers += roundHandler.OnWaitingForPlayers;

            Sexiled.Events.Handlers.Player.ChangingItem += playerHandler.OnChangingItem;

            harmony = new Harmony($"com.{nameof(CustomItems)}.SexiledTeam-{DateTime.Now.Ticks}");
            GlobalPatchProcessor.PatchAll(harmony, out int failedPatch);
            if (failedPatch != 0)
                Log.Error($"Patching failed! There are {failedPatch} broken patches.");

            base.OnEnabled();
        }

        /// <inheritdoc />
        public override void OnDisabled()
        {
            Sexiled.Events.Handlers.Server.WaitingForPlayers -= roundHandler!.OnWaitingForPlayers;

            Sexiled.Events.Handlers.Player.ChangingItem -= playerHandler!.OnChangingItem;

            harmony?.UnpatchAll();

            base.OnDisabled();
        }
    }
}