using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsScriptableObject", menuName = "ScriptableObjects/playerStats")]
public class PlayerStats : ScriptableObject
{
    [Header("Player topdownSettings")]

    [SerializeField] GameObject shipBody;
    [SerializeField] GameObject turretBody;

    [Tooltip("Time it takes for spin to complete.")]
    [SerializeField] float TopDownSpinDuration;



    [Header("handling")]
    [SerializeField] float shipMass;

    [Tooltip("General speed of ship")]
    [SerializeField] float MovementSpeed;

    [Tooltip("Speed of ship turning in another direction")]
    [SerializeField] float TurningRate;

    [Tooltip("The rate of time it takes for ship to change between altitudes in topdown")]
    [SerializeField] float altitudeSpeedChangeRate;

    [Tooltip("dictates how much the ship spins. Typical rate is a full 360 turn, can be adjusted for different turn values.")]
    [SerializeField] float spinAmount;

    [Tooltip("strength of lateral dodge, without horizontal input not much will happen though.")]
    [SerializeField] float SpinDodgeSpeed;


    [Header("Third Person Handling")]
    [SerializeField] float ControlSpeed;
    [Tooltip("forward velocity, Higher speeds give faster flight, no gradual acceleration.")]
    [SerializeField] float ThrustValue;

    [Tooltip("How fast the ship breaks, higher value faster breaking Note that ships mass also takes part in rate of breaking.")]
    [SerializeField] float BreakValue;

    [Tooltip("Left and Right rotation, higher values give faster turning, can make it feel twitchier")]
    [SerializeField] float YawRate;

    [Tooltip("Up and Down Movement, higher values give faster climbs and dives")]
    [SerializeField] float PitchRate;

    [Tooltip("VISUAL: tilts ship when steering horiztontaly, more visible banking no change in actual flight direction")]
    [SerializeField] float MaxRollAngle;

    [Tooltip("VISUAL speed of visual banking, lower value makes it lag and settle quicker")]
    [SerializeField] float VisualRotationSpeed;




    [Header("Controller properties")]

    [Tooltip(" (more important for controller) ignores movement input when combined stick magnitude is below that value, less accidental input with value change but omre movement neded before turning starts")]
    [SerializeField] float InputDeadZone;

    [Tooltip("For controller aim for turret moves in response to stick, faster controll over turret aiming, doesn't change ship steering or mosue aim speed")]
    [SerializeField] float ControllerAimSpeed;


    [Header("COMBAT- Projectile Properties")]

    [Tooltip("Damage projectile deals Bigger value higher boom boom")]
    [SerializeField] int Damage;

    [Tooltip("How fast the projectile is, higher value means higher projectile velocity")]
    [SerializeField] float ProjectileSpeed;

    [Tooltip("Interval between shots, value msut be above 0, higher values grant faster rate of fire.")]
    [SerializeField] float FireRate;

    [Tooltip("Muzle flash duration, may want to sync this with fire rate for visual feel, higher value means flash stays longer")]
    [SerializeField] float MuzzleFlashDuration;

    [Tooltip("Cost to fire projectiles, higher value means it costs more to shoot, should adjust with damage and projectile values so it feels fair")]
    [SerializeField] float CostToFire;

    [Header("COMBAT - Turret Properties")]

    [Tooltip("Rate with third person turret rotation reaching a requeste angle, higher values means turret catches up quicker, slower makes it more sluggish, be careful when adjusting this for fairness")]
    [SerializeField] float TurretAimSpeed;

    [Tooltip("Max left and right turret aim adjustments to this can increase horizontal aim arc, could be adjusted for some ships for wider angles to shoot")]
    [SerializeField] float MaxTurretYaw;

    [Tooltip("Max up and Down Turret Aim, Adjust to increase vertical aiming, could be adjusted for taller aim")]
    [SerializeField] float MaxTurretPitch;

    
    

}
