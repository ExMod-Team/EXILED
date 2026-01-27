// -----------------------------------------------------------------------
// <copyright file="Scp127MaxHs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using Exiled.API.Features.Items;
    using HarmonyLib;
    using InventorySystem.Items.Firearms.Modules.Scp127;

#pragma warning disable SA1313
    /// <summary>
    /// Patches <see cref="Scp127HumeModule.HsMax"/> to implement <see cref="Scp127.HsMax"/> setter.
    /// </summary>
    [HarmonyPatch(typeof(Scp127HumeModule), nameof(Scp127HumeModule.HsMax), MethodType.Getter)]
    internal class Scp127MaxHs
    {
        private static void Postfix(Scp127HumeModule __instance, ref float __result)
        {
            Scp127 item = Item.Get<Scp127>(__instance.ItemSerial);

            if (item == null || !item.CustomHsMax.HasValue)
                return;

            __result = item.CustomHsMax.Value;
            return;
        }
    }
}