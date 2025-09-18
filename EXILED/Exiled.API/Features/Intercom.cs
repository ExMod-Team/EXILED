// -----------------------------------------------------------------------
// <copyright file="Intercom.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using Mirror;

    using PlayerRoles.Voice;

    using UnityEngine;

    using GameIntercom = PlayerRoles.Voice.Intercom;

    /// <summary>
    /// A set of tools to easily handle the Intercom.
    /// </summary>
    public static class Intercom
    {
        /// <summary>
        /// Gets the instance of <see cref="PlayerRoles.Voice.IntercomDisplay"/>.
        /// </summary>
        public static IntercomDisplay IntercomDisplay => IntercomDisplay._singleton;

        /// <summary>
        /// Gets or sets the text displayed on the intercom screen.
        /// </summary>
        public static string DisplayText
        {
            get => IntercomDisplay.Network_overrideText;
            set => IntercomDisplay.Network_overrideText = value;
        }

        /// <summary>
        /// Gets or sets the current state of the intercom.
        /// </summary>
        public static IntercomState State
        {
            get => GameIntercom.State;
            set => GameIntercom.State = value;
        }

        /// <summary>
        /// Gets the intercom's <see cref="UnityEngine.GameObject"/>.
        /// </summary>
        public static GameObject GameObject => GameIntercom._singleton.gameObject;

        /// <summary>
        /// Gets the intercom's <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public static Transform Transform => GameIntercom._singleton.transform;

        /// <summary>
        /// Gets a value indicating whether the intercom is currently being used.
        /// </summary>
        public static bool InUse => State is IntercomState.InUse or IntercomState.Starting;

        /// <summary>
        /// Gets the <see cref="Player"/> that is using the intercom.
        /// </summary>
        /// <remarks>Will be <see langword="null"/> if <see cref="InUse"/> is <see langword="false"/>.</remarks>
        public static Player Speaker => !InUse ? null : Player.Get(GameIntercom._singleton._curSpeaker);

        /// <summary>
        /// Gets or sets the remaining cooldown of the intercom.
        /// </summary>
        public static double RemainingCooldown
        {
            get => GameIntercom._singleton.Network_nextTime - NetworkTime.time;
            set => GameIntercom._singleton.Network_nextTime = NetworkTime.time + value;
        }

        /// <summary>
        /// Gets or sets the remaining speech time of the intercom.
        /// </summary>
        public static float SpeechRemainingTime
        {
            get => !InUse ? 0f : GameIntercom._singleton.RemainingTime;
            set => GameIntercom._singleton._nextTime = NetworkTime.time + value;
        }

        /// <summary>
        /// Plays the intercom's sound.
        /// </summary>
        /// <param name="isStarting">Sets a value indicating whether the sound is the intercom's start speaking sound.</param>
        public static void PlaySound(bool isStarting) => GameIntercom._singleton.RpcPlayClip(isStarting);

        /// <summary>
        /// Modifies whether the player is overriding the intercom to speak globally.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> whose intercom override state will be changed.</param>
        /// <param name="newState">Indicates whether the player should be given the global intercom override.</param>
        /// <returns><see langword="true"/> if the player was successfully added or removed from the override list; otherwise, <see langword="false"/>.</returns>
        public static bool TrySetOverride(Player player, bool newState) => GameIntercom.TrySetOverride(player?.ReferenceHub, newState);

        /// <summary>
        /// Checks whether the player is currently overriding the intercom to speak globally.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check for a global intercom override.</param>
        /// <returns><see langword="true"/> if the player has global intercom override; otherwise, <see langword="false"/>.</returns>
        public static bool HasOverride(Player player) => GameIntercom.HasOverride(player?.ReferenceHub);

        /// <summary>
        /// Reset the intercom's cooldown.
        /// </summary>
        public static void Reset() => State = IntercomState.Ready;

        /// <summary>
        /// Times out the intercom.
        /// </summary>
        public static void Timeout() => State = IntercomState.Cooldown;
    }
}