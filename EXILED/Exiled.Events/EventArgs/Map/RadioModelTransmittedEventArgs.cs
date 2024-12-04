// -----------------------------------------------------------------------
// <copyright file="RadioModelTransmittedEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    /// <summary>
    /// Contains all information after the server transmitted radiomodel message.
    /// </summary>
    public class RadioModelTransmittedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RadioModelTransmittedEventArgs" /> class.
        /// </summary>
        /// <param name="serial">
        /// <inheritdoc cref="Serial" />
        /// </param>
        /// <param name="transmittedPlayer">
        /// <inheritdoc cref="TransmittedPlayer" />
        /// </param>
        /// <param name="transmittedRadioModel">
        /// <inheritdoc cref="TransmittedRadioModel" />
        /// </param>
        public RadioModelTransmittedEventArgs(ushort serial, bool transmittedPlayer, bool transmittedRadioModel) // Влад, если ты это читаешь, отправь мне скрин в дс)
        {
            Serial = serial;
            TransmittedPlayer = transmittedPlayer;
            TransmittedRadioModel = transmittedRadioModel;
        }

        /// <summary>
        /// Gets a radio serial.
        /// </summary>
        public ushort Serial { get; }

        /// <summary>
        /// Gets a value indicating whether gets whether the player has transmitted.
        /// </summary>
        public bool TransmittedPlayer { get; }

        /// <summary>
        /// Gets a value indicating whether gets whether the radiomodel has transmitted a signal.
        /// </summary>
        public bool TransmittedRadioModel { get; }
    }
}