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
        /// Prevents the player from reloading weapons and using medical items.
        /// </summary>
        AmnesiaItems,

        /// <summary>
        /// Makes SCP-939 invisible to players under its effect. Visibility is temporarily restored when SCP-939 takes damage or attacks.
        /// </summary>
        AmnesiaVision,

        /// <summary>
        /// Drains the player's stamina and then health.
        /// </summary>
        Asphyxiated,

        /// <summary>
        /// Damages the player over time.
        /// </summary>
        Bleeding,

        /// <summary>
        /// Blurs the player's screen.
        /// </summary>
        Blinded,

        /// <summary>
        /// Increases damage the player receives. Does not apply any standalone damage.
        /// </summary>
        Burned,

        /// <summary>
        /// Blurs the player's screen while rotating.
        /// </summary>
        Concussed,

        /// <summary>
        /// Effect given to player after being hurt by SCP-106.
        /// </summary>
        Corroding,

        /// <summary>
        /// Deafens the player.
        /// </summary>
        Deafened,

        /// <summary>
        /// Removes 10% of the player's health per second.
        /// </summary>
        Decontaminating,

        /// <summary>
        /// Slows down the player's movement.
        /// </summary>
        Disabled,

        /// <summary>
        /// Prevents the player from moving.
        /// </summary>
        Ensnared,

        /// <summary>
        /// Halves the player's maximum stamina and stamina regeneration rate.
        /// </summary>
        Exhausted,

        /// <summary>
        /// Flashes the player.
        /// </summary>
        Flashed,

        /// <summary>
        /// Drains the player's health while sprinting.
        /// </summary>
        Hemorrhage,

        /// <summary>
        /// Reduces the player's FOV, gives infinite stamina and gives the effect of underwater sound.
        /// </summary>
        Invigorated,

        /// <summary>
        /// Reduces damage taken by body shots.
        /// </summary>
        BodyshotReduction,

        /// <summary>
        /// Damages the player every 5 seconds, starting low and increasing over time.
        /// </summary>
        Poisoned,

        /// <summary>
        /// Increases the speed of the player while also draining health.
        /// </summary>
        Scp207,

        /// <summary>
        /// Makes the player invisible.
        /// </summary>
        Invisible,

        /// <summary>
        /// Slows down the player's movement with the SCP-106 sinkhole effect.
        /// </summary>
        SinkHole,

        /// <summary>
        /// Reduces overall damage taken.
        /// </summary>
        DamageReduction,

        /// <summary>
        /// Increases movement speed.
        /// </summary>
        MovementBoost,

        /// <summary>
        /// Reduces the severity of negative effects.
        /// </summary>
        RainbowTaste,

        /// <summary>
        /// Drops the player's current item, disables interaction with objects, spawns hands that drop to the floor, and deals damage while effect is active.
        /// </summary>
        SeveredHands,

        /// <summary>
        /// Prevents the player from sprinting and reduces movement speed by 20%.
        /// </summary>
        Stained,

        /// <summary>
        /// Causes the player to become gain immunity to certain negative status effects.
        /// </summary>
        Vitality,

        /// <summary>
        /// Cause the player to slowly take damage, reduces bullet accuracy, and increases item pickup time.
        /// </summary>
        Hypothermia,

        /// <summary>
        /// Increases the player's motor function, causing the player to reduce the weapon draw time, reload spead, item pickup speed, and medical item usage.
        /// </summary>
        Scp1853,

        /// <summary>
        /// Effect given to player after being hurt by SCP-049. Deals 8 damage per second, after an inital 16 damage for the first second.
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
        /// Protects players from enemy damage if the config is enabled.
        /// </summary>
        SpawnProtected,

        /// <summary>
        /// Make Scp106 able to see you when he is in the ground (stalking), causes the player's screens to become monochromatic when seeing Scp106, and instantly killed if attacked by Scp106.
        /// </summary>
        Traumatized,

        /// <summary>
        /// Slows down the player, providing passive health regeneration, AHP up to 75, and saves the player from death once.
        /// </summary>
        AntiScp207,

        /// <summary>
        /// The effect that SCP-079 gives the scanned player with the Breach Scanner. Mutes the soundtrack.
        /// </summary>
        Scanned,

        /// <summary>
        /// Teleports the player to the pocket dimension and drains health until the player escapes or is killed. The amount of damage recieved increases the longer the effect is applied.
        /// </summary>
        PocketCorroding,

        /// <summary>
        /// Reduces walking sound by 10% per intensity level.
        /// </summary>
        SilentWalk,

        /// <summary>
        /// Makes you a marshmallow guy.
        /// </summary>
        // [Obsolete("Not functional in-game")]
        Marshmallow,

        /// <summary>
        /// The effect that is given to the player when getting attacked by SCP-3114's Strangle ability.
        /// </summary>
        Strangled,

        /// <summary>
        /// Allow players to pass through doors.
        /// </summary>
        Ghostly,

        /// <summary>
        /// Manipulate which fog type the player will have.
        /// <remarks>You can choose fog with <see cref="CustomRendering.FogType"/> and putting it on intensity.</remarks>
        /// </summary>
        FogControl,

        /// <summary>
        /// Slows the player down by 1% per intensity.
        /// </summary>
        Slowness,

        /// <summary>
        /// Allows the affected user to see players through walls, with a slight delay between spurts of viewability.
        /// </summary>
        Scp1344,

        /// <summary>
        /// Does not blind the player. Spawns eyeballs that drop to the floor, and does 10 damage per second.
        /// </summary>
        SeveredEyes,

        /// <summary>
        /// Immediately kills the player with death message "Fatal blunt trauma; the body is badly mutilated and pupled.", and "Reason: Crushed" through console.
        /// </summary>
        PitDeath,

        /// <summary>
        /// Blurs the affected player's vision.
        /// </summary>
        Blurred,

        /// <summary>
        /// Makes you a flamingo <see cref="CustomPlayerEffects.BecomingFlamingo"/>.
        /// </summary>
        [Obsolete("Only availaible for Christmas and AprilFools.")]
        BecomingFlamingo,

        /// <summary>
        /// Makes you a Child after eating Cake <see cref="Scp559Effect"/>.
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
        /// <see cref="CustomPlayerEffects.Scp1344Detected"/>.
        /// </summary>
        Scp1344Detected,

        /// <summary>
        /// Allows the affected player to speak with players in spectator or overwatch.
        /// </summary>
        Scp1576,

        /// <summary>
        /// Increases the player's jump height.
        /// </summary>
        Lightweight,

        /// <summary>
        /// Decreases the player's jump height.
        /// </summary>
        HeavyFooted,

        /// <summary>
        /// Makes the user transparent, 255 being completely transparent.
        /// </summary>
        Fade,

        /// <summary>
        /// Allows the user to see in dark areas. Does not extend the viewing range. Scales with intensity.
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
        /// Gives the user 25 non-decaying AHP, and sets the user's HP to 75 if it was above or at 75, otherwise if &lt;75 keeps current HP. Clearing this effect does not reset the AHP nor HP maximum.
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
        /// Allows SCPs to see the target from a certain distance. Works on SCPs.
        /// </summary>
        AnomalousTarget,
    }
}
