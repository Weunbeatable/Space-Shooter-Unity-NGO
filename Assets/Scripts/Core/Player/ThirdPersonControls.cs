using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ThirdPersonControls : NetworkBehaviour
{
    [Header("input controller")]
    [Tooltip("The scriptable object for the game controls ")] [SerializeField] InputReader playerInput;
    [Header("General Setup Settings")]
    [Tooltip("How fast ship moves up and down based upon player input")][SerializeField] float controlSpeed = 20f;
    [Tooltip("control the x constraints for movement")][SerializeField] float xRange = 5f; // the range our player can move left and right
    [Tooltip("Control y constraints for movement")][SerializeField] float yRange = 7f;

    [Tooltip("array to store lasers")][SerializeField] GameObject[] lasers;

    [Header("Ship position based  Tuning")]
    [Tooltip("position control of ship")][SerializeField] float positionPitchFator = -2f; // how much we affect the change in ship position upon using a control
    [Tooltip("adjust values to affect ships yaw position")][SerializeField] float positionYawFactor = -5f;


    [Header("pitch, yaw, and roll control aka X,Y,Z controls")]
    [Tooltip("Adjust to affect ships yaw movement")][SerializeField] float controlYawFactor = -10f;
    [Tooltip("adjust to controll the roll 'Z' factor of ship")][SerializeField] float controlRollFactor = -25f;
    [Tooltip("pitch control of ship")][SerializeField] float controlPitchFactor = -10f; // how much we see the pitch being affected by our mouse movement

    float yThrow;
    float xThrow;
    void Update()
    {
        ProcessTranslation();
        ProcessRotation();
        ProcessFiring();
    }

    private void ProcessRotation()
    {
        float pitchDueToPosition = transform.localPosition.y * positionPitchFator;
        float PitchDueToControlThrow = yThrow * controlPitchFactor;

        float yawDueToPosition = transform.localPosition.x * positionYawFactor;
        float yawDuetoControlThrow = xThrow * controlYawFactor;

        float rollDueToControlThrow = xThrow * controlRollFactor;

        float pitch = pitchDueToPosition + PitchDueToControlThrow;
        float yaw = yawDueToPosition;
        float roll = rollDueToControlThrow; // these three will be updated based upon position, an dposition of craft as well as how player is positioning the controller
        transform.localRotation = Quaternion.Euler(pitch, yaw, roll); // Quaternions relate to our rotation. x, y, z = pitch, yaw and roll
        Debug.Log("Ship_Pitch " + pitch);
        Debug.Log("Ship_Yaw " + yaw);
        Debug.Log("Ship_Roll " + roll);
    }

    private void ProcessTranslation()
    {
        xThrow = Input.GetAxis("Horizontal");
        yThrow = Input.GetAxis("Vertical");

        float xOffset = xThrow * Time.deltaTime * controlSpeed;
        float yOffset = yThrow * Time.deltaTime * controlSpeed;


        float rawXPos = transform.localPosition.x + xOffset;
        float rawYPos = transform.localPosition.y + yOffset;

        float clampedXPos = Mathf.Clamp(rawXPos, -xRange, xRange); // this allows us to specify how far left and right our ship can move. 
        float clampedYPos = Mathf.Clamp(rawYPos, -yRange + 5, yRange);

        transform.localPosition = new Vector3(clampedXPos, clampedYPos, transform.localPosition.z);// this allows us to move the ship in 3D space hence vector3, clampedxpos restricts our left and right movement
        Debug.Log("X_Controller_Pos " + clampedXPos);
        Debug.Log("Y_Controller_Pos " + clampedYPos);
    }

    void ProcessFiring()
    {
        if (Input.GetButton("Fire1"))
        {
            SetLasersActive(true);
        }
        else
        {
            SetLasersActive(false);
        }
    }

    void SetLasersActive(bool isActive)
    {
        foreach (GameObject laser in lasers)
        {
            var emissionModule = laser.GetComponent<ParticleSystem>().emission;
            emissionModule.enabled = isActive; // is active is basically calling the boolean to check wheter or not we are clicking the button

        }
    }

}
