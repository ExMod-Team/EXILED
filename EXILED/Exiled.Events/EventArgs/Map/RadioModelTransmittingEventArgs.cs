// -----------------------------------------------------------------------
// <copyright file="RadioModelTransmittingEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before the server transmitted radiomodel message.
    /// </summary>
    public class RadioModelTransmittingEventArgs : IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RadioModelTransmittingEventArgs" /> class.
        /// </summary>
        /// <param name="serial">
        /// <inheritdoc cref="Serial" />
        /// </param>
        /// <param name="transmittingPlayer">
        /// <inheritdoc cref="TransmittingPlayer" />
        /// </param>
        /// <param name="transmittingRadioModel">
        /// <inheritdoc cref="TransmittingRadioModel" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public RadioModelTransmittingEventArgs(ushort serial, bool transmittingPlayer, bool transmittingRadioModel, bool isAllowed) // ТЕПЕРЬ НАХУЙ Я НЕ ЗАБУДУ ЗДЕСЬ isAllowed
        {
            Serial = serial;
            TransmittingPlayer = transmittingPlayer;
            TransmittingRadioModel = transmittingRadioModel;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets a radio serial.
        /// </summary>
        public ushort Serial { get; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets whether the player has transmitted.
        /// </summary>
        public bool TransmittingPlayer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets whether the radiomodel has transmitted.
        /// </summary>
        public bool TransmittingRadioModel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets whether to allow the signal to be transmitted.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}