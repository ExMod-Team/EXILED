// -----------------------------------------------------------------------
// <copyright file="OpeningGift.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp2536
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Christmas.Scp2536;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp2536;
    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp2536GiftController.ServerInteract"/>
    /// to add <see cref="Handlers.Scp2536.OpeningGift"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp2536), nameof(Handlers.Scp2536.OpeningGift))]
    [HarmonyPatch(typeof(Scp2536GiftController), nameof(Scp2536GiftController.ServerInteract))]
    internal class OpeningGift
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();

            int offset = -3;
            int index = newInstructions.FindLastIndex(x => x.Calls(Method(typeof(Scp2536GiftController), nameof(Scp2536GiftController.RpcSetGiftState)))) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
                {
                    // Player.Get(hub);
                    new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // OpeningGiftEventArgs ev = new(Player, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(OpeningGiftEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Scp2536.OnOppeningGift(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Scp2536), nameof(Handlers.Scp2536.OnOppeningGift))),

                    // if (!ev.IsAllowed)
                    //   goto retLabel;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(OpeningGiftEventArgs), nameof(OpeningGiftEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, retLabel),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}