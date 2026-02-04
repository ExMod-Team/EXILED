// -----------------------------------------------------------------------
// <copyright file="Item.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
#pragma warning disable SA1623 // Property summary documentation should match accessors

    using Exiled.Events.EventArgs.Item;

    using Exiled.Events.Features;

    /// <summary>
    /// Item related events.
    /// </summary>
    public static class Item
    {
        /// <summary>
        /// Invoked before the ammo of an firearm are changed.
        /// </summary>
        public static Event<ChangingAmmoEventArgs> ChangingAmmo { get; set; } = new ();

        /// <summary>
        /// Invoked before item attachments are changed.
        /// </summary>
        public static Event<ChangingAttachmentsEventArgs> ChangingAttachments { get; set; } = new();

        /// <summary>
        /// Invoked before receiving a preference.
        /// </summary>
        public static Event<ReceivingPreferenceEventArgs> ReceivingPreference { get; set; } = new();

        /// <summary>
        /// Invoked before a keycard interacts with a door.
        /// </summary>
        public static Event<KeycardInteractingEventArgs> KeycardInteracting { get; set; } = new();

        /// <summary>
        /// Invoked before a melee item is swung.
        /// </summary>
        public static Event<SwingingEventArgs> Swinging { get; set; } = new();

        /// <summary>
        /// Invoked when a <see cref="API.Features.Items.Jailbird"/> starts charging.
        /// </summary>
        public static Event<ChargingJailbirdEventArgs> ChargingJailbird { get; set; } = new();

        /// <summary>
        /// Invoked after a <see cref="API.Features.Items.Jailbird"/> finishes charging.
        /// </summary>
        public static Event<JailbirdChargeCompleteEventArgs> JailbirdChargeComplete { get; set; } = new();

        /// <summary>
        /// Invoked before a radio pickup is draining battery.
        /// </summary>
        public static Event<UsingRadioPickupBatteryEventArgs> UsingRadioPickupBattery { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Pickups.MicroHIDPickup"/> state is changed.
        /// </summary>
        public static Event<ChangingMicroHIDPickupStateEventArgs> ChangingMicroHIDPickupState { get; set; } = new();

        /// <summary>
        /// Invoked before item inspection is started.
        /// </summary>
        public static Event<InspectingItemEventArgs> InspectingItem { get; set; } = new();

        /// <summary>
        /// Invoked after item inspection is started.
        /// </summary>
        public static Event<InspectedItemEventArgs> InspectedItem { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="ItemType.ParticleDisruptor"/> firing while on the ground.
        /// <remarks>The client will still see all effects, like sounds and shoot.</remarks>
        /// </summary>
        public static Event<DisruptorFiringEventArgs> DisruptorFiring { get; set; } = new();

        /// <summary>
        /// Invoked before the Jailbird's <see cref="InventorySystem.Items.Jailbird.JailbirdWearState"/> is changed.
        /// </summary>
        public static Event<JailbirdChangingWearStateEventArgs> JailbirdChangingWearState { get; set; } = new();

        /// <summary>
        /// Invoked after the Jailbird's <see cref="InventorySystem.Items.Jailbird.JailbirdWearState"/> is changed.
        /// </summary>
        public static Event<JailbirdChangedWearStateEventArgs> JailbirdChangedWearState { get; set; } = new();

        /// <summary>
        /// Invoked before a marshmallow man punches.
        /// </summary>
        public static Event<PunchingEventArgs> Punching { get; set; } = new();

        /// <summary>
        /// Invoked before a marshmallow man cackles.
        /// </summary>
        public static Event<CacklingEventArgs> Cackling { get; set; } = new();

        /// <summary>
        /// Called before the Jailbird's <see cref="InventorySystem.Items.Jailbird.JailbirdWearState"/> is changed.
        /// </summary>
        /// <param name="ev">The <see cref="JailbirdChangingWearStateEventArgs"/> instance.</param>
        public static void OnJailbirdStateChanging(JailbirdChangingWearStateEventArgs ev) => JailbirdChangingWearState.InvokeSafely(ev);

        /// <summary>
        /// Called after the Jailbird's <see cref="InventorySystem.Items.Jailbird.JailbirdWearState"/> is changed.
        /// </summary>
        /// <param name="ev">The <see cref="JailbirdChangedWearStateEventArgs"/> instance.</param>
        public static void OnJailbirdStateChanged(JailbirdChangedWearStateEventArgs ev) => JailbirdChangedWearState.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="ItemType.ParticleDisruptor"/> firing while on the ground.
        /// WARNING: Client still receive the shoot sound AND the ammo is still removed. (even if <see cref="DisruptorFiringEventArgs.IsAllowed"/> = false).
        /// </summary>
        /// <param name="ev">The <see cref="DisruptorFiringEventArgs"/> instance.</param>
        public static void OnDisruptorFiring(DisruptorFiringEventArgs ev) => DisruptorFiring.InvokeSafely(ev);

        /// <summary>
        /// Called before the ammo of an firearm is changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingAmmoEventArgs"/> instance.</param>
        public static void OnChangingAmmo(ChangingAmmoEventArgs ev) => ChangingAmmo.InvokeSafely(ev);

        /// <summary>
        /// Called before item attachments are changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingAttachmentsEventArgs" /> instance.</param>
        public static void OnChangingAttachments(ChangingAttachmentsEventArgs ev) => ChangingAttachments.InvokeSafely(ev);

        /// <summary>
        /// Called before receiving a preference.
        /// </summary>
        /// <param name="ev">The <see cref="ReceivingPreferenceEventArgs" /> instance.</param>
        public static void OnReceivingPreference(ReceivingPreferenceEventArgs ev) => ReceivingPreference.InvokeSafely(ev);

        /// <summary>
        /// Called before keycard interacts with a door.
        /// </summary>
        /// <param name="ev">The <see cref="KeycardInteractingEventArgs"/> instance.</param>
        public static void OnKeycardInteracting(KeycardInteractingEventArgs ev) => KeycardInteracting.InvokeSafely(ev);

        /// <summary>
        /// Called before a melee item is swung.
        /// </summary>
        /// <param name="ev">The <see cref="SwingingEventArgs"/> instance.</param>
        public static void OnSwinging(SwingingEventArgs ev) => Swinging.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Items.Jailbird"/> that is being charged.
        /// </summary>
        /// <param name="ev">The <see cref="ChargingJailbirdEventArgs"/> instance.</param>
        public static void OnChargingJailbird(ChargingJailbirdEventArgs ev) => ChargingJailbird.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Items.Jailbird"/> finish charging.
        /// </summary>
        /// <param name="ev">The <see cref="ChargingJailbirdEventArgs"/> instance.</param>
        public static void OnJailbirdChargeComplete(JailbirdChargeCompleteEventArgs ev) => JailbirdChargeComplete.InvokeSafely(ev);

        /// <summary>
        /// Called before radio pickup is draining battery.
        /// </summary>
        /// <param name="ev">The <see cref="UsingRadioPickupBatteryEventArgs"/> instance.</param>
        public static void OnUsingRadioPickupBattery(UsingRadioPickupBatteryEventArgs ev) => UsingRadioPickupBattery.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Pickups.MicroHIDPickup"/> state is changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingMicroHIDPickupStateEventArgs"/> instance.</param>
        public static void OnChangingMicroHIDPickupState(ChangingMicroHIDPickupStateEventArgs ev) => ChangingMicroHIDPickupState.InvokeSafely(ev);

        /// <summary>
        /// Called before item inspection is started.
        /// </summary>
        /// <param name="ev">The <see cref="InspectingItemEventArgs"/> instance.</param>
        public static void OnInspectingItem(InspectingItemEventArgs ev) => InspectingItem.InvokeSafely(ev);

        /// <summary>
        /// Called before item inspection is started.
        /// </summary>
        /// <param name="ev">The <see cref="InspectedItemEventArgs"/> instance.</param>
        public static void OnInspectedItem(InspectedItemEventArgs ev) => InspectedItem.InvokeSafely(ev);

        /// <summary>
        /// Called before a marshmallow man punches.
        /// </summary>
        /// <param name="ev">The <see cref="PunchingEventArgs"/> instance.</param>
        public static void OnPunching(PunchingEventArgs ev) => Punching.InvokeSafely(ev);

        /// <summary>
        /// Called before a marshmallow man cackles.
        /// </summary>
        /// <param name="ev">The <see cref="CacklingEventArgs"/> instance.</param>
        public static void OnCackling(CacklingEventArgs ev) => Cackling.InvokeSafely(ev);
    }
}