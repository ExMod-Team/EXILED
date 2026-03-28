// -----------------------------------------------------------------------
// <copyright file="Scp2536.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Scp2536;
    using Exiled.Events.Features;
#pragma warning disable SA1623

    /// <summary>
    /// SCP-2536 related events.
    /// </summary>
    public static class Scp2536
    {
        /// <summary>
        /// Invoked before SCP-2536 chooses target to spawn.
        /// </summary>
        public static Event<FindingPositionEventArgs> FindingPosition { get; set; } = new();

        /// <summary>
        /// Invoked once SCP-2536 has found a spawn location.
        /// </summary>
        public static Event<FoundPositionEventArgs> FoundPosition { get; set; } = new();

        /// <summary>
        /// Invoked before SCP-2536 gives a gift to a player.
        /// </summary>
        public static Event<GrantingGiftEventArgs> GrantingGift { get; set; } = new();

        /// <summary>
        /// Invoked before SCP-2536 is open to a player.
        /// </summary>
        public static Event<OpeningGiftEventArgs> OpeningGift { get; set; } = new();

        /// <summary>
        /// Called before SCP-2536 chooses a target.
        /// </summary>
        /// <param name="ev">The <see cref="FindingPositionEventArgs"/> instance.</param>
        public static void OnFindingPosition(FindingPositionEventArgs ev) => FindingPosition.InvokeSafely(ev);

        /// <summary>
        /// Called after SCP-2536 chooses a target.
        /// </summary>
        /// <param name="ev">The <see cref="FoundPositionEventArgs"/> instance.</param>
        public static void OnFoundPosition(FoundPositionEventArgs ev) => FoundPosition.InvokeSafely(ev);

        /// <summary>
        /// Called before SCP-2536 gives a gift to a player.
        /// </summary>
        /// <param name="ev">The <see cref="GrantingGiftEventArgs"/> instance.</param>
        public static void OnGrantingGift(GrantingGiftEventArgs ev) => GrantingGift.InvokeSafely(ev);

        /// <summary>
        /// Called before SCP-2536 is open to a player.
        /// </summary>
        /// <param name="ev">The <see cref="OpeningGiftEventArgs"/> instance.</param>
        public static void OnOppeningGift(OpeningGiftEventArgs ev) => OpeningGift.InvokeSafely(ev);
    }
}