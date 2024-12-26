// -----------------------------------------------------------------------
// <copyright file="Install.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.Hub
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Http;

    using CommandSystem;

    using Exiled.API.Features;
    using Exiled.Events.Commands.Hub.HubApi.Models;
    using Exiled.Loader;
    using Exiled.Loader.GHApi;
    using Exiled.Loader.GHApi.Models;
    using Exiled.Loader.GHApi.Settings;
    using Exiled.Permissions.Extensions;

    using RemoteAdmin;

    /// <summary>
    /// The command to install a plugin from EXILED Hub.
    /// </summary>
    public class Install : ICommand, IUsageProvider
    {
        /// <summary>
        /// Gets static instance of the <see cref="Install"/> command.
        /// </summary>
        public static Install Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "install";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "i" };

        /// <inheritdoc/>
        public string[] Usage { get; } = { "Plugin name", "Release tag (optional)" };

        /// <inheritdoc/>
        public string Description { get; } = "Installs a plugin from EXILED Hub.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            const string permission = "hub.install";

            if (!sender.CheckPermission(permission) && sender is PlayerCommandSender playerSender && !playerSender.FullPermissions)
            {
                response = $"You don't have permissions to install the plugins. Required permission node: \"{permission}\".";
                return false;
            }

            if (arguments.Count == 0)
            {
                response = "Missing arguments! Usage: hub install <plugin name> (release tag)";
                return false;
            }

            using HttpClient client = HubApi.ApiProvider.CreateClient();

            HubPlugin? pluginData = HubApi.ApiProvider.GetInstallationData(arguments.At(0), client).GetAwaiter().GetResult();

            if (pluginData == null)
            {
                response = "An error has occurred while fetching the plugin data. Please check if the plugin name is correct and try again.";
                return false;
            }

            Release[] pluginReleases = client.GetReleases(pluginData.Value.RepositoryId, new GetReleasesSettings(50, 1)).GetAwaiter().GetResult();
            Release releaseToDownload = pluginReleases[0];

            if (arguments.Count > 1)
            {
                Release foundRelease = pluginReleases.FirstOrDefault(x => x.TagName == arguments.At(1));

                if (foundRelease.Id == 0)
                {
                    response = "Release with the provided tag not found.";
                    return false;
                }

                releaseToDownload = foundRelease;
            }

            ReleaseAsset[] releaseAssets = releaseToDownload.Assets.Where(x => x.Name.IndexOf("nwapi", StringComparison.OrdinalIgnoreCase) == -1).ToArray();

            Log.Info($"Downloading release \"{releaseToDownload.TagName}\". Found {releaseAssets.Length} asset(s) to download.");

            foreach (ReleaseAsset asset in releaseAssets)
            {
                Log.Info($"Downloading asset {asset.Name}. Asset size: {Math.Round(asset.Size / 1000f, 2)} KB.");
                using HttpResponseMessage assetResponse = client.GetAsync(asset.BrowserDownloadUrl).ConfigureAwait(false).GetAwaiter().GetResult();

                string pluginPath = Path.Combine(Paths.Plugins, asset.Name);

                if (File.Exists(pluginPath) && Environment.OSVersion.Platform == PlatformID.Unix)
                    LinuxPermission.SetFileUserAndGroupReadWriteExecutePermissions(pluginPath);

                using Stream stream = assetResponse.Content.ReadAsStreamAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                using FileStream fileStream = new(pluginPath, FileMode.Create, FileAccess.Write, FileShare.None);
                stream.CopyToAsync(fileStream).ConfigureAwait(false).GetAwaiter().GetResult();

                if (Environment.OSVersion.Platform == PlatformID.Unix)
                    LinuxPermission.SetFileUserAndGroupReadWriteExecutePermissions(pluginPath);
            }

            response = $"{arguments.At(0)} has been successfully installed.";
            return true;
        }
    }
}