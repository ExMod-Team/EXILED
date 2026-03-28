// -----------------------------------------------------------------------
// <copyright file="GrantingGift.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp2536
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Christmas.Scp2536;
    using Christmas.Scp2536.Gifts;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp2536;
    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp2536GiftController.ServerGrantRandomGift"/>
    /// to add <see cref="Handlers.Scp2536.GrantingGift"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp2536), nameof(Handlers.Scp2536.GrantingGift))]
    [HarmonyPatch(typeof(Scp2536GiftController), nameof(Scp2536GiftController.ServerGrantRandomGift))]
    internal class GrantingGift
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(GrantingGiftEventArgs));
            LocalBuilder naughty = generator.DeclareLocal(typeof(Naughty));

            int offset = -1;
            int index = newInstructions.FindLastIndex(x => x.LoadsField(Field(typeof(Scp2536GiftBase), nameof(Scp2536GiftBase.ObtainedBy)))) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Player.Get(hub);
                new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // gift
                new(OpCodes.Ldloc_2),

                // true
                new(OpCodes.Ldc_I4_1),

                // GrantingGiftEventArgs ev = new(Player, Scp2536GiftBase, true);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(GrantingGiftEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Scp2536.OnGrantingGift(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Scp2536), nameof(Handlers.Scp2536.OnGrantingGift))),

                // if (!ev.IsAllowed)
                //   goto retLabel;
                new(OpCodes.Callvirt, PropertyGetter(typeof(GrantingGiftEventArgs), nameof(GrantingGiftEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),

                // gift = ev.Gift;
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(GrantingGiftEventArgs), nameof(GrantingGiftEventArgs.Gift))),
                new(OpCodes.Stloc_2),
            });

            offset = 1;
            index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(Scp2536GiftController), nameof(Scp2536GiftController.ServerGetGift), null, new[] { typeof(Naughty) }))) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // naughty is already on the stack
                // naughty = this.ServerGetGift<Naught>();
                new(OpCodes.Stloc_S, naughty),

                // Player.Get(hub);
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // naughty
                new(OpCodes.Ldloc_S, naughty),

                // true
                new(OpCodes.Ldc_I4_1),

                // GrantingGiftEventArgs ev = new(Player, Scp2536GiftBase, true);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(GrantingGiftEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Scp2536.OnGrantingGift(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Scp2536), nameof(Handlers.Scp2536.OnGrantingGift))),

                // if (!ev.IsAllowed)
                //   goto retLabel;
                new(OpCodes.Callvirt, PropertyGetter(typeof(GrantingGiftEventArgs), nameof(GrantingGiftEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),

                // load naughty onto stack
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(GrantingGiftEventArgs), nameof(GrantingGiftEventArgs.Gift))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}