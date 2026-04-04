// -----------------------------------------------------------------------
// <copyright file="EffectType.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using System;

    using Exiled.API.Extensions;

    /// <summary>
    /// Status effects as enum.
    /// </summary>
    /// <seealso cref="EffectTypeExtension.TryGetEffectType(CustomPlayerEffects.StatusEffectBase, out EffectType)"/>
    /// <seealso cref="EffectTypeExtension.TryGetType(EffectType, out Type)"/>
    public enum EffectType
    {
        /// <summary>
        /// This EffectType do not exist it's only use when not found or error.
        /// </summary>
        None,

        /// <summary>
        /// Prevents the affected player from reloading weapons and using medical items.
        /// </summary>
        AmnesiaItems,

        /// <summary>
        /// Makes SCP-939 invisible to players under its effect. Visibility is temporarily restored when SCP-939 takes damage or attacks.
        /// </summary>
        AmnesiaVision,

        /// <summary>
        /// Drains the affected player's stamina and then health.
        /// </summary>
        Asphyxiated,

        /// <summary>
        /// Damages the affected player over time.
        /// </summary>
        Bleeding,

        /// <summary>
        /// Blurs the affected player's screen.
        /// </summary>
        Blinded,

        /// <summary>
        /// Increases damage the affected player receives. Does not apply any standalone damage.
        /// </summary>
        Burned,

        /// <summary>
        /// Blurs the affected player's screen while rotating.
        /// </summary>
        Concussed,

        /// <summary>
        /// Effect given to the affected player after being hurt by SCP-106.
        /// </summary>
        Corroding,

        /// <summary>
        /// Muffles the affected player's audio. Does not scale with intensity.
        /// </summary>
        Deafened,

        /// <summary>
        /// Removes 10% of the affected player's health per second.
        /// </summary>
        Decontaminating,

        /// <summary>
        /// Slows down the affected player's movement.
        /// </summary>
        Disabled,

        /// <summary>
        /// Prevents the affected player from moving.
        /// </summary>
        Ensnared,

        /// <summary>
        /// Halves the affected player's maximum stamina and stamina regeneration rate.
        /// </summary>
        Exhausted,

        /// <summary>
        /// Flashes the affected player.
        /// </summary>
        Flashed,

        /// <summary>
        /// Drains the affected player's health while sprinting.
        /// </summary>
        Hemorrhage,

        /// <summary>
        /// Increases the affected player's FOV very slightly, gives infinite stamina and gives the effect of underwater sound.
        /// </summary>
        Invigorated,

        /// <summary>
        /// Reduces the affected player's damage taken by body shots.
        /// </summary>
        BodyshotReduction,

        /// <summary>
        /// Damages the affected player every 5 seconds, starting low and increasing over time, capping out at 20 hp every 5 seconds.
        /// </summary>
        Poisoned,

        /// <summary>
        /// Increases the speed of the affected player while also draining health, dependent on how fast the player is moving.
        /// </summary>
        Scp207,

        /// <summary>
        /// Makes the affected player invisible.
        /// </summary>
        Invisible,

        /// <summary>
        /// Slows the affected player's movement speed, adds vignette, and makes the affected player's footsteps the same as SCP106's.
        /// </summary>
        SinkHole,

        /// <summary>
        /// Reduces the affected player's overall damage taken.
        /// </summary>
        DamageReduction,

        /// <summary>
        /// Increases the affected player's movement speed.
        /// </summary>
        MovementBoost,

        /// <summary>
        /// Reduces the severity of the affected player's negative effects.
        /// </summary>
        RainbowTaste,

        /// <summary>
        /// Drops the affected player's current item, disables interaction with objects, spawns hands that drop to the floor, and deals damage while effect is active.
        /// </summary>
        SeveredHands,

        /// <summary>
        /// Prevents the affected player from sprinting, plays a sound alongside their every footstep, reduces movement speed by 20%.
        /// </summary>
        Stained,

        /// <summary>
        /// Causes the affected player to gain immunity to certain negative status effects.
        /// </summary>
        Vitality,

        /// <summary>
        /// Cause the affected player to slowly take damage, reduces bullet accuracy, applies a blue vignette, plays a sound effect spanning the entire effect's length, and increases item pickup time.
        /// </summary>
        Hypothermia,

        /// <summary>
        /// Increases the affected player's motor function, causing the affected player to reduce the weapon draw time, reload spead, item pickup speed, and medical item usage.
        /// </summary>
        Scp1853,

        /// <summary>
        /// Effect given to a player after being hurt by SCP-049. Deals 8 damage per second, after an inital 16 damage for the first second.
        /// </summary>
        CardiacArrest,

        /// <summary>
        /// Cause the lighting in the facility to dim heavily for the player.
        /// </summary>
        InsufficientLighting,

        /// <summary>
        /// Disable ambient sound.
        /// </summary>
        SoundtrackMute,

        /// <summary>
        /// Protects the affected player from enemy damage if the config is enabled.
        /// </summary>
        SpawnProtected,

        /// <summary>
        /// All players with the Scp106 role will be able to see the affected player whilst stalking. Causes the affected player's screens to become monochromatic when seeing Scp106. The affected player is instantly killed if attacked by Scp106.
        /// </summary>
        Traumatized,

        /// <summary>
        /// Slows the affected player, provides passive health regeneration and passive AHP gain up to 75, and can save the affected player from fatal damage once per effect.
        /// </summary>
        AntiScp207,

        /// <summary>
        /// The effect applied by SCP-079's breach scanner. Mutes the affected player's soundtrack.
        /// </summary>
        Scanned,

        /// <summary>
        /// Teleports the affected player to the pocket dimension and drains their health until the affected player escapes the pocket dimension or is killed. The amount of damage recieved increases the longer the effect is applied.
        /// </summary>
        PocketCorroding,

        /// <summary>
        /// Reduces the affected player's own movement sounds by 10% per intensity level.
        /// </summary>
        SilentWalk,

        /// <summary>
        /// Makes you a marshmallow guy.
        /// </summary>
        // [Obsolete("Not functional in-game")]
        Marshmallow,

        /// <summary>
        /// The effect that is given to the player while getting attacked by SCP-3114's Strangle ability.
        /// </summary>
        Strangled,

        /// <summary>
        /// Allows the affected player to pass through doors.
        /// </summary>
        Ghostly,

        /// <summary>
        /// Manipulate which fog type the affected player will have.
        /// <remarks>You can choose fog with <see cref="CustomRendering.FogType"/> and putting it on intensity.</remarks>
        /// </summary>
        FogControl,

        /// <summary>
        /// Slows the affected player down by 1% per intensity.
        /// </summary>
        Slowness,

        /// <summary>
        /// Allows the affected player to see other players through walls, with a slight delay between spurts of viewability.
        /// </summary>
        Scp1344,

        /// <summary>
        /// Does not blind the affected player. Spawns eyeballs that drop to the floor, and does 10 damage per second.
        /// </summary>
        SeveredEyes,

        /// <summary>
        /// Immediately kills the affected player with death message "Fatal blunt trauma; the body is badly mutilated and pupled.", and "Reason: Crushed" through console.
        /// </summary>
        PitDeath,

        /// <summary>
        /// Blurs the affected player's vision. Does not scale with intensity.
        /// </summary>
        Blurred,

        /// <summary>
        /// Makes the affected player a flamingo <see cref="CustomPlayerEffects.BecomingFlamingo"/>.
        /// </summary>
        [Obsolete("Only availaible for Christmas and AprilFools.")]
        BecomingFlamingo,

        /// <summary>
        /// Makes the affected player a Child after eating Cake <see cref="Scp559Effect"/>.
        /// </summary>
        [Obsolete("Only availaible for Christmas and AprilFools.")]
        Scp559,

        /// <summary>
        /// Scp956 found you <see cref="global::Scp956Target"/>.
        /// </summary>
        [Obsolete("Only availaible for Christmas and AprilFools.")]
        Scp956Target,

        /// <summary>
        /// you are snowed <see cref="global::Snowed"/>.
        /// </summary>
        [Obsolete("Only availaible for Christmas and AprilFools.")]
        Snowed,

        /// <summary>
        /// Plays a sound effect to the affected player, and adds purple vignette to the affected player's vision.
        /// </summary>
        Scp1344Detected,

        /// <summary>
        /// Allows the affected player to speak with players in spectator or overwatch.
        /// </summary>
        Scp1576,

        /// <summary>
        /// Increases the affected player's jump height.
        /// </summary>
        Lightweight,

        /// <summary>
        /// Decreases the affected player's jump height.
        /// </summary>
        HeavyFooted,

        /// <summary>
        /// Makes the affected player transparent, 255 being completely transparent.
        /// </summary>
        Fade,

        /// <summary>
        /// Allows the affected player to see in dark areas. Does not extend the viewing range. Scales with intensity.
        /// </summary>
        NightVision,

        /// <summary>
        /// <see cref="CustomPlayerEffects.Metal"/>.
        /// </summary>
        [Obsolete("Only availaible for Halloween.")]
        Metal,

        /// <summary>
        /// <see cref="CustomPlayerEffects.OrangeCandy"/>.
        /// </summary>
        [Obsolete("Only availaible for Halloween.")]
        OrangeCandy,

        /// <summary>
        /// <see cref="CustomPlayerEffects.OrangeWitness"/>.
        /// </summary>
        [Obsolete("Only availaible for Halloween.")]
        OrangeWitness,

        /// <summary>
        /// <see cref="CustomPlayerEffects.Prismatic"/>.
        /// </summary>
        [Obsolete("Only availaible for Halloween.")]
        Prismatic,

        /// <summary>
        /// <see cref="CustomPlayerEffects.SlowMetabolism"/>.
        /// </summary>
        [Obsolete("Only availaible for Halloween.")]
        SlowMetabolism,

        /// <summary>
        /// <see cref="CustomPlayerEffects.Spicy"/>.
        /// </summary>
        [Obsolete("Only availaible for Halloween.")]
        Spicy,

        /// <summary>
        /// <see cref="CustomPlayerEffects.SugarCrave"/>.
        /// </summary>
        [Obsolete("Only availaible for Halloween or Christmas.")]
        SugarCrave,

        /// <summary>
        /// <see cref="CustomPlayerEffects.SugarHigh"/>.
        /// </summary>
        [Obsolete("Only availaible for Halloween.")]
        SugarHigh,

        /// <summary>
        /// <see cref="CustomPlayerEffects.SugarRush"/>.
        /// </summary>
        [Obsolete("Only availaible for Halloween.")]
        SugarRush,

        /// <summary>
        /// <see cref="CustomPlayerEffects.TemporaryBypass"/>.
        /// </summary>
        [Obsolete("Only availaible for Halloween.")]
        TemporaryBypass,

        /// <summary>
        /// <see cref="CustomPlayerEffects.TraumatizedByEvil"/>.
        /// </summary>
        [Obsolete("Only availaible for Halloween.")]
        TraumatizedByEvil,

        /// <summary>
        /// <see cref="CustomPlayerEffects.WhiteCandy"/>.
        /// </summary>
        [Obsolete("Only availaible for Halloween.")]
        WhiteCandy,

        /// <summary>
        /// Gives the affected player 25 non-decaying AHP, and sets their HP to 75 if it was above or at 75, otherwise if &lt;75 keeps current HP. Clearing this effect does not reset their AHP nor HP maximum.
        /// </summary>
        Scp1509Resurrected,

        /// <summary>
        /// <see cref="CustomPlayerEffects.FocusedVision"/>.
        /// </summary>
        FocusedVision,

        /// <summary>
        /// If the affected player has a maximum hume shield, this sets the hume shield to the maximum value.
        /// </summary>
        AnomalousRegeneration,

        /// <summary>
        /// Allows SCPs to see the affected player from a certain distance. Works on SCPs.
        /// </summary>
        AnomalousTarget,
    }
}
