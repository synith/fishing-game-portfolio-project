using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingControls : MonoBehaviour
{
    public static FishingControls Instance { get; private set; }

    public event EventHandler OnButtonPressed;
    public event EventHandler OnPausePressed;

    FishingInputActions fishingInputActions;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        fishingInputActions = new FishingInputActions();
    }

    void OnEnable()
    {
        fishingInputActions.Player.FishAttempt.performed += FishAttempt_Performed;
        fishingInputActions.Player.FishAttempt.Enable();

        fishingInputActions.Player.Pause.performed += Pause_performed;
        fishingInputActions.Player.Pause.Enable();
    }

    void Pause_performed(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        OnPausePressed?.Invoke(this, EventArgs.Empty);
    }

    void FishAttempt_Performed(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        OnButtonPressed?.Invoke(this, EventArgs.Empty);
    }

    void OnDisable()
    {
        fishingInputActions.Player.FishAttempt.performed -= FishAttempt_Performed;
        fishingInputActions.Player.FishAttempt.Disable();

        fishingInputActions.Player.Pause.performed -= Pause_performed;
        fishingInputActions.Player.Pause.Disable();
    }
}
