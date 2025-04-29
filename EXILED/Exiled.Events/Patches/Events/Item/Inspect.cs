// -----------------------------------------------------------------------
// <copyright file="Inspect.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Item
{
#pragma warning disable SA1402
#pragma warning disable SA1649
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Item;
    using HarmonyLib;
    using InventorySystem.Items.Firearms.Modules;
    using InventorySystem.Items.Jailbird;
    using InventorySystem.Items.Keycards;
    using InventorySystem.Items.MicroHID.Modules;
    using InventorySystem.Items.Usables.Scp1344;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="SimpleInspectorModule.ServerProcessCmd"/>
    /// to add <see cref="Handlers.Item.InspectingItem"/> and <see cref="Handlers.Item.InspectedItem"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Item), nameof(Handlers.Item.InspectingItem))]
    [EventPatch(typeof(Handlers.Item), nameof(Handlers.Item.InspectedItem))]
    [HarmonyPatch(typeof(SimpleInspectorModule), nameof(SimpleInspectorModule.ServerProcessCmd))]
    internal class InspectWeapon
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldarg_0);

            newInstructions.InsertRange(index, new[]
            {
                // this.Firearm
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Callvirt, PropertyGetter(typeof(SimpleInspectorModule), nameof(SimpleInspectorModule.Firearm))),

                // true
                new(OpCodes.Ldc_I4_1),

                // InspectingItemEventArgs ev = new(this.Firearm)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InspectingItemEventArgs))[0]),

                // Handlers.Item.OnInspectingItem(ev)
                new(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnInspectingItem))),
            });

            newInstructions.InsertRange(newInstructions.Count - 1, new CodeInstruction[]
            {
                // this.Firearm
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(SimpleInspectorModule), nameof(SimpleInspectorModule.Firearm))),

                // InspectedItemEventArgs ev = new(this.Firearm)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InspectedItemEventArgs))[0]),

                // Handlers.Item.OnInspectedItem(ev)
                new(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnInspectedItem))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="Scp1344NetworkHandler.TryInspect"/>
    /// to add <see cref="Handlers.Item.InspectingItem"/> and <see cref="Handlers.Item.InspectedItem"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Item), nameof(Handlers.Item.InspectingItem))]
    [EventPatch(typeof(Handlers.Item), nameof(Handlers.Item.InspectedItem))]
    [HarmonyPatch(typeof(Scp1344NetworkHandler), nameof(Scp1344NetworkHandler.TryInspect))]
    internal class InspectScp1344
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = 1;
            int index = newInstructions.FindIndex(x => x.Is(OpCodes.Callvirt, PropertyGetter(typeof(Scp1344Item), nameof(Scp1344Item.AllowInspect)))) + offset;

            LocalBuilder allowed = generator.DeclareLocal(typeof(bool));

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // store isAllowed
                new(OpCodes.Stloc_S, allowed.LocalIndex),

                // curInstance
                new(OpCodes.Ldloc_0),

                // load isAllowed
                new(OpCodes.Ldloc_S, allowed.LocalIndex),

                // InspectingItemEventArgs ev = new(curInstance, isAllowed);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InspectingItemEventArgs))[0]),
                new(OpCodes.Dup),

                // Handlers.Item.OnInspectingItem(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnInspectingItem))),

                // load isAllowed
                new(OpCodes.Callvirt, PropertyGetter(typeof(InspectingItemEventArgs), nameof(InspectingItemEventArgs.IsAllowed))),
            });

            newInstructions.InsertRange(newInstructions.Count - 1, new CodeInstruction[]
            {
                // curInstance
                new(OpCodes.Ldloc_0),

                // InspectedItemEventArgs = new(curInstance);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InspectedItemEventArgs))[0]),

                // Handlers.Item.OnInspectedItem(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnInspectedItem))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="KeycardItem.ServerProcessCmd"/>
    /// to add <see cref="Handlers.Item.InspectingItem"/> and <see cref="Handlers.Item.InspectedItem"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Item), nameof(Handlers.Item.InspectingItem))]
    [EventPatch(typeof(Handlers.Item), nameof(Handlers.Item.InspectedItem))]
    [HarmonyPatch(typeof(KeycardItem), nameof(KeycardItem.ServerProcessCmd))]
    internal class InspectKeycard
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = 1;
            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldloc_1) + offset;

            LocalBuilder allowed = generator.DeclareLocal(typeof(bool));

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // save flag value as isAllowed
                new(OpCodes.Stloc_S, allowed.LocalIndex),

                // this
                new(OpCodes.Ldarg_0),

                // load isAllowed
                new(OpCodes.Ldloc_S, allowed.LocalIndex),

                // InspectingItemEventArgs ev = new(this, isAllowed)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InspectingItemEventArgs))[0]),
                new(OpCodes.Dup),

                // Handlers.Item.OnInspectingItem(ev)
                new(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnInspectingItem))),

                // load isAllowed
                new(OpCodes.Callvirt, PropertyGetter(typeof(InspectingItemEventArgs), nameof(InspectingItemEventArgs.IsAllowed))),
            });

            newInstructions.InsertRange(newInstructions.Count - 1, new CodeInstruction[]
            {
                // this
                new(OpCodes.Ldarg_0),

                // InspectedItemEventArgs ev = new(this)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InspectedItemEventArgs))[0]),

                // Handlers.Item.OnInspectedItem(ev)
                new(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnInspectedItem))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void Return(KeycardItem item) => item.ServerSendPrivateRpc(x => KeycardItem.WriteInspect(x, false));
    }

    /// <summary>
    /// Patches <see cref="JailbirdItem.ServerProcessCmd"/>
    /// to add <see cref="Handlers.Item.InspectingItem"/> and <see cref="Handlers.Item.InspectedItem"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Item), nameof(Handlers.Item.InspectingItem))]
    [EventPatch(typeof(Handlers.Item), nameof(Handlers.Item.InspectedItem))]
    [HarmonyPatch(typeof(JailbirdItem), nameof(JailbirdItem.ServerProcessCmd))]
    internal class InspectJailbird
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = 1;
            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldloc_2) + offset;

            LocalBuilder allowed = generator.DeclareLocal(typeof(bool));

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // save flag value as isAllowed
                new(OpCodes.Stloc_S, allowed.LocalIndex),

                // this
                new(OpCodes.Ldarg_0),

                // load isAllowed
                new(OpCodes.Ldloc_S, allowed.LocalIndex),

                // InspectingItemEventArgs ev = new(this, isAllowed)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InspectingItemEventArgs))[0]),
                new(OpCodes.Dup),

                // Handlers.Item.OnInspectingItem(ev)
                new(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnInspectingItem))),

                // load isAllowed
                new(OpCodes.Callvirt, PropertyGetter(typeof(InspectingItemEventArgs), nameof(InspectingItemEventArgs.IsAllowed))),
            });

            index = newInstructions.FindLastIndex(x => x.Calls(Method(typeof(JailbirdItem), nameof(JailbirdItem.SendRpc)))) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // this
                new(OpCodes.Ldarg_0),

                // InspectedItemEventArgs ev = new(this)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InspectedItemEventArgs))[0]),

                // Handlers.Item.OnInspectedItem(ev)
                new(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnInspectedItem))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="DrawAndInspectorModule.ServerProcessCmd"/>
    /// to add <see cref="Handlers.Item.InspectingItem"/> and <see cref="Handlers.Item.InspectedItem"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Item), nameof(Handlers.Item.InspectingItem))]
    [EventPatch(typeof(Handlers.Item), nameof(Handlers.Item.InspectedItem))]
    [HarmonyPatch(typeof(DrawAndInspectorModule), nameof(DrawAndInspectorModule.ServerProcessCmd))]
    internal class InspectMicroHid
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldarg_0);

            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                // this.MicroHid
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Callvirt, PropertyGetter(typeof(DrawAndInspectorModule), nameof(DrawAndInspectorModule.MicroHid))),

                // true
                new(OpCodes.Ldc_I4_1),

                // InspectingItemEventArgs ev = new(this.MicroHid, true)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InspectingItemEventArgs))[0]),
                new(OpCodes.Dup),

                // Handlers.Item.OnInspectingItem(ev)
                new(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnInspectingItem))),

                // if (!ev.IsAllowed)
                //     return
                new(OpCodes.Callvirt, PropertyGetter(typeof(InspectingItemEventArgs), nameof(InspectingItemEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),
            });

            newInstructions.InsertRange(newInstructions.Count - 1, new CodeInstruction[]
            {
                // this.MicroHid
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(DrawAndInspectorModule), nameof(DrawAndInspectorModule.MicroHid))),

                // InspectedItemEventArgs = new(this.MicroHid)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InspectedItemEventArgs))[0]),

                // Handlers.Item.OnInspectedItem(ev)
                new(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnInspectedItem))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}