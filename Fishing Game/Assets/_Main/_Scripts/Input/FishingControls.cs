using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingControls : MonoBehaviour
{
    public static FishingControls Instance { get; private set; }

    public event EventHandler OnFishAttempt;

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
        fishingInputActions.Player.Fish.performed += FishAttempt_Performed;
        fishingInputActions.Player.Fish.Enable();
    }

    void FishAttempt_Performed(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        OnFishAttempt?.Invoke(this, EventArgs.Empty);
    }

    void OnDisable()
    {
        fishingInputActions.Player.Fish.performed += FishAttempt_Performed;
        fishingInputActions.Player.Fish.Enable();
    }


}
