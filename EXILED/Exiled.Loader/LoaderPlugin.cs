// -----------------------------------------------------------------------
// <copyright file="LoaderPlugin.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader
{
    using System;
    using System.IO;
    using System.Reflection;

    using LabApi.Loader.Features.Plugins;

    using MEC;

    using Log = API.Features.Log;
    using Paths = API.Features.Paths;

    /// <summary>
    /// The Northwood LabAPI Plugin class for the EXILED Loader.
    /// </summary>
    public class LoaderPlugin : Plugin<Config>
    {
#pragma warning disable SA1401
        /// <summary>
        /// The config for the EXILED Loader.
        /// </summary>
        public static new Config Config;
#pragma warning restore SA1401

        /// <summary>
        /// Gets the Name of the EXILED Loader.
        /// </summary>
        public override string Name => "Exiled Loader";

        /// <summary>
        /// Gets the Description of the EXILED Loader.
        /// </summary>
        public override string Description => "Loads the EXILED Plugin Framework.";

        /// <summary>
        /// Gets the Author of the EXILED Loader.
        /// </summary>
        public override string Author => "ExMod-Team";

        /// <summary>
        /// Gets the RequiredApiVersion of the EXILED Loader.
        /// </summary>
        public override Version RequiredApiVersion => AutoUpdateFiles.RequiredLabAPIVersion;

        /// <summary>
        /// Gets the Exiled Version.
        /// </summary>
        public override Version Version { get; } = new Version(0, 0, 0);
#pragma warning restore SA1401

        /// <summary>
        /// Called by LabAPI when the plugin is enabled.
        /// </summary>
        public override void Enable()
        {
            if (Config == null)
            {
                Log.Error("Detected null config, EXILED will not be loaded.");
                return;
            }

            if (!Config.IsEnabled)
            {
                Log.Info("EXILED is disabled on this server via config.");
                return;
            }

            Log.Info($"Loading EXILED Version: {Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}");

            Paths.Reload(Config.ExiledDirectoryPath);

            Log.Info($"Exiled root path set to: {Paths.Exiled}");

            Directory.CreateDirectory(Paths.Exiled);
            Directory.CreateDirectory(Paths.Configs);
            Directory.CreateDirectory(Paths.Plugins);
            Directory.CreateDirectory(Paths.Dependencies);

            Timing.RunCoroutine(new Loader().Run());
        }

        /// <summary>
        /// Called by LabAPI when the plugin is Disable.
        /// </summary>
        public override void Disable()
        {
            // Plugin will not be disable
        }
    }
}