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
    bool isActive;
    bool hasAttempted;




    void Start()
    {
        targetZone.SetZone(TARGET_ZONE_RADIUS, TARGET_ZONE_THICKNESS);
        targetZone.UpdateZone(TARGET_ZONE_RADIUS, TARGET_ZONE_THICKNESS);

        float startRadius = GetStartRadius(difficulty);
        playerZone.SetZone(startRadius, PLAYER_ZONE_THICKNESS);
        playerZone.UpdateZone(startRadius, PLAYER_ZONE_THICKNESS);

        currentRadius = startRadius;
    }

    void SetDifficulty(Difficulty difficulty)
    {
        this.difficulty = difficulty;

        float startRadius = GetStartRadius(difficulty);
        currentRadius = startRadius;

        playerZone.SetZone(startRadius, PLAYER_ZONE_THICKNESS);
        playerZone.UpdateZone(startRadius, PLAYER_ZONE_THICKNESS);

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !hasAttempted)
        { 
            isActive = true;
            OnFishingStart?.Invoke(this, EventArgs.Empty);
        }


        if (Input.GetKeyDown(KeyCode.R))
        {
            isActive = false;
            hasAttempted = false;

            float startRadius = GetStartRadius(difficulty);
            playerZone.SetZone(startRadius, PLAYER_ZONE_THICKNESS);
            playerZone.UpdateZone(startRadius, PLAYER_ZONE_THICKNESS);
            UpdateZoneColor(playerZone, targetZone, insideZoneColor, outsideZoneColor);

            currentRadius = startRadius;
            OnFishingRestart?.Invoke(this, EventArgs.Empty);
        }

        if (!hasAttempted && !isActive)
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

        if (!isActive) return;

        if (currentRadius < MIN_RADIUS)
        {
            FishAction();
            return;
        }

        currentRadius -= shrinkRate * Time.deltaTime;

        playerZone.SetZone(currentRadius, PLAYER_ZONE_THICKNESS);
        playerZone.UpdateZone(currentRadius, PLAYER_ZONE_THICKNESS);
        UpdateZoneColor(playerZone, targetZone, insideZoneColor, outsideZoneColor);


        if (hasAttempted) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            hasAttempted = true;
            FishAction();
        }
    }

    void UpdateZoneColor(Zone zone, Zone targetZone, Color insideZoneColor, Color outsideZoneColor)
    {
        if (PlayerInTargetZone(zone, targetZone))
        {
            zone.SetColor(insideZoneColor);
        }
        else
        {
            playerZone.SetColor(outsideZoneColor);
        }
    }

    void FishAction()
    {
        isActive = false;
        bool isCaught = PlayerInTargetZone(playerZone, targetZone);
        OnFishAction?.Invoke(this, isCaught);
    }


    bool PlayerInTargetZone(Zone playerZone, Zone targetZone)
    {
        if (playerZone.GetOuterRadius() < targetZone.GetInnerRadius())
        {
            return false;
        }
        else if (playerZone.GetInnerRadius() > targetZone.GetOuterRadius())
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
