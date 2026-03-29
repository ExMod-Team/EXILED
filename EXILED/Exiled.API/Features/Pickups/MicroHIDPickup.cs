// -----------------------------------------------------------------------
// <copyright file="MicroHIDPickup.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using Exiled.API.Features.Items;
    using Exiled.API.Interfaces;
    using InventorySystem.Items.MicroHID.Modules;

    using BaseMicroHID = InventorySystem.Items.MicroHID.MicroHIDPickup;

    /// <summary>
    /// A wrapper class for a MicroHID pickup.
    /// </summary>
    public class MicroHIDPickup : Pickup, IWrapper<BaseMicroHID>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroHIDPickup"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="BaseMicroHID"/> class.</param>
        internal MicroHIDPickup(BaseMicroHID pickupBase)
            : base(pickupBase)
        {
            Base = pickupBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroHIDPickup"/> class.
        /// </summary>
        internal MicroHIDPickup()
            : base(ItemType.MicroHID)
        {
            Base = (BaseMicroHID)((Pickup)this).Base;
        }

        /// <summary>
        /// Gets the <see cref="BaseMicroHID"/> that this class is encapsulating.
        /// </summary>
        public new BaseMicroHID Base { get; }

        /// <summary>
        /// Gets the <see cref="InventorySystem.Items.MicroHID.Modules.CycleController"/> of this <see cref="MicroHIDPickup"/>.
        /// </summary>
        public CycleController CycleController => Base._cycleController;

        /// <summary>
        /// Gets or sets the MicroHID Energy Level.
        /// </summary>
        public float Energy
        {
            get => EnergyManagerModule.GetEnergy(Serial);
            set => EnergyManagerModule.SyncEnergy[Serial] = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="MicroHidPhase"/>.
        /// </summary>
        public MicroHidPhase State
        {
            get => CycleController.Phase;
            set => CycleController.Phase = value;
        }

        /// <summary>
        /// Gets or sets progress of winging up.
        /// </summary>
        /// <value>A value between <c>0</c> and <c>1</c>.</value>
        public float WindUpProgress
        {
            get => CycleController.ServerWindUpProgress;
            set => CycleController.ServerWindUpProgress = value;
        }

        /// <summary>
        /// Gets or sets the last received <see cref="MicroHidFiringMode"/>.
        /// </summary>
        public MicroHidFiringMode LastFiringMode
        {
            get => CycleController.LastFiringMode;
            set => CycleController.LastFiringMode = value;
        }

        /// <summary>
        /// Starts firing the MicroHID.
        /// </summary>
        /// <param name="firingMode">Fire mode.</param>
        public void Fire(MicroHidFiringMode firingMode = MicroHidFiringMode.PrimaryFire)
        {
            switch (firingMode)
            {
                case MicroHidFiringMode.PrimaryFire:
                    if (TryGetFireController(MicroHidFiringMode.PrimaryFire, out PrimaryFireModeModule primaryFireModeModule))
                        primaryFireModeModule.ServerFire();
                    break;
                case MicroHidFiringMode.ChargeFire:
                    if (TryGetFireController(MicroHidFiringMode.ChargeFire, out ChargeFireModeModule chargeFireModeModule))
                        chargeFireModeModule.ServerFire();
                    break;
                default:
                    if (TryGetFireController(MicroHidFiringMode.BrokenFire, out BrokenFireModeModule brokenFireModeModule))
                        brokenFireModeModule.ServerFire();
                    break;
            }
        }

        /// <summary>
        /// Explodes the MicroHID.
        /// </summary>
        public void Explode()
        {
            if (TryGetFireController(MicroHidFiringMode.ChargeFire, out ChargeFireModeModule module))
                module.ServerExplode();
        }

        /// <summary>
        /// Tries to get a <see cref="FiringModeControllerModule"/> assosiated with the specified <see cref="MicroHidFiringMode"/>.
        /// </summary>
        /// <param name="firingMode">Target firing mode.</param>
        /// <param name="module">Found module or <c>null</c>.</param>
        /// <typeparam name="T">Type of module.</typeparam>
        /// <returns><c>true</c> if module was found, <c>false</c> otherwise.</returns>
        public bool TryGetFireController<T>(MicroHidFiringMode firingMode, out T module)
            where T : FiringModeControllerModule
        {
            if (CycleController._firingModeControllers.Count == 0)
            {
                module = null;
                return false;
            }

            module = (T)CycleController._firingModeControllers.Find(x => x.AssignedMode == firingMode);
            return module != null;
        }

        /// <summary>
        /// Tries to get a <see cref="FiringModeControllerModule"/> assosiated with the last <see cref="MicroHidFiringMode"/>.
        /// </summary>
        /// <param name="module">Found module or <c>null</c>.</param>
        /// <returns><c>true</c> if module was found, <c>false</c> otherwise.</returns>
        public bool TryGetLastFireController(out FiringModeControllerModule module) => TryGetFireController(LastFiringMode, out module);

        /// <summary>
        /// Returns the MicroHIDPickup in a human readable format.
        /// </summary>
        /// <returns>A string containing MicroHIDPickup related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Energy}|";
    }
}
