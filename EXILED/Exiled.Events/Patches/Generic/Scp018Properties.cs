// -----------------------------------------------------------------------
// <copyright file="Scp018Properties.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic.Scp079API
{
#pragma warning disable SA1313
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;

    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Pickups.Projectiles;

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="InventorySystem.Items.ThrowableProjectiles.Scp018Projectile.SetupModule"/>.
    /// Adds the <see cref="Scp018Projectile.VelocityPerBounce" /> and <see cref="Scp018Projectile.MaxVelocity" /> support.
    /// </summary>
    [HarmonyPatch(typeof(InventorySystem.Items.ThrowableProjectiles.Scp018Projectile), nameof(InventorySystem.Items.ThrowableProjectiles.Scp018Projectile.SetupModule))]
    internal class Scp018Properties
    {
        private static void Postfix(InventorySystem.Items.ThrowableProjectiles.Scp018Projectile __instance)
        {
            Scp018Projectile projectile = Pickup.Get<Scp018Projectile>(__instance);

            projectile.SetupProjectile();
        }
    }
}
