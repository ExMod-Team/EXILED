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

    using API.Enums;
    using API.Features;
    using CentralAuth;
    using Exiled.API.Features.Core.UserSettings;
    using Exiled.Events.Features;
    using HarmonyLib;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.Usables;
    using PlayerRoles.Ragdolls;
    using PlayerRoles.RoleAssign;

    using Respawning;
    using UnityEngine.SceneManagement;
    using UserSettings.ServerSpecific;

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
        /// Gets the Harmony instance.
        /// </summary>
        public static Harmony Harmony { get; private set; }

        /// <inheritdoc/>
        public override PluginPriority Priority { get; } = PluginPriority.Higher;

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Instance = this;
            base.OnEnabled();

            Stopwatch watch = Stopwatch.StartNew();

            Patch();

            watch.Stop();

            Log.Info($"{(Config.UseDynamicPatching ? "Non-event" : "All")} patches completed in {watch.Elapsed}");

            ServerConsole.ReloadServerName();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            base.OnDisabled();

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
            Log.Debug("Unpatching events...");
            Harmony.UnpatchAll(Harmony.Id);
            Harmony = null;
            Log.Debug("All events have been unpatched complete. Goodbye!");
        }
    }
}
