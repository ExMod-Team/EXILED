// -----------------------------------------------------------------------
// <copyright file="CollisionHandler.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Components
{
    using System;

    using Features;

    using InventorySystem.Items.ThrowableProjectiles;

    using UnityEngine;

    /// <summary>
    /// Collision Handler for grenades.
    /// </summary>
    public class CollisionHandler : MonoBehaviour
    {
        private bool initialized;

        /// <summary>
        /// Gets the thrower of the grenade.
        /// </summary>
        public GameObject Owner { get; private set; }

        /// <summary>
        /// Gets the grenade itself.
        /// </summary>
        public EffectGrenade Grenade { get; private set; }

        /// <summary>
        /// Inits the <see cref="CollisionHandler"/> object.
        /// </summary>
        /// <param name="owner">The grenade owner.</param>
        /// <param name="grenade">The grenade component.</param>
        public void Init(GameObject owner, ThrownProjectile grenade)
        {
            Owner = owner;
            Grenade = (EffectGrenade)grenade;
            initialized = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            try
            {
                if (!initialized)
                    return;
                if (Owner == null || Grenade == null)
                    return;
                if (collision?.collider?.gameObject == Owner)
                    return;
                if (collision.collider.gameObject.TryGetComponent<EffectGrenade>(out _))
                    return;

                Grenade.TargetTime = 0.1f;
            }
            catch (Exception exception)
            {
                Log.Error($"{nameof(OnCollisionEnter)} error:\n{exception}");
                Destroy(this);
            }
        }
    }
}