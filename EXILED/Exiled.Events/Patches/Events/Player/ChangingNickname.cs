// -----------------------------------------------------------------------
// <copyright file="ChangingNickname.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="NicknameSync.set_Network_displayName"/> to add the <see cref="Handlers.Player.ChangingNickname"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ChangingNickname))]
    [HarmonyPatch(typeof(NicknameSync), nameof(NicknameSync.Network_displayName), MethodType.Setter)]
    internal static class ChangingNickname
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder player = generator.DeclareLocal(typeof(Player));
            LocalBuilder oldName = generator.DeclareLocal(typeof(string));
            Label continueLabel = generator.DefineLabel();

            const int offset = 1;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Brtrue) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(this._hub);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(NicknameSync), nameof(NicknameSync._hub))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, player.LocalIndex),

                // player.CustomName
                new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.CustomName))),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, oldName.LocalIndex),

                // value;
                new(OpCodes.Ldarg_1),

                // new ChangingNicknameEventArgs(player, player.CustomName, value)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingNicknameEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),

                // Handlers.Player.OnChangingNickname(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnChangingNickname))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingNicknameEventArgs), nameof(ChangingNicknameEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, continueLabel),
                new(OpCodes.Pop),
                new(OpCodes.Ret),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingNicknameEventArgs), nameof(ChangingNicknameEventArgs.NewName))).WithLabels(continueLabel),
                new(OpCodes.Starg_S, 1),
            });

            newInstructions.InsertRange(newInstructions.Count - 1, new[]
            {
                // player
                new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),

                // oldName
                new CodeInstruction(OpCodes.Ldloc_S, oldName.LocalIndex),

                // new ChangingNicknameEventArgs(player, oldName)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangedNicknameEventArgs))[0]),

                // Handlers.Player.OnChangedNickname(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnChangedNickname))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}