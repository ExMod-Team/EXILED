// -----------------------------------------------------------------------
// <copyright file="ShowCreditTag.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CreditTags.Commands
{
    using System;

    using CommandSystem;

    using Exiled.API.Features;

    /// <summary>
    /// A client command to show an EXILED credit tag.
    /// </summary>
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ShowCreditTag : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "exiledtag";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new[] { "crtag", "et", "ct" };

        /// <inheritdoc/>
        public string Description { get; } = "Shows your EXILED Credits tag, if available.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            CommandSender cmdSender = (CommandSender)sender;

            if (Player.Get(cmdSender.SenderId) is not Player player)
            {
                response = "You cannot use this command while still authenticating.";
                return false;
            }

            bool found = CreditTags.Instance.ShowCreditTag(player, true);
            response = found ? "Your credit tag has been shown." : "You do not own a credit tag.";
            return true;
        }
    }
}