// -----------------------------------------------------------------------
// <copyright file="Example.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Sexiled.Example
{
    using Sexiled.API.Enums;
    using Sexiled.API.Features;
    using Sexiled.Example.Events;

    /// <summary>
    /// The example plugin.
    /// </summary>
    public class Example : Plugin<Config>
    {
        private static readonly Example Singleton = new();

        private ServerHandler serverHandler;
        private PlayerHandler playerHandler;
        private WarheadHandler warheadHandler;
        private MapHandler mapHandler;
        private ItemHandler itemHandler;
        private Scp914Handler scp914Handler;
        private Scp096Handler scp096Handler;

        private Example()
        {
        }

        /// <summary>
        /// Gets the only existing instance of this plugin.
        /// </summary>
        public static Example Instance => Singleton;

        /// <inheritdoc/>
        public override PluginPriority Priority { get; } = PluginPriority.Last;

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            RegisterEvents();

            Log.Warn($"I correctly read the string config, its value is: {Config.String}");
            Log.Warn($"I correctly read the int config, its value is: {Config.Int}");
            Log.Warn($"I correctly read the float config, its value is: {Config.Float}");

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            UnregisterEvents();
            base.OnDisabled();
        }

        /// <summary>
        /// Registers the plugin events.
        /// </summary>
        private void RegisterEvents()
        {
            serverHandler = new ServerHandler();
            playerHandler = new PlayerHandler();
            warheadHandler = new WarheadHandler();
            mapHandler = new MapHandler();
            itemHandler = new ItemHandler();
            scp914Handler = new Scp914Handler();
            scp096Handler = new Scp096Handler();

            Sexiled.Events.Handlers.Server.WaitingForPlayers += serverHandler.OnWaitingForPlayers;
            Sexiled.Events.Handlers.Server.RoundStarted += serverHandler.OnRoundStarted;

            Sexiled.Events.Handlers.Player.Destroying += playerHandler.OnDestroying;
            Sexiled.Events.Handlers.Player.Spawned += playerHandler.OnSpawned;
            Sexiled.Events.Handlers.Player.Escaping += playerHandler.OnEscaping;
            Sexiled.Events.Handlers.Player.Hurting += playerHandler.OnHurting;
            Sexiled.Events.Handlers.Player.Dying += playerHandler.OnDying;
            Sexiled.Events.Handlers.Player.Died += playerHandler.OnDied;
            Sexiled.Events.Handlers.Player.ChangingRole += playerHandler.OnChangingRole;
            Sexiled.Events.Handlers.Player.ChangingItem += playerHandler.OnChangingItem;
            Sexiled.Events.Handlers.Player.UsingItem += playerHandler.OnUsingItem;
            Sexiled.Events.Handlers.Player.PickingUpItem += playerHandler.OnPickingUpItem;
            Sexiled.Events.Handlers.Player.DroppingItem += playerHandler.OnDroppingItem;
            Sexiled.Events.Handlers.Player.Verified += playerHandler.OnVerified;
            Sexiled.Events.Handlers.Player.FailingEscapePocketDimension += playerHandler.OnFailingEscapePocketDimension;
            Sexiled.Events.Handlers.Player.EscapingPocketDimension += playerHandler.OnEscapingPocketDimension;
            Sexiled.Events.Handlers.Player.UnlockingGenerator += playerHandler.OnUnlockingGenerator;
            Sexiled.Events.Handlers.Player.PreAuthenticating += playerHandler.OnPreAuthenticating;
            Sexiled.Events.Handlers.Player.Shooting += playerHandler.OnShooting;
            Sexiled.Events.Handlers.Player.ReloadingWeapon += playerHandler.OnReloading;
            Sexiled.Events.Handlers.Player.ReceivingEffect += playerHandler.OnReceivingEffect;

            Sexiled.Events.Handlers.Warhead.Stopping += warheadHandler.OnStopping;
            Sexiled.Events.Handlers.Warhead.Starting += warheadHandler.OnStarting;

            Sexiled.Events.Handlers.Scp106.Teleporting += playerHandler.OnTeleporting;

            Sexiled.Events.Handlers.Scp914.Activating += playerHandler.OnActivating;
            Sexiled.Events.Handlers.Scp914.ChangingKnobSetting += playerHandler.OnChangingKnobSetting;
            Sexiled.Events.Handlers.Scp914.UpgradingPlayer += playerHandler.OnUpgradingPlayer;

            Sexiled.Events.Handlers.Map.ExplodingGrenade += mapHandler.OnExplodingGrenade;
            Sexiled.Events.Handlers.Map.GeneratorActivating += mapHandler.OnGeneratorActivated;

            Sexiled.Events.Handlers.Item.ChangingAmmo += itemHandler.OnChangingAmmo;
            Sexiled.Events.Handlers.Item.ChangingAttachments += itemHandler.OnChangingAttachments;
            Sexiled.Events.Handlers.Item.ReceivingPreference += itemHandler.OnReceivingPreference;

            Sexiled.Events.Handlers.Scp914.UpgradingPickup += scp914Handler.OnUpgradingItem;

            Sexiled.Events.Handlers.Scp096.AddingTarget += scp096Handler.OnAddingTarget;
        }

        /// <summary>
        /// Unregisters the plugin events.
        /// </summary>
        private void UnregisterEvents()
        {
            Sexiled.Events.Handlers.Server.WaitingForPlayers -= serverHandler.OnWaitingForPlayers;
            Sexiled.Events.Handlers.Server.RoundStarted -= serverHandler.OnRoundStarted;

            Sexiled.Events.Handlers.Player.Destroying -= playerHandler.OnDestroying;
            Sexiled.Events.Handlers.Player.Dying -= playerHandler.OnDying;
            Sexiled.Events.Handlers.Player.Died -= playerHandler.OnDied;
            Sexiled.Events.Handlers.Player.ChangingRole -= playerHandler.OnChangingRole;
            Sexiled.Events.Handlers.Player.ChangingItem -= playerHandler.OnChangingItem;
            Sexiled.Events.Handlers.Player.PickingUpItem += playerHandler.OnPickingUpItem;
            Sexiled.Events.Handlers.Player.Verified -= playerHandler.OnVerified;
            Sexiled.Events.Handlers.Player.FailingEscapePocketDimension -= playerHandler.OnFailingEscapePocketDimension;
            Sexiled.Events.Handlers.Player.EscapingPocketDimension -= playerHandler.OnEscapingPocketDimension;
            Sexiled.Events.Handlers.Player.UnlockingGenerator -= playerHandler.OnUnlockingGenerator;
            Sexiled.Events.Handlers.Player.PreAuthenticating -= playerHandler.OnPreAuthenticating;

            Sexiled.Events.Handlers.Warhead.Stopping -= warheadHandler.OnStopping;
            Sexiled.Events.Handlers.Warhead.Starting -= warheadHandler.OnStarting;

            Sexiled.Events.Handlers.Scp106.Teleporting -= playerHandler.OnTeleporting;

            Sexiled.Events.Handlers.Scp914.Activating -= playerHandler.OnActivating;
            Sexiled.Events.Handlers.Scp914.ChangingKnobSetting -= playerHandler.OnChangingKnobSetting;

            Sexiled.Events.Handlers.Map.ExplodingGrenade -= mapHandler.OnExplodingGrenade;
            Sexiled.Events.Handlers.Map.GeneratorActivating -= mapHandler.OnGeneratorActivated;

            Sexiled.Events.Handlers.Item.ChangingAmmo -= itemHandler.OnChangingAmmo;
            Sexiled.Events.Handlers.Item.ChangingAttachments -= itemHandler.OnChangingAttachments;
            Sexiled.Events.Handlers.Item.ReceivingPreference -= itemHandler.OnReceivingPreference;

            Sexiled.Events.Handlers.Scp914.UpgradingPickup -= scp914Handler.OnUpgradingItem;

            Sexiled.Events.Handlers.Scp096.AddingTarget -= scp096Handler.OnAddingTarget;

            serverHandler = null;
            playerHandler = null;
            warheadHandler = null;
            mapHandler = null;
            itemHandler = null;
            scp914Handler = null;
            scp096Handler = null;
        }
    }
}