// -----------------------------------------------------------------------
// <copyright file="Server.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Exiled.API.Enums;

    using GameCore;

    using Interfaces;

    using MEC;

    using Mirror;

    using PlayerRoles.RoleAssign;

    using RoundRestarting;

    using UnityEngine;

    /// <summary>
    /// A set of tools to easily work with the server.
    /// </summary>
    public static class Server
    {
        private static MethodInfo sendSpawnMessage;

        /// <summary>
        /// Gets a dictionary that pairs assemblies with their associated plugins.
        /// </summary>
        public static Dictionary<Assembly, IPlugin<IConfig>> PluginAssemblies { get; } = new();

        /// <summary>
        /// Gets the player's host of the server.
        /// Might be <see langword="null"/> when called when the server isn't loaded.
        /// </summary>
        public static Player Host { get; internal set; }

        /// <summary>
        /// Gets the cached <see cref="global::Broadcast"/> component.
        /// </summary>
        public static global::Broadcast Broadcast => global::Broadcast.Singleton;

        /// <summary>
        /// Gets the cached <see cref="SendSpawnMessage"/> <see cref="MethodInfo"/>.
        /// </summary>
        public static MethodInfo SendSpawnMessage => sendSpawnMessage ??= typeof(NetworkServer).GetMethod("SendSpawnMessage", BindingFlags.NonPublic | BindingFlags.Static);

        /// <summary>
        /// Gets or sets the name of the server.
        /// </summary>
        public static string Name
        {
            get => ServerConsole.ServerName;
            set
            {
                ServerConsole.ServerName = value;
                ServerConsole.Singleton.RefreshServerNameSafe();
            }
        }

        /// <summary>
        /// Gets the server's version.
        /// </summary>
        public static string Version => GameCore.Version.VersionString;

        /// <summary>
        /// Gets a value indicating whether streaming of this version is allowed.
        /// </summary>
        public static bool StreamingAllowed => GameCore.Version.StreamingAllowed;

        /// <summary>
        /// Gets a value indicating whether this server is on a beta version of SCP:SL.
        /// </summary>
        public static bool IsBeta => GameCore.Version.PublicBeta || GameCore.Version.PrivateBeta;

        /// <summary>
        /// Gets a value indicating the type of build this server is hosted on.
        /// </summary>
        public static GameCore.Version.VersionType BuildType => GameCore.Version.BuildType;

        /// <summary>
        /// Gets the RemoteAdmin permissions handler.
        /// </summary>
        public static PermissionsHandler PermissionsHandler => ServerStatic.PermissionsHandler;

        /// <summary>
        /// Gets the Ip address of the server.
        /// </summary>
        public static string IpAddress => ServerConsole.Ip;

        /// <summary>
        /// Gets a value indicating whether this server is a dedicated server.
        /// </summary>
        public static bool IsDedicated => ServerStatic.IsDedicated;

        /// <summary>
        /// Gets the port of the server.
        /// </summary>
        public static ushort Port => ServerStatic.ServerPort;

        /// <summary>
        /// Gets the actual ticks per second of the server.
        /// </summary>
        public static double Tps
        {
            get
            {
                double delta = Time.deltaTime;

                if (delta <= 0)
                    return MaxTps;

                double tps = 1d / delta;

                tps = Math.Min(tps, MaxTps);

                return tps;
            }
        }

        /// <summary>
        /// Gets the average ticks per second of the server.
        /// </summary>
        public static double SmoothTps => Math.Round(1f / Time.smoothDeltaTime);

        /// <summary>
        /// Gets or sets the max ticks per second of the server.
        /// </summary>
        public static short MaxTps
        {
            get => ServerStatic.ServerTickrate;
            set => ServerStatic.ServerTickrate = value;
        }

        /// <summary>
        /// Gets the actual frametime of the server.
        /// </summary>
        public static double Frametime => Math.Round(1f / Time.deltaTime);

        /// <summary>
        /// Gets or sets a value indicating whether friendly fire is enabled.
        /// </summary>
        /// <seealso cref="Player.IsFriendlyFireEnabled"/>
        public static bool FriendlyFire
        {
            get => ServerConsole.FriendlyFire;
            set
            {
                ServerConsole.FriendlyFire = value;
                ServerConfigSynchronizer.Singleton.RefreshMainBools();

                PlayerStatsSystem.AttackerDamageHandler.RefreshConfigs();
            }
        }

        /// <inheritdoc cref="Player.Count"/>
        public static int PlayerCount => Player.Count;

        /// <inheritdoc cref="Player.ConnectedCount"/>
        public static int PlayerConnectedCount => Player.ConnectedCount;

        /// <summary>
        /// Gets or sets the maximum number of players able to be on the server.
        /// </summary>
        public static int MaxPlayerCount
        {
            get => CustomNetworkManager.slots;
            set => CustomNetworkManager.slots = value;
        }

        /// <summary>
        /// Gets a value indicating whether late join is enabled.
        /// </summary>
        public static bool LateJoinEnabled => LateJoinTime > 0;

        /// <summary>
        /// Gets the late join time, in seconds. If a player joins less than this many seconds into a game, they will be given a random class.
        /// </summary>
        public static float LateJoinTime => ConfigFile.ServerConfig.GetFloat(RoleAssigner.LateJoinKey, 0f);

        /// <summary>
        /// Gets or sets a value indicating whether the server is marked as Heavily Modded.
        /// <remarks>
        /// Read the VSR for more info about its usage.
        /// </remarks>
        /// </summary>
        [Obsolete("This field has been deleted because it used the wrong field (TransparentlyModded)")]
        public static bool IsHeavilyModded
        {
            get => false;
            set => _ = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the server is marked as Transparently Modded.
        /// <remarks>
        /// It is not used now, wait for a new VSR update.
        /// </remarks>
        /// </summary>
        public static bool IsTransparentlyModded
        {
            get => ServerConsole.TransparentlyModdedServerConfig;
            set => ServerConsole.TransparentlyModdedServerConfig = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this server has the whitelist enabled.
        /// </summary>
        public static bool IsWhitelisted
        {
            get => ServerConsole.WhiteListEnabled;
            set => ServerConsole.WhiteListEnabled = value;
        }

        /// <summary>
        /// Gets the list of user IDs of players currently whitelisted.
        /// </summary>
        public static HashSet<string> WhitelistedPlayers => WhiteList.Users;

        /// <summary>
        /// Gets a value indicating whether this server is verified.
        /// </summary>
        public static bool IsVerified => CustomNetworkManager.IsVerified;

        /// <summary>
        /// Gets or sets a value indicating whether idle mode is enabled.
        /// </summary>
        public static bool IsIdleModeEnabled
        {
            get => IdleMode.IdleModeEnabled;
            set => IdleMode.IdleModeEnabled = value;
        }

        /// <summary>
        /// Gets the dictionary of the server's session variables.
        /// <para>
        /// Session variables can be used to save temporary data. Data is stored in a <see cref="Dictionary{TKey, TValue}"/>.
        /// The key of the data is always a <see cref="string"/>, whereas the value can be any <see cref="object"/>.
        /// The data stored in a session variable can be accessed by different assemblies; it is recommended to uniquely identify stored data so that it does not conflict with other plugins that may also be using the same name.
        /// Data saved with session variables is not being saved on server restart. If the data must be saved after a restart, a database must be used instead.
        /// </para>
        /// </summary>
        public static Dictionary<string, object> SessionVariables { get; } = new();

        /// <summary>
        /// Restarts the server, reconnects all players.
        /// </summary>
        /// <seealso cref="RestartRedirect(ushort)"/>
        public static void Restart() => Round.Restart(false, true, ServerStatic.NextRoundAction.Restart);

        /// <summary>
        /// Restarts the server with specified options.
        /// </summary>
        /// <param name="fastRestart">Indicates whether the restart should be fast.</param>
        /// <param name="overrideRestartAction">Indicates whether to override the default restart action.</param>
        /// <param name="restartAction">Specifies the action to perform after the restart.</param>
        public static void Restart(bool fastRestart, bool overrideRestartAction = false, ServerStatic.NextRoundAction restartAction = ServerStatic.NextRoundAction.DoNothing) =>
            Round.Restart(fastRestart, overrideRestartAction, restartAction);

        /// <summary>
        /// Shutdowns the server, disconnects all players.
        /// </summary>
        /// <seealso cref="ShutdownRedirect(ushort)"/>
        public static void Shutdown() => global::Shutdown.Quit();

        /// <summary>
        /// Shutdowns the server, disconnects all players.
        /// </summary>
        /// <param name="quit">Indicates whether to terminate the application after shutting down the server.</param>
        /// <param name="suppressShutdownBroadcast">Indicates whether to suppress the broadcast notification about the shutdown.</param>
        /// <seealso cref="ShutdownRedirect(ushort)"/>
        public static void Shutdown(bool quit, bool suppressShutdownBroadcast = false) => global::Shutdown.Quit(quit, suppressShutdownBroadcast);

        /// <summary>
        /// Redirects players to a server on another port, restarts the current server.
        /// </summary>
        /// <param name="redirectPort">The port to redirect players to.</param>
        /// <returns><see langword="true"/> if redirection was successful; otherwise, <see langword="false"/>.</returns>
        /// <remarks>If the returned value is <see langword="false"/>, the server won't restart.</remarks>
        public static bool RestartRedirect(ushort redirectPort)
        {
            NetworkServer.SendToAll(new RoundRestartMessage(RoundRestartType.RedirectRestart, 0.0f, redirectPort, true, false));
            Timing.CallDelayed(0.5f, Restart);

            return true;
        }

        /// <summary>
        /// Redirects players to a server on another port, restarts the current server.
        /// </summary>
        /// <param name="redirectPort">The port to redirect players to.</param>
        /// <param name="fastRestart">Indicates whether the restart should be fast.</param>
        /// <param name="overrideRestartAction">Indicates whether to override the default restart action.</param>
        /// <param name="restartAction">Specifies the action to perform after the restart.</param>
        public static void RestartRedirect(ushort redirectPort, bool fastRestart, bool overrideRestartAction = false, ServerStatic.NextRoundAction restartAction = ServerStatic.NextRoundAction.DoNothing)
        {
            NetworkServer.SendToAll(new RoundRestartMessage(RoundRestartType.RedirectRestart, 0.0f, redirectPort, true, false));
            Timing.CallDelayed(0.5f, () => { Restart(fastRestart, overrideRestartAction, restartAction); });
        }

        /// <summary>
        /// Redirects players to a server on another port, shutdowns the current server.
        /// </summary>
        /// <param name="redirectPort">The port to redirect players to.</param>
        /// <returns><see langword="true"/> if redirection was successful; otherwise, <see langword="false"/>.</returns>
        /// <remarks>If the returned value is <see langword="false"/>, the server won't shutdown.</remarks>
        public static bool ShutdownRedirect(ushort redirectPort)
        {
            NetworkServer.SendToAll(new RoundRestartMessage(RoundRestartType.RedirectRestart, 0.0f, redirectPort, true, false));
            Timing.CallDelayed(0.5f, Shutdown);

            return true;
        }

        /// <summary>
        /// Redirects players to a server on another port, shutdowns the current server.
        /// </summary>
        /// <param name="redirectPort">The port to redirect players to.</param>
        /// <param name="quit">Indicates whether to terminate the application after shutting down the server.</param>
        /// <param name="suppressShutdownBroadcast">Indicates whether to suppress the broadcast notification about the shutdown.</param>
        public static void ShutdownRedirect(ushort redirectPort, bool quit, bool suppressShutdownBroadcast = false)
        {
            NetworkServer.SendToAll(new RoundRestartMessage(RoundRestartType.RedirectRestart, 0.0f, redirectPort, true, false));
            Timing.CallDelayed(0.5f, () => { Shutdown(quit, suppressShutdownBroadcast); });
        }

        /// <summary>
        /// Executes a server command.
        /// </summary>
        /// <param name="command">The command to be run.</param>
        /// <param name="sender">The <see cref="CommandSender"/> running the command.</param>
        /// <returns>Command response, if there is one; otherwise, <see langword="null"/>.</returns>
        public static string ExecuteCommand(string command, CommandSender sender = null) => GameCore.Console.Singleton.TypeCommand(command, sender);

        /// <summary>
        /// Safely gets an <see cref="object"/> from <see cref="SessionVariables"/>, then casts it to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The returned object type.</typeparam>
        /// <param name="key">The key of the object to get.</param>
        /// <param name="result">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter is used.</param>
        /// <returns><see langword="true"/> if the SessionVariables contains an element with the specified key; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetSessionVariable<T>(string key, out T result)
        {
            if (SessionVariables.TryGetValue(key, out object value) && value is T type)
            {
                result = type;
                return true;
            }

            result = default;
            return false;
        }
    }
}