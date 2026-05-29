using System;
using UnityEngine;

public static class InputManager
{
    private static PlayerInputs inputs;

    public static event Action OnJump, OnAiming, OnCharging, OnShooting, OnCancel, OnPause;

    // public static event Action OnPauseRequested;
    // public static Action OnPauseAllowed;

    static InputManager()
    {
        inputs = new PlayerInputs();
        inputs.Enable();
        
        inputs.Player.Jump.performed += _ => OnJump?.Invoke();
        inputs.Player.BackFromShooting.performed += _ => OnAiming?.Invoke();
        inputs.Player.BackFromShooting.canceled += _ => OnCancel?.Invoke();
        inputs.Player.Shoot.performed += _ => OnCharging?.Invoke();
        inputs.Player.Shoot.canceled += _ => OnShooting?.Invoke();
        inputs.Player.Pause.performed += _ => OnPause?.Invoke();
        // inputs.Player.Pause.performed += _ => OnPauseRequested?.Invoke();
        // OnPauseAllowed += SwitchTo_UI;
    }
    
    private static Vector2 GetMovementVector => inputs.Player.Movement.ReadValue<Vector2>();

    public static bool IsMoving(out Vector2 direction)
    {
        direction = new Vector2(GetMovementVector.x, GetMovementVector.y);
        return direction != Vector2.zero;
    }

    // static void SwitchTo_UI()
    // {
    //     inputs.Player.Disable();
    //     inputs.UI.Enable();
    // }

    // static void SwitchTo_PlayerInput()
    // {
    //     inputs.UI.Disable();
    //     inputs.Player.Enable();
    // }
}
