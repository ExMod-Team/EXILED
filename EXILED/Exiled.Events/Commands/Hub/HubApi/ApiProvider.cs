// -----------------------------------------------------------------------
// <copyright file="ApiProvider.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.Hub.HubApi
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Reflection;
    using System.Threading.Tasks;

    using Exiled.Events.Commands.Hub.HubApi.Models;

    using Utf8Json;

    /// <summary>
    /// An API bridge to EXILED Hub.
    /// </summary>
    public static class ApiProvider
    {
        /// <summary>
        /// The API endpoint to get the plugin installation data.
        /// </summary>
        private const string InstallApiEndpoint = "https://hub.exiled-team.net/api/install?name=";

        /// <summary>
        /// Gets installation data of the plugin by name.
        /// </summary>
        /// <param name="pluginName">The name of plugin.</param>
        /// <param name="client">The <see cref="HttpClient"/>.</param>
        /// <returns>A <see cref="HubPlugin"/> instance containing installation data.</returns>
        public static async Task<HubPlugin?> GetInstallationData(string pluginName, HttpClient client)
        {
            var url = InstallApiEndpoint + pluginName;
            using var response = await client.GetAsync(url).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                return JsonSerializer.Deserialize<HubPlugin>(stream);
            }

            return null;
        }

        /// <summary>
        /// Creates a HTTP client for EXILED Hub API.
        /// </summary>
        /// <returns>Created HTTP client.</returns>
        internal static HttpClient CreateClient()
        {
            HttpClient client = new();

            client.Timeout = TimeSpan.FromSeconds(460);
            client.DefaultRequestHeaders.Add("User-Agent", $"Exiled.Events (https://github.com/ExMod-Team/EXILED, {Assembly.GetExecutingAssembly().GetName().Version.ToString(3)})");

            return client;
        }
    }
}