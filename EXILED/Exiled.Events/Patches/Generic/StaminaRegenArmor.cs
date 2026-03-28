// -----------------------------------------------------------------------
// <copyright file="StaminaRegenArmor.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
#pragma warning disable SA1313

    using HarmonyLib;
    using InventorySystem.Items.Armor;

    /// <summary>
    /// Patches <see cref="BodyArmor.StaminaRegenMultiplier"/>.
    /// Implements <see cref="API.Features.Items.Armor.StaminaRegenMultiplier"/>.
    /// </summary>
    [HarmonyPatch(typeof(BodyArmor), nameof(BodyArmor.StaminaRegenMultiplier), MethodType.Getter)]
    internal class StaminaRegenArmor
    {
        private static void Postfix(BodyArmor __instance, ref float __result)
        {
            if(Item.Get(__instance) is Armor armor)
                __result *= armor.StaminaRegenMultiplier;
        }
    }
}