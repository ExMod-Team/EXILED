// -----------------------------------------------------------------------
// <copyright file="MultiGroup.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Permissions.Features.MultipleGroups
{
#pragma warning disable SA1401
    using System;
    using System.Collections.Generic;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;

    using HarmonyLib;

    /// <summary>
    /// Main class for Multi-Group system.
    /// </summary>
    public static class MultiGroup
    {
        /// <summary>
        /// A value indicating whether the system is active.
        /// </summary>
        internal static bool IsActive = false;

        /// <summary>
        /// Internal harmony instance.
        /// </summary>
        private static Harmony harmony;

        /// <summary>
        /// Gets a collection of all groups of all users.
        /// </summary>
        public static Dictionary<string, string[]> Permissions { get; private set; }

        /// <summary>
        /// Main method for initializing MultiGroup.
        /// </summary>
        /// <param name="force">Whether the load should be forced, ignoring config.</param>
        public static void Init(bool force = false) // TODO: add list pooling
        {
            if (!Exiled.Permissions.Permissions.Instance.Config.UseMultiGroup && !force)
                return;

            List<string> groups = ServerStatic.PermissionsHandler._config.GetStringList("AdditionalGroups");
            Dictionary<string, List<string>> users = new();

            foreach (string entry in groups)
            {
                if (!entry.Contains(": "))
                {
                    Log.Error($"Invalid entry at AdditionalGroups in config_remoteadmin.txt: {entry}");
                    continue;
                }

                int length = entry.IndexOf(": ", StringComparison.Ordinal);
                string key = entry.Substring(0, length);
                string value = entry.Substring(length + 2);

                users.GetOrAdd(key, () => ListPool<string>.Pool.Get()).Add(value);
            }

            if (users.Count == 0)
                return;

            try
            {
                harmony = new("exiled.permissions-multigroups");
                harmony.CreateClassProcessor(typeof(BaseCheckPatch)).Patch();
            }
            catch (HarmonyException exception)
            {
                Log.Error($"Patch error while patching for MultiGroups! {exception}");
                return;
            }

            IsActive = true;
            Permissions = new();

            foreach (KeyValuePair<string, List<string>> kvp in users)
            {
                if (ServerStatic.PermissionsHandler.Members.TryGetValue(kvp.Key, out string group))
                    kvp.Value.Add(group);

                Permissions[kvp.Key] = ListPool<string>.Pool.ToArrayReturn(kvp.Value);
            }
        }

        /// <summary>
        /// Disables module and frees all resources.
        /// </summary>
        public static void Disable()
        {
            if (!IsActive)
                return;

            IsActive = false;
            harmony.UnpatchAll("exiled.permissions-multigroups");
            Permissions.Clear();
        }
    }
}