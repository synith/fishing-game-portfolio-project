using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingMechanic : MonoBehaviour
{
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



    void Start()
    {
        SetDifficulty(difficulty);
        SetTargetZone();
        //SetTargetZone(GetHandicap(IFishingRod.GetRodLevel(), IFish.GetFishLevel()));
    }

    void SetTargetZone(int handicap = 0)
    {
        handicap = Mathf.Clamp(handicap, 0, 4);
        float thickness = TARGET_ZONE_THICKNESS;
        // TODO: increase thickness based on handicap
        targetZone.SetZone(TARGET_ZONE_RADIUS, thickness);
    }

    int GetHandicap(int rodLevel, int fishLevel)
    {
        int handicap = rodLevel - fishLevel;
        return handicap < 0 ? 0 : handicap;
    }

    void SetDifficulty(Difficulty difficulty)
    {
        this.difficulty = difficulty;

        float startRadius = GetStartRadius(difficulty);
        currentRadius = startRadius;

        playerZone.SetZone(startRadius, PLAYER_ZONE_THICKNESS);

        OnDifficultyChanged?.Invoke(this, difficulty);
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

    void StartFishing()
    {
        isShrinking = true;
        OnFishingStart?.Invoke(this, EventArgs.Empty);
    }

    void RestartFishing(Difficulty difficulty)
    {
        isShrinking = false;
        hasAttempted = false;
        SetDifficulty(difficulty);
        HandleZoneColorChange(insideZoneColor, outsideZoneColor);
        OnFishingRestart?.Invoke(this, EventArgs.Empty);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !hasAttempted)
        {
            StartFishing();
        }

        if (Input.GetKeyDown(KeyCode.R) && hasAttempted)
        {
            RestartFishing(difficulty);
        }

        if (!hasAttempted && !isShrinking)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetDifficulty(Difficulty.Easy);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetDifficulty(Difficulty.Medium);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SetDifficulty(Difficulty.Hard);
            }
        }

        if (!isShrinking) return;

        HandleShrinkingPlayerZone();

        if (hasAttempted) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            FishAction();
        }
    }

    bool PlayerZoneStoppedShrinking() => currentRadius < MIN_RADIUS;

    void HandleShrinkingPlayerZone()
    {
        if (PlayerZoneStoppedShrinking())
        {
            FishAction();
            return;
        }

        currentRadius -= shrinkRate * Time.deltaTime;

        playerZone.SetZone(currentRadius, PLAYER_ZONE_THICKNESS);
        HandleZoneColorChange(insideZoneColor, outsideZoneColor);
    }

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

    void FishAction()
    {
        hasAttempted = true;
        isShrinking = false;
        bool isCaught = PlayerInTargetZone(playerZone, targetZone);
        OnFishAction?.Invoke(this, isCaught);
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
