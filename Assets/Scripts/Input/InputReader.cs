using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;


[CreateAssetMenu(fileName = "New Input Reader", menuName = "Input/Input Reader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    public event Action<bool> PrimaryFireEvent;
    public event Action<bool> ChangeAltitudeEvent;
    public event Action<bool> UseAbilityEvent;
    public event Action<Vector2> MoveEvent;
    public event Action<bool> BoostFireEvent;
    public event Action<bool> MapViewEvent;
    public event Action SpinEvent;

    public Vector2 AimPosition {  get; private set; }
    public Vector2 MouseAimPosition { get; private set; }
    public Vector2 ControllerAim { get; private set; }
    public bool IsUsingGamepad { get; private set; } // for control scheme updates and ui visual changes.

    public bool IsMapOpen { get; private set; }

    private Controls controls;

    private void OnEnable()
    {
        // instance of controls
        if (controls == null)
        {
            controls = new Controls();
            controls.Player.SetCallbacks(this);
        }

        controls.Player.Enable();
        //
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>()); 
    }

    public void OnPrimaryFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PrimaryFireEvent?.Invoke(true);
        }
        else if (context.canceled)
        {
            PrimaryFireEvent?.Invoke(false);
        }
       
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        // help us figure out where to aim
       AimPosition = context.ReadValue<Vector2>();

        if (context.performed)
        {
            IsUsingGamepad = false;
        }
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    public void OnChangeAltitude(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ChangeAltitudeEvent?.Invoke(true);
        }
      /*  else if (context.canceled)
        {
            ChangeAltitudeEvent?.Invoke(false);
        }*/
    }

    public void OnUseAbility(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            UseAbilityEvent?.Invoke(true);
        }
        else if (context.canceled)
        {
            UseAbilityEvent?.Invoke(false);
        }
    }

    public void OnBoost(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            BoostFireEvent?.Invoke(true);
        }
        else if (context.canceled)
        {
            BoostFireEvent?.Invoke(false);
        }
    }

    public void OnAimController(InputAction.CallbackContext context)
    {
         ControllerAim = context.ReadValue<Vector2>();

        if(context.performed && ControllerAim.sqrMagnitude > 0.01f) // check to see if player is using controller 
            // may change squaremag offset value due to people with stick drift. 
        {
            IsUsingGamepad = true; 
        }
    }

    public void OnMap(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IsMapOpen = !IsMapOpen; // swap state from true to false; 
            Debug.Log("map state is " + IsMapOpen); // verify the map is working correctly. 
            MapViewEvent?.Invoke(IsMapOpen);
        }

    }

    public void OnBarrelRollSpin(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SpinEvent?.Invoke();
        }
    }
}
