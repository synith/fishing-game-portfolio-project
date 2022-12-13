using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingControls : MonoBehaviour
{
    public static FishingControls Instance { get; private set; }

    public event EventHandler OnFishAttemptPressed;

    public event EventHandler OnDebugTestPressed;
    public event EventHandler OnDebugRestartPressed;

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


        fishingInputActions.Debug.Test.performed += Test_performed;
        fishingInputActions.Debug.Test.Enable();

        fishingInputActions.Debug.Restart.performed += Restart_performed;
        fishingInputActions.Debug.Restart.Enable();
    }

    private void Restart_performed(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        OnDebugRestartPressed?.Invoke(this, EventArgs.Empty);
    }

    private void Test_performed(InputAction.CallbackContext context)
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
        fishingInputActions.Player.FishAttempt.performed -= FishAttempt_Performed;
        fishingInputActions.Player.FishAttempt.Disable();

        fishingInputActions.Debug.Test.performed -= Test_performed;
        fishingInputActions.Debug.Test.Disable();

        fishingInputActions.Debug.Restart.performed -= Test_performed;
        fishingInputActions.Debug.Restart.Disable();
    }


}
