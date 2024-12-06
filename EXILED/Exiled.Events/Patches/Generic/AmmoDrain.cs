// -----------------------------------------------------------------------
// <copyright file="AmmoDrain.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1402
#pragma warning disable SA1649
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pools;

    using HarmonyLib;

    using InventorySystem.Items;
    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.Modules;

    using MapGeneration;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch for adding <see cref="API.Features.Doors.AirlockController"/> to list.
    /// </summary>
    [HarmonyPatch(typeof(AutomaticActionModule))]
    internal class AmmoDrainAutomatic
    {
        /// <summary>
        /// Constructs instructions for modifying ammo by <see cref="Exiled.API.Features.Items.Firearm.AmmoDrain"/>.
        /// </summary>
        /// <param name="firearm">Firearm local variable.</param>
        /// <param name="ammoDrain">AmmoDrain local variable..</param>
        /// <param name="continueLabel">Label for help.</param>
        /// <returns>Instructions for <see cref="Exiled.API.Features.Items.Firearm.AmmoDrain"/>.</returns>
        internal static IEnumerable<CodeInstruction> GetInstructions(LocalBuilder firearm, LocalBuilder ammoDrain, Label continueLabel)
        {
            yield return new CodeInstruction(OpCodes.Ldc_I4_1);
            yield return new CodeInstruction(OpCodes.Stloc_S, firearm.LocalIndex);

            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(FirearmSubcomponentBase), nameof(FirearmSubcomponentBase.Firearm)));
            yield return new CodeInstruction(OpCodes.Call, GetDeclaredMethods(typeof(Item)).First(x => !x.IsGenericMethod && x.Name is nameof(Item.Get) && x.GetParameters().Length is 1 && x.GetParameters()[0].ParameterType == typeof(ItemBase)));
            yield return new CodeInstruction(OpCodes.Isinst, typeof(API.Features.Items.Firearm));
            yield return new CodeInstruction(OpCodes.Dup);
            yield return new CodeInstruction(OpCodes.Stloc_S, firearm.LocalIndex);
            yield return new CodeInstruction(OpCodes.Brfalse_S, continueLabel);

            yield return new CodeInstruction(OpCodes.Ldloc_S, firearm.LocalIndex);
            yield return new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Items.Firearm), nameof(API.Features.Items.Firearm.AmmoDrain)));
            yield return new CodeInstruction(OpCodes.Stloc_S, ammoDrain.LocalIndex);

            yield return new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel);
        }

        [HarmonyPatch(nameof(AutomaticActionModule.ServerShoot))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> First(IEnumerable<CodeInstruction> codeInstructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(codeInstructions);

            LocalBuilder firearm = generator.DeclareLocal(typeof(API.Features.Items.Firearm));
            LocalBuilder ammoDrain = generator.DeclareLocal(typeof(int));

            Label cnt = generator.DefineLabel();

            // if (this is Scp079Camera camera)
            //     Room.RoomIdentifierToRoom[Room].CamerasValue.Add(new Camera(camera));
            newInstructions.InsertRange(
                0,
                GetInstructions(firearm, ammoDrain, cnt));

            int offset = 1;
            int index = newInstructions.FindIndex(i => i.Calls(PropertyGetter(typeof(AutomaticActionModule), nameof(AutomaticActionModule.AmmoStored)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldloc_S, ammoDrain.LocalIndex),
                    new(OpCodes.Div),
                });

            offset = 0;
            index = newInstructions.FindIndex(i => i.Calls(Method(typeof(IPrimaryAmmoContainerModule), nameof(IPrimaryAmmoContainerModule.ServerModifyAmmo)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldloc_S, ammoDrain.LocalIndex),
                    new(OpCodes.Mul),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        [HarmonyPatch(nameof(AutomaticActionModule.ServerCycleAction))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Second(IEnumerable<CodeInstruction> codeInstructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(codeInstructions);

            LocalBuilder firearm = generator.DeclareLocal(typeof(API.Features.Items.Firearm));
            LocalBuilder ammoDrain = generator.DeclareLocal(typeof(int));

            Label cnt = generator.DefineLabel();
            newInstructions.InsertRange(
                0,
                GetInstructions(firearm, ammoDrain, cnt));

            int offset = 1;
            int index = newInstructions.FindIndex(i => i.Calls(PropertyGetter(typeof(AutomaticActionModule), nameof(AutomaticActionModule.AmmoStored)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldloc_S, ammoDrain.LocalIndex),
                    new(OpCodes.Div),
                });

            offset = 0;
            index = newInstructions.FindIndex(i => i.Calls(Method(typeof(IPrimaryAmmoContainerModule), nameof(IPrimaryAmmoContainerModule.ServerModifyAmmo)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldloc_S, ammoDrain.LocalIndex),
                    new(OpCodes.Mul),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patch for adding <see cref="API.Features.Doors.AirlockController"/> to list.
    /// </summary>
    [HarmonyPatch(typeof(PumpActionModule), nameof(PumpActionModule.Pump))]
    internal class AmmoDrainPump
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(codeInstructions);

            LocalBuilder firearm = generator.DeclareLocal(typeof(API.Features.Items.Firearm));
            LocalBuilder ammoDrain = generator.DeclareLocal(typeof(int));

            Label cnt = generator.DefineLabel();

            newInstructions.InsertRange(
                0,
                AmmoDrainAutomatic.GetInstructions(firearm, ammoDrain, cnt));

            int offset = 1;
            int index = newInstructions.FindLastIndex(i => i.Calls(PropertyGetter(typeof(PumpActionModule), nameof(PumpActionModule.AmmoStored)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldloc_S, ammoDrain.LocalIndex),
                    new(OpCodes.Div),
                });

            offset = 0;
            index = newInstructions.FindIndex(i => i.Calls(Method(typeof(IPrimaryAmmoContainerModule), nameof(IPrimaryAmmoContainerModule.ServerModifyAmmo)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldloc_S, ammoDrain.LocalIndex),
                    new(OpCodes.Mul),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patch for adding <see cref="API.Features.Doors.AirlockController"/> to list.
    /// </summary>
    [HarmonyPatch(typeof(RevolverClipReloaderModule), nameof(RevolverClipReloaderModule.InsertAmmoFromClip))]
    internal class AmmoDrainRevolver
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(codeInstructions);

            LocalBuilder firearm = generator.DeclareLocal(typeof(API.Features.Items.Firearm));
            LocalBuilder ammoDrain = generator.DeclareLocal(typeof(int));

            Label cnt = generator.DefineLabel();

            newInstructions.InsertRange(
                0,
                AmmoDrainAutomatic.GetInstructions(firearm, ammoDrain, cnt));

            int offset = 1;
            int index = newInstructions.FindIndex(i => i.Calls(PropertyGetter(typeof(IAmmoContainerModule), nameof(IAmmoContainerModule.AmmoMax)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldloc_S, ammoDrain.LocalIndex),
                    new(OpCodes.Mul),
                });

            offset = 0;
            index = newInstructions.FindIndex(i => i.Calls(Method(typeof(IPrimaryAmmoContainerModule), nameof(IPrimaryAmmoContainerModule.ServerModifyAmmo)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldloc_S, ammoDrain.LocalIndex),
                    new(OpCodes.Div),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}