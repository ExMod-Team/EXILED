// -----------------------------------------------------------------------
// <copyright file="DatabaseHandler.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CreditTags.Features
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Cryptography;
    using Exiled.API.Features;
    using Exiled.CreditTags.Enums;

    public static class DatabaseHandler
    {
        private const string Url = "https://raw.githubusercontent.com/ExMod-Team/CreditTags/main/data.yml";
        private const string ETagCacheFileName = "etag_cache.txt";
        private const string DatabaseCacheFileName = "data.yml";
        private const int CacheTimeInMinutes = 5;

        static DatabaseHandler()
        {
            // If the cache directory doesn't exist, create it.
            if (!CacheDirectory.Exists)
            {
                CacheDirectory.Create();
                return;
            }

            // If the database cache file exists we process the data.
            if (!File.Exists(DatabaseCachePath))
                return;

            try
            {
                ProcessData(File.ReadAllText(DatabaseCachePath));

                // If the ETag cache file exists we read the data.
                if (File.Exists(ETagCachePath))
                {
                    ETagCache = File.ReadAllText(ETagCachePath);
                }
            }
            catch (Exception e)
            {
                Log.Error($"{nameof(DatabaseHandler)}: There was an error reading the cache files.");
                Log.Error(e);
            }
        }

        /// <summary>
        /// Gets the path to the cache directory.
        /// </summary>
        private static DirectoryInfo CacheDirectory { get; } = new (Path.Combine(Paths.Configs, "CreditTags"));

        /// <summary>
        /// Gets the path to the cache file.
        /// </summary>
        private static string ETagCachePath { get; } = Path.Combine(CacheDirectory.FullName, ETagCacheFileName);

        /// <summary>
        /// Gets the path to the database cache file.
        /// </summary>
        private static string DatabaseCachePath { get; } = Path.Combine(CacheDirectory.FullName, DatabaseCacheFileName);

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey,TValue}"/> of recently cached userIds and their ranks.
        /// </summary>
        private static Dictionary<string, RankType> RankCache { get; } = new();

        /// <summary>
        /// Gets or sets the ETag cache.
        /// </summary>
        private static string ETagCache { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the last time the database was updated.
        /// </summary>
        private static DateTime LastUpdate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Tries to get the rank of a user from the cache.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <param name="rank">The rank of the user.</param>
        /// <returns>Returns a value indicating whether the rank was found.</returns>
        public static bool TryGetRank(string userId, out RankType rank)
        {
            byte[] hash = Sha.Sha256(userId);
            string hashedId = Sha.HashToString(hash);
            Log.Debug("User Id: " + userId + ", Hashed Id: " + hashedId);
            return RankCache.TryGetValue(hashedId, out rank);
        }

        /// <summary>
        /// Updates the data from the database.
        /// </summary>
        public static void UpdateData()
        {
            if (DateTime.Now - LastUpdate < TimeSpan.FromMinutes(CacheTimeInMinutes))
                return;

            ThreadSafeRequest.Go(Url, ETagCache);
            LastUpdate = DateTime.Now;
        }

        /// <summary>
        /// Saves the ETag to the cache.
        /// </summary>
        /// <param name="etag">The ETag to save.</param>
        public static void SaveETag(string etag)
        {
            ETagCache = etag;
            File.WriteAllText(ETagCachePath, etag);
            Log.Debug($"{nameof(SaveETag)}: Successfully saved the ETag to the cache.");
        }

        /// <summary>
        /// Processes the data from the database.
        /// </summary>
        /// <param name="data">The data to process.</param>
        public static void ProcessData(string data)
        {
            try
            {
                TagItem[] items = TagItem.FromYaml(data);

                if (items is null || items.Length == 0)
                {
                    Log.Debug("No items found in the database.");
                    return;
                }

                foreach (TagItem item in items)
                {
                    Log.Debug($"Processing item: {item.Id} - {item.Type}");
                    RankCache[item.Id] = (RankType)item.Type;
                }

                File.WriteAllText(DatabaseCachePath, data);
                Log.Debug($"{nameof(ProcessData)}: Successfully processed the data from the database.");
            }
            catch (Exception e)
            {
                Log.Error("There was an error processing the data from the database.");
                Log.Error(e);
            }
        }
    }
}