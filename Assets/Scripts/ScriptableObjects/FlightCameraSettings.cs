using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[CreateAssetMenu(fileName ="CameraSettingsScriptableObject", menuName ="ScriptableObjects/FlightCam")]
public class FlightCameraSettings : ScriptableObject
{
    [Header("Cinemachine Virtual Camera properties")]
    public Transform FollowTransform = null;
    public Transform LookAtTransform = null;
    public float HorizontalFOV = 60f;
    // may not expose because this value isn't saved
    public CinemachineTransposer CinemachineTransposer = null;
    public Vector3 FollowingOffset = new Vector3(0f, 0f, 0f);
    public float xDamping = 0f;
    public float yDamping = 0f;
    public float zDamping = 0f;
    // may not expose cause this value isn't saved
    public Cinemachine3rdPersonAim Cinemachine3RdPersonAim = null;

    // may need to setup some composer values to test better aim 

    [Header("Transform properties")]
    public Vector3 CameraPositionValues = new Vector3(0f, 0f, 0f);
    public Vector3 CameraRotationValues = new Vector3(0f, 0f, 0f);
    public Vector3 CameraScaleValues = new Vector3(0f, 0f, 0f);
}
