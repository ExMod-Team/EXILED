// -----------------------------------------------------------------------
// <copyright file="Utility.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Utility
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    using API.Enums;
    using Exiled.API.Features;
    using HarmonyLib;

    /// <summary>
    /// Patch and unpatch events into the game.
    /// </summary>
    public sealed class Utility : Plugin<Config>
    {
        /// <summary>
        /// Gets the plugin instance.
        /// </summary>
        public static Utility Instance { get; private set; }

        /// <summary>
        /// Gets the eventHandler.
        /// </summary>
        public static EventHandler EventHandler { get; private set; }

        /// <summary>
        /// Gets the Harmony instance.
        /// </summary>
        public static Harmony Harmony { get; private set; }

        /// <inheritdoc/>
        public override PluginPriority Priority { get; } = PluginPriority.Higher;

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Instance = this;
            EventHandler = new EventHandler
            {
                Config = Config,
            };
            Events.Handlers.Player.ChangingRole += EventHandler.OnChangingRole;
            Events.Handlers.Server.WaitingForPlayers += EventHandler.OnWaitingForPlayers;
            Events.Handlers.Map.PlacingBulletHole += EventHandler.OnPlacingBulletHole;

            // BloodDecal is Missing
            Patch();
            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            base.OnDisabled();
            Events.Handlers.Player.ChangingRole += EventHandler.OnChangingRole;
            Events.Handlers.Server.WaitingForPlayers += EventHandler.OnWaitingForPlayers;
            Events.Handlers.Map.PlacingBulletHole += EventHandler.OnPlacingBulletHole;
            Unpatch();
        }

        /// <summary>
        /// Patches all events.
        /// </summary>
        public void Patch()
        {
            Harmony = new Harmony($"com.{nameof(Utility)}.ExiledTeam-{DateTime.Now.Ticks}");
            GlobalPatchProcessor.PatchAll(Harmony, out int failedPatch);
            if (failedPatch != 0)
                Log.Error($"Patching failed! There are {failedPatch} broken patches.");
        }

        /// <summary>
        /// Unpatches all events.
        /// </summary>
        public void Unpatch()
        {
            Harmony.UnpatchAll(Harmony.Id);
            Harmony = null;
        }
    }
}
