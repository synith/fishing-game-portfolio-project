using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingMechanic : MonoBehaviour
{
    #region Variables
    public enum Difficulty { Easy, Medium, Hard };

    public static event EventHandler<bool> OnFishAction;
    public static event EventHandler OnFishingStart;
    public static event EventHandler OnFishingRestart;
    public static event EventHandler<Difficulty> OnDifficultyChanged;

    [SerializeField] Difficulty difficulty;
    [SerializeField] Zone playerZone;
    [SerializeField] Zone targetZone;

    [SerializeField] Color insideZoneColor;
    [SerializeField] Color outsideZoneColor;

    [SerializeField] float shrinkRate = 1;

    [SerializeField, Range(2.5f, 6f)] float easyStartRadius = 6f;
    [SerializeField, Range(2.5f, 6f)] float mediumStartRadius = 4.5f;
    [SerializeField, Range(2.5f, 6f)] float hardStartRadius = 3f;

    const float TARGET_ZONE_RADIUS = 1f;
    const float TARGET_ZONE_THICKNESS = 0.5f;
    const float PLAYER_ZONE_THICKNESS = 0.2f;
    const float MIN_RADIUS = 0.1f;

    float currentRadius;
    bool isShrinking;
    bool hasAttempted;
    #endregion

    void Start()
    {
        SetDifficulty(difficulty);
        SetTargetZone();
        //SetTargetZone(GetHandicap(IFishingRod.GetRodLevel(), IFish.GetFishLevel()));
    }

    void Update()
    {
        HandleShrinkingPlayerZone();
    }

    #region Event Subscription
    void OnEnable()
    {
        FishingControls.Instance.OnFishAttempt += FishingControls_OnFishAttempt;
        FishingControls.Instance.OnDebugTest += FishingControls_OnDebugTest;
        FishingControls.Instance.OnDebugRestart += FishingControls_OnDebugRestart;
    }

    private void FishingControls_OnFishAttempt(object sender, EventArgs e) => AttemptToCatchFish();
    private void FishingControls_OnDebugTest(object sender, EventArgs e) => StartFishing();
    private void FishingControls_OnDebugRestart(object sender, EventArgs e) => RestartFishing(difficulty);

    void OnDisable()
    {
        FishingControls.Instance.OnFishAttempt -= FishingControls_OnFishAttempt;
        FishingControls.Instance.OnDebugTest -= FishingControls_OnDebugTest;
        FishingControls.Instance.OnDebugRestart -= FishingControls_OnDebugRestart;
    }
    #endregion

    void SetDifficulty(Difficulty difficulty)
    {
        this.difficulty = difficulty;

        float startRadius = GetStartRadius(difficulty);
        currentRadius = startRadius;

        playerZone.SetZone(startRadius, PLAYER_ZONE_THICKNESS);

        OnDifficultyChanged?.Invoke(this, difficulty);
    }

    void SetTargetZone(int handicap = 0)
    {
        handicap = Mathf.Clamp(handicap, 0, 4);
        float thickness = TARGET_ZONE_THICKNESS;
        // TODO: increase thickness based on handicap
        targetZone.SetZone(TARGET_ZONE_RADIUS, thickness);
    }

    void HandleShrinkingPlayerZone()
    {
        if (!isShrinking) return;

        if (PlayerZoneStoppedShrinking())
        {
            AttemptToCatchFish();
            return;
        }

        currentRadius -= shrinkRate * Time.deltaTime;

        playerZone.SetZone(currentRadius, PLAYER_ZONE_THICKNESS);
        HandleZoneColorChange(insideZoneColor, outsideZoneColor);
    }
    void StartFishing()
    {
        if (hasAttempted) return;

        isShrinking = true;

        OnFishingStart?.Invoke(this, EventArgs.Empty);
    }

    void AttemptToCatchFish()
    {
        if (hasAttempted || !isShrinking) return;

        hasAttempted = true;
        isShrinking = false;

        bool isCaught = PlayerInTargetZone(playerZone, targetZone);
        OnFishAction?.Invoke(this, isCaught);
    }

    void RestartFishing(Difficulty difficulty)
    {
        if (!hasAttempted) return;

        isShrinking = false;
        hasAttempted = false;

        SetDifficulty(difficulty);
        HandleZoneColorChange(insideZoneColor, outsideZoneColor);

        OnFishingRestart?.Invoke(this, EventArgs.Empty);
    }

    int GetHandicap(int rodLevel, int fishLevel)
    {
        int handicap = rodLevel - fishLevel;
        return handicap < 0 ? 0 : handicap;
    }

    float GetStartRadius(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy: return easyStartRadius;
            case Difficulty.Medium: return mediumStartRadius;
            case Difficulty.Hard: return hardStartRadius;
            default: return easyStartRadius;
        }
    }

    bool PlayerZoneStoppedShrinking() => currentRadius < MIN_RADIUS;    

    void HandleZoneColorChange(Color insideZoneColor, Color outsideZoneColor)
    {
        if (PlayerInTargetZone(playerZone, targetZone))
        {
            playerZone.SetColor(insideZoneColor);
        }
        else
        {
            playerZone.SetColor(outsideZoneColor);
        }
    }  

    bool PlayerInTargetZone(Zone playerZone, Zone targetZone)
    {
        if (playerZone.GetOutsideRingRadius() < targetZone.GetInsideRingRadius())
        {
            return false;
        }
        else if (playerZone.GetInsideRingRadius() > targetZone.GetOutsideRingRadius())
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
