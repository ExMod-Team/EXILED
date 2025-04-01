// -----------------------------------------------------------------------
// <copyright file="LoaderPlugin.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Sexiled.Loader
{
    using System;
    using System.IO;
    using System.Reflection;

    using MEC;

    using PluginAPI.Core.Attributes;

    using Log = API.Features.Log;
    using Paths = API.Features.Paths;

    /// <summary>
    /// The Northwood PluginAPI Plugin class for the SEXILED Loader.
    /// </summary>
    public class LoaderPlugin
    {
#pragma warning disable SA1401
        /// <summary>
        /// The config for the SEXILED Loader.
        /// </summary>
        [PluginConfig]
        public static Config Config;
#pragma warning restore SA1401

        /// <summary>
        /// Called by PluginAPI when the plugin is enabled.
        /// </summary>
        [PluginEntryPoint("Sexiled Loader", null, "Loads the SEXILED Plugin Framework.", "ExMod-Team")]
        [PluginPriority(byte.MinValue)]
        public void Enable()
        {
            if (Config == null)
            {
                Log.Error("Detected null config, SEXILED will not be loaded.");
                return;
            }

            if (!Config.IsEnabled)
            {
                Log.Info("EXILED is disabled on this server via config.");
                return;
            }

            Log.Info($"Loading SEXILED Version: {Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}");

            Paths.Reload(Config.SexiledDirectoryPath);

            Log.Info($"Sexiled root path set to: {Paths.Sexiled}");

            Directory.CreateDirectory(Paths.Sexiled);
            Directory.CreateDirectory(Paths.Configs);
            Directory.CreateDirectory(Paths.Plugins);
            Directory.CreateDirectory(Paths.Dependencies);

            Timing.RunCoroutine(new Loader().Run());
        }
    }
}