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

    [SerializeField] Zone _playerZone;
    [SerializeField] Zone _targetZone;

    [SerializeField] Color _insideZoneColor;
    [SerializeField] Color _outsideZoneColor;

    [SerializeField] MMFeedbacks AttemptPossibleFeedback;
    [SerializeField] MMFeedbacks AttemptMadeFeedback;
    [SerializeField] MMFeedbacks AttemptFailedFeedback;
    [SerializeField] MMFeedbacks FishingStartedFeedback;
    [SerializeField] MMFeedbacks FishCaughtFeedback;

    const float SHRINK_RATE = 5;

    const float EASY_START_RADIUS = 6f;
    const float MEDIUM_START_RADIUS = 4.5f;
    const float HARD_START_RADIUS = 3f;

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

    #region Event Subscription
    void OnEnable()
    {
        FishingControls.Instance.OnFishAttemptPressed += FishingControls_OnFishAttempt;
        FishingControls.Instance.OnDebugTestPressed += FishingControls_OnDebugTest;
        FishingControls.Instance.OnDebugRestartPressed += FishingControls_OnDebugRestart;

        FishTracker.Instance.OnActiveFishChanged += FishTracker_OnActiveFishChanged;
    }
    void OnDisable()
    {
        FishingControls.Instance.OnFishAttemptPressed -= FishingControls_OnFishAttempt;
        FishingControls.Instance.OnDebugTestPressed -= FishingControls_OnDebugTest;
        FishingControls.Instance.OnDebugRestartPressed -= FishingControls_OnDebugRestart;

        FishTracker.Instance.OnActiveFishChanged -= FishTracker_OnActiveFishChanged;
    }
    void FishingControls_OnFishAttempt(object sender, EventArgs e) => AttemptToCatchFish();
    void FishingControls_OnDebugTest(object sender, EventArgs e) => StartFishing();
    void FishingControls_OnDebugRestart(object sender, EventArgs e) => RestartFishing(currentDifficulty);
    void FishTracker_OnActiveFishChanged(FishSO fish) => SetDifficultyListFromFish(fish);

    #endregion

    void Awake()
    {
        difficultyList = new List<Difficulty>();
    }
    void Start()
    {
        FishSO startingFish = FishTracker.Instance.ActiveFish;
        SetDifficultyListFromFish(startingFish);

        startingColor = _targetZone.GetColor();
    }
    void Update()
    {
        HandleShrinkingPlayerZone();

        void HandleShrinkingPlayerZone()
        {
            if (!isShrinking) return;

            if (IsPlayerTooFarInside())
            {
                // Player took too long to press button
                AttemptToCatchFish();
                return;
            }

            currentRadius -= SHRINK_RATE * Time.deltaTime;

            _playerZone.SetZone(currentRadius, PLAYER_ZONE_THICKNESS);
            HandleZoneColor();
        }
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
    void AttemptToCatchFish()
    {
        if (hasAttempted || !isShrinking) return;

        hasAttempted = true;
        isShrinking = false;

        StopCoroutine(nameof(ShowAttemptPossible));
        AttemptMadeFeedback?.PlayFeedbacks();


        bool isAttemptPossible = IsAttemptPossible();
        bool isFishOnLastDifficulty = difficultyList.Count == 1;

        TryCatchFish(isAttemptPossible, isFishOnLastDifficulty);
        OnFishAttempt?.Invoke(isAttemptPossible, isFishOnLastDifficulty);

        void TryCatchFish(bool isPossible, bool isLastDifficulty)
        {
            if (isPossible && isLastDifficulty)
            {
                // fish is caught
                HandleZoneColor();
                FishCaughtFeedback?.PlayFeedbacks();
                FishTracker.Instance.SetRandomFishActive();
            }
            if (isPossible && !isLastDifficulty)
            {
                // needs more attempts
                difficultyList.Remove(currentDifficulty);

                currentDifficulty = GetNextDifficultyInList();

                RestartFishing(currentDifficulty);

                Invoke(nameof(StartFishing), 1.5f);
            }
            if (!isPossible)
            {
                // failed
                AttemptFailedFeedback?.PlayFeedbacks();
                HandleZoneColor();

                hasAttempted = true;

                FishTracker.Instance.SetRandomFishActive();
                currentDifficulty = GetNextDifficultyInList();

                // RestartFishing(currentDifficulty);

                print("FAIL");
            }
        }
    }
    void RestartFishing(Difficulty difficulty)
    {
        if (!hasAttempted) return;

        isShrinking = false;
        hasAttempted = false;

        SetDifficulty(difficulty);

        OnFishingRestart?.Invoke(this, EventArgs.Empty);
    }

    IEnumerator ShowAttemptPossible()
    {
        yield return new WaitUntil(() => IsAttemptPossible());
        AttemptPossibleFeedback?.PlayFeedbacks();
    }



    
    void HandleZoneColor()
    {
        if (IsAttemptPossible())
        {
            _targetZone.SetColor(_insideZoneColor);
            return;
        }
        else if (IsPlayerTooFarInside())
        {
            _targetZone.SetColor(_outsideZoneColor);
            return;
        }
        else if (IsNotStarted())
        {
            _targetZone.SetColor(startingColor);
            return;
        }
        else if (!isShrinking && !IsAttemptPossible())
        {
            _targetZone.SetColor(_outsideZoneColor);
            return;
        }
        _targetZone.SetColor(startingColor);
    }

    void SetDifficultyListFromFish(FishSO fish)
    {
        difficultyList.Clear();

        foreach (Difficulty difficulty in fish.difficultyList)
        {
            difficultyList.Add(difficulty);
        }

        currentDifficulty = GetNextDifficultyInList();
        SetDifficulty(currentDifficulty);
        _targetZone.SetZone(TARGET_ZONE_RADIUS, TARGET_ZONE_THICKNESS);
    }
    void SetDifficulty(Difficulty difficulty)
    {
        currentDifficulty = difficulty;

        float startRadius = GetStartRadiusForDifficulty(difficulty);
        currentRadius = startRadius;

        _playerZone.SetZone(startRadius, PLAYER_ZONE_THICKNESS);

        OnDifficultyChanged?.Invoke(this, difficulty);

        float GetStartRadiusForDifficulty(Difficulty difficulty)
        {
            switch (difficulty)
            {
                case Difficulty.Easy: return EASY_START_RADIUS;
                case Difficulty.Medium: return MEDIUM_START_RADIUS;
                case Difficulty.Hard: return HARD_START_RADIUS;
                default: return EASY_START_RADIUS;
            }
        }
    }

    Difficulty GetNextDifficultyInList()
    {
        Difficulty difficulty = difficultyList[0];
        return difficulty;
    }


    bool IsAttemptPossible()
    {
        if (IsPlayerTooFarInside()) return false;
        if (IsPlayerTooFarOutside()) return false;

        return true;
    }
    bool IsPlayerTooFarInside() => _playerZone.GetOutsideRingRadius() < _targetZone.GetInsideRingRadius();
    bool IsPlayerTooFarOutside() => _playerZone.GetInsideRingRadius() > _targetZone.GetOutsideRingRadius();
    bool IsNotStarted() => !isShrinking && !hasAttempted;
}
