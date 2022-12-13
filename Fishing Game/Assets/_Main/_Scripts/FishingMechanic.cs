using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingMechanic : MonoBehaviour
{
    #region Variables
    public enum Difficulty { Easy, Medium, Hard };

    public static event Action<bool, bool> OnFishAttempt;
    public static event EventHandler OnFishingStart;
    public static event EventHandler OnFishingRestart;
    public static event EventHandler<Difficulty> OnDifficultyChanged;

    [SerializeField] Zone playerZone;
    [SerializeField] Zone targetZone;

    [SerializeField] Color insideZoneColor;
    [SerializeField] Color outsideZoneColor;

    [SerializeField] float shrinkRate = 1;

    [SerializeField, Range(2.5f, 6f)] float easyStartRadius = 6f;
    [SerializeField, Range(2.5f, 6f)] float mediumStartRadius = 4.5f;
    [SerializeField, Range(2.5f, 6f)] float hardStartRadius = 3f;

    [SerializeField] MMFeedbacks AttemptPossibleFeedback;
    [SerializeField] MMFeedbacks AttemptMadeFeedback;
    [SerializeField] MMFeedbacks AttemptFailedFeedback;
    [SerializeField] MMFeedbacks FishingStartedFeedback;
    [SerializeField] MMFeedbacks FishCaughtFeedback;

    const float TARGET_ZONE_RADIUS = 1f;
    const float TARGET_ZONE_THICKNESS = 0.7f;
    const float PLAYER_ZONE_THICKNESS = 0.2f;
    const float MIN_RADIUS = 0.1f;

    List<Difficulty> difficultyList;
    Difficulty currentDifficulty;

    Color startingColor;
    float currentRadius;
    bool isShrinking;
    bool hasAttempted;
    #endregion

    private void Awake()
    {
        difficultyList = new List<Difficulty>();
    }

    void Start()
    {
        FishSO startingFish = FishTracker.Instance.ActiveFish;
        ConfigureDifficultyList(startingFish);

        startingColor = targetZone.GetColor();

        FishTracker.Instance.OnActiveFishChanged += FishTracker_OnActiveFishChanged;
    }

    private void ConfigureDifficultyList(FishSO fish)
    {
        difficultyList.Clear();

        foreach (Difficulty difficulty in fish.difficultyList)
        {
            difficultyList.Add(difficulty);
        }

        Debug.Log($"Difficulty for Fish: {fish}");
        for (int i = 0; i < difficultyList.Count; i++)
        {
            Debug.Log($"Difficulty {i} : {difficultyList[i]}");
        }

        currentDifficulty = GetNextDifficultyInList();
        SetDifficulty(currentDifficulty);
        SetTargetZone();
    }

    private void FishTracker_OnActiveFishChanged(FishSO fish)
    {
        ConfigureDifficultyList(fish);
    }

    Difficulty GetNextDifficultyInList()
    {
        Difficulty difficulty = difficultyList[0];
        return difficulty;
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

        OnFishAttempt += HandleFishingAttempt;
    }

    void HandleFishingAttempt(bool isPossible, bool isLastDifficulty)
    {

        if (isPossible && isLastDifficulty)
        {
            HandleZoneColor();
            FishCaughtFeedback?.PlayFeedbacks();
            FishTracker.Instance.SetRandomFishActive();


            print("SUCCEED!");
        }
        if (isPossible && !isLastDifficulty)
        {
            difficultyList.Remove(currentDifficulty);


            currentDifficulty = GetNextDifficultyInList();

            
            RestartFishing(currentDifficulty);



            Invoke(nameof(StartFishing), 1.5f);
            print("AGAIN!");
            
        }
        if (!isPossible)
        {
            AttemptFailedFeedback?.PlayFeedbacks();
            HandleZoneColor();

            hasAttempted = true;

            FishTracker.Instance.SetRandomFishActive();
            currentDifficulty = GetNextDifficultyInList();


            // RestartFishing(currentDifficulty);

            print("FAIL");
            
        }


    }

    void FishingControls_OnFishAttempt(object sender, EventArgs e) => AttemptToCatchFish();
    void FishingControls_OnDebugTest(object sender, EventArgs e) => StartFishing();
    void FishingControls_OnDebugRestart(object sender, EventArgs e) => RestartFishing(currentDifficulty);

    void OnDisable()
    {
        FishingControls.Instance.OnFishAttempt -= FishingControls_OnFishAttempt;
        FishingControls.Instance.OnDebugTest -= FishingControls_OnDebugTest;
        FishingControls.Instance.OnDebugRestart -= FishingControls_OnDebugRestart;

        OnFishAttempt -= HandleFishingAttempt;
    }
    #endregion


    void SetDifficulty(Difficulty difficulty)
    {
        currentDifficulty = difficulty;

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

        if (HasPlayerZoneReachedEnd())
        {
            AttemptToCatchFish();
            return;
        }

        currentRadius -= shrinkRate * Time.deltaTime;

        playerZone.SetZone(currentRadius, PLAYER_ZONE_THICKNESS);
        HandleZoneColor();
    }
    void StartFishing()
    {
        if (hasAttempted) return;

        isShrinking = true;
        StartCoroutine(nameof(ShowAttemptPossible));

        FishingStartedFeedback?.PlayFeedbacks();

        OnFishingStart?.Invoke(this, EventArgs.Empty);

        HandleZoneColor();
    }

    IEnumerator ShowAttemptPossible()
    {
        yield return new WaitUntil(() => IsAttemptPossible());
        AttemptPossibleFeedback?.PlayFeedbacks();
    }

    void AttemptToCatchFish()
    {
        if (hasAttempted || !isShrinking) return;

        hasAttempted = true;
        isShrinking = false;

        StopCoroutine(nameof(ShowAttemptPossible));
        AttemptMadeFeedback?.PlayFeedbacks();
        

        bool isAttemptPossible = IsAttemptPossible();
        bool isFishOnLastDifficulty = difficultyList.Count == 1;

        OnFishAttempt?.Invoke(isAttemptPossible, isFishOnLastDifficulty);       
    }

    void RestartFishing(Difficulty difficulty)
    {
        if (!hasAttempted) return;

        isShrinking = false;
        hasAttempted = false;

        SetDifficulty(difficulty);    

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

    bool HasPlayerZoneReachedEnd() => playerZone.GetOutsideRingRadius() < targetZone.GetInsideRingRadius();
    bool HasNotStarted() => !isShrinking && !hasAttempted;

    void HandleZoneColor()
    {
        if (IsAttemptPossible())
        {
            targetZone.SetColor(insideZoneColor);
            return;
        }
        else if (HasPlayerZoneReachedEnd())
        {
            targetZone.SetColor(outsideZoneColor);
            return;
        }
        else if (HasNotStarted())
        {
            targetZone.SetColor(startingColor);
            return;
        }
        else if (!isShrinking && !IsAttemptPossible())
        {
            targetZone.SetColor(outsideZoneColor);
            return;
        }
        targetZone.SetColor(startingColor);
    }

    bool IsAttemptPossible()
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
