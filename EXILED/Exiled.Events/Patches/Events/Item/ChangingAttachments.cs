// -----------------------------------------------------------------------
// <copyright file="ChangingAttachments.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Item
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Items;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Item;

    using HarmonyLib;

    using InventorySystem.Items.Firearms.Attachments;

    using Mirror;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches
    /// <see cref="AttachmentsServerHandler.ServerReceiveChangeRequest(NetworkConnection, AttachmentsChangeRequest)" />.
    /// Adds the <see cref="Handlers.Item.ChangingAttachments" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Item), nameof(Handlers.Item.ChangingAttachments))]
    [HarmonyPatch(typeof(AttachmentsServerHandler), nameof(AttachmentsServerHandler.ServerReceiveChangeRequest))]
    internal static class ChangingAttachments
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingAttachmentsEventArgs));

            Label returnLabel = generator.DefineLabel();

            /*
            [] <= Here, right after msg validation.
            IL_0039: ldloc.1      // curInstance
            IL_003a: ldarg.1      // msg
            IL_003b: ldfld        unsigned int32 InventorySystem.Items.Firearms.Attachments.AttachmentsChangeRequest::AttachmentsCode
            IL_0040: ldc.i4.1
            IL_0041: call         void InventorySystem.Items.Firearms.Attachments.AttachmentsUtils::ApplyAttachmentsCode(class InventorySystem.Items.Firearms.Firearm, unsigned int32, bool)
             */
            int index = newInstructions.FindIndex(i => i.IsLdarg(1)) - 1;
            List<Label> jumpLabels = newInstructions[index].ExtractLabels();

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // ev = new ChangingAttachmentsEventArgs(msg);
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingAttachmentsEventArgs))[0]),
                    new(OpCodes.Stloc, ev),

                    // Handlers.Item.OnChangingAttachments(ev);
                    new(OpCodes.Ldloc, ev),
                    new(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnChangingAttachments))),

                    // if (!ev.IsAllowed) return;
                    new(OpCodes.Ldloc, ev),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingAttachmentsEventArgs), nameof(ChangingAttachmentsEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // msg.AttachmentsCode = ev.NewCode;
                    new(OpCodes.Ldarga, 1),
                    new(OpCodes.Ldloc, ev),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingAttachmentsEventArgs), nameof(ChangingAttachmentsEventArgs.NewCode))),
                    new(OpCodes.Stfld, Field(typeof(AttachmentsChangeRequest), nameof(AttachmentsChangeRequest.AttachmentsCode))),
                });

            // Prevent if expression from branching over inserted instructions
            newInstructions[index].WithLabels(jumpLabels);

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}