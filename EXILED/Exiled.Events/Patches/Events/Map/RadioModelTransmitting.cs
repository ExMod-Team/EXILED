// -----------------------------------------------------------------------
// <copyright file="RadioModelTransmitting.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

namespace Exiled.Events.Patches.Events.Map
{
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using InventorySystem.Items.Radio;
    using PlayerRoles.Voice;
    using VoiceChat.Playbacks;

    /// <summary>
    /// Patches <see cref="RadioViewmodel.GetTxRx" />.
    /// Adds the <see cref="Map.RadioModelTransmitting" /> event and
    /// <see cref="Map.RadioModelTransmitted" /> event.
    /// </summary>
    [EventPatch(typeof(Map), nameof(Map.RadioModelTransmitting))]
    [EventPatch(typeof(Map), nameof(Map.RadioModelTransmitted))]
    [HarmonyPatch(typeof(RadioViewmodel), nameof(RadioViewmodel.GetTxRx))]
    internal static class RadioModelTransmitting
    {
        private static bool Prefix(RadioViewmodel __instance, out bool tx, out bool rx) // One of the Russians, teach me how to write Transpiller.
        {
            tx = false;
            rx = false;
            foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
            {
                if (allHub.roleManager.CurrentRole is IVoiceRole voiceRole && PersonalRadioPlayback.IsTransmitting(allHub))
                {
                    if (allHub == __instance.Hub)
                    {
                        tx = true;
                    }
                    else if (!(voiceRole.VoiceModule as IRadioVoiceModule).RadioPlayback.Source.mute)
                    {
                        rx = true;
                    }
                }
            }

            RadioModelTransmittingEventArgs ev = new(__instance.ItemId.SerialNumber, tx, rx, true);
            Map.OnRadioModelTransmitting(ev);

            if (!ev.IsAllowed)
            {
                tx = false;
                rx = false;
                return false;
            }

            tx = ev.TransmittingPlayer;
            rx = ev.TransmittingRadioModel;
            return false;
        }

        private static void Postfix(RadioViewmodel __instance, ref bool tx, ref bool rx)
        {
            RadioModelTransmittedEventArgs ev = new(__instance.ItemId.SerialNumber, tx, rx);
            Map.OnRadioModelTransmitted(ev);
        }
    }
}
