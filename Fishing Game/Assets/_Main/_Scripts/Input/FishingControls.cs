using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingControls : MonoBehaviour
{
    public static FishingControls Instance { get; private set; }

    public event EventHandler OnFishAttemptPressed;

    public event EventHandler OnDebugTestPressed;
    public event EventHandler OnDebugRestartPressed;
    public event EventHandler OnPausePressed;

    FishingInputActions _fishingInputActions;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _fishingInputActions = new FishingInputActions();
    }

    void OnEnable()
    {
        _fishingInputActions.Player.FishAttempt.performed += FishAttempt_Performed;
        _fishingInputActions.Player.FishAttempt.Enable();

        _fishingInputActions.Player.Pause.performed += Pause_performed;
        _fishingInputActions.Player.Pause.Enable();


        _fishingInputActions.Debug.Test.performed += Test_performed;
        _fishingInputActions.Debug.Test.Enable();

        _fishingInputActions.Debug.Restart.performed += Restart_performed;
        _fishingInputActions.Debug.Restart.Enable();
    }

    void Pause_performed(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        OnPausePressed?.Invoke(this, EventArgs.Empty);
    }

    void Restart_performed(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        OnDebugRestartPressed?.Invoke(this, EventArgs.Empty);
    }

    void Test_performed(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        OnDebugTestPressed?.Invoke(this, EventArgs.Empty);
    }

    void FishAttempt_Performed(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        OnFishAttemptPressed?.Invoke(this, EventArgs.Empty);
    }

    void OnDisable()
    {
        _fishingInputActions.Player.FishAttempt.performed -= FishAttempt_Performed;
        _fishingInputActions.Player.FishAttempt.Disable();

        _fishingInputActions.Player.Pause.performed -= Pause_performed;
        _fishingInputActions.Player.Pause.Disable();

        _fishingInputActions.Debug.Test.performed -= Test_performed;
        _fishingInputActions.Debug.Test.Disable();

        _fishingInputActions.Debug.Restart.performed -= Test_performed;
        _fishingInputActions.Debug.Restart.Disable();
    }
}
