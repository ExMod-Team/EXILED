namespace Exiled.Events.Patches.Events.Map
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Attributes;

    using EventArgs.Map;

    using HarmonyLib;

    using Respawning.Announcements;

    using System.Text;

    using API.Features;

    using MEC;

    using Subtitles;

    using Utils.Networking;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="WaveAnnouncementBase.SendSubtitles"/> to allow custom strings to be shown on wave spawns.
    /// </summary>
    // [EventPatch(typeof(Handlers.Map), nameof(Handlers.Map.WaveSendingSubtitles))]

    // [HarmonyPatch(typeof(WaveAnnouncementBase), nameof(WaveAnnouncementBase.SendSubtitles))]\
    [HarmonyPatch]
    public class WaveSendSubtitles
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            Log.Info("\n\n\n\n\0000000000000000000 wooooahhhhhh\n\n\n\n");
            Type baseClass = typeof(WaveAnnouncementBase);
            string methodToFind = nameof(WaveAnnouncementBase.SendSubtitles);
            var method = baseClass.GetMethod(methodToFind, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Log.Info("\n\n\n\n\n111111111111111111 wooooahhhhhh\n\n\n\n");
            if (method == null || !method.IsAbstract)
                throw new ArgumentException("Provided method is not abstract");
            Log.Info("\n\n\n\n\nwooooahhhhhh\n\n\n\n");
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!baseClass.IsAssignableFrom(type) || type.IsAbstract)
                        continue;

                    var overridden = type.GetMethod(methodToFind,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

                    if (overridden != null && overridden.GetBaseDefinition() == method)
                        yield return overridden;
                }
            }
        }

        private static bool Prefix(WaveAnnouncementBase __instance)
        {
            StringBuilder messageToSend = new();
            WaveSendingSubtitlesEventArgs ev = new(__instance, messageToSend);
            Handlers.Map.OnWaveSendingSubtitles(ev);

            if (!ev.IsAllowed)
            {
                return false;
            }

            if (ev.runDefaultCode)
            {
                return true;
            }

            List<SubtitlePart> list = new()
            {
                new SubtitlePart(SubtitleType.Custom, ev.Words.ToString()),
            };
            Timing.CallDelayed(5f, () =>
            {
                new SubtitleMessage(list.ToArray()).SendToAuthenticated(0);
            });
            return false;
        }
    }
}