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

    List<Difficulty> _difficultyList;
    Difficulty _currentDifficulty;

    Color _startingColor;
    float _currentRadius;
    bool _isShrinking;
    bool _hasAttempted;
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
    void FishingControls_OnDebugRestart(object sender, EventArgs e) => RestartFishing(_currentDifficulty);
    void FishTracker_OnActiveFishChanged(FishSO fish) => SetDifficultyListFromFish(fish);

    #endregion

    void Awake()
    {
        _difficultyList = new List<Difficulty>();
    }
    void Start()
    {
        FishSO startingFish = FishTracker.Instance.ActiveFish;
        SetDifficultyListFromFish(startingFish);

        _startingColor = _targetZone.GetColor();
    }
    void Update()
    {
        HandleShrinkingPlayerZone();

        void HandleShrinkingPlayerZone()
        {
            if (!_isShrinking) return;

            if (IsPlayerTooFarInside())
            {
                // Player took too long to press button
                AttemptToCatchFish();
                return;
            }

            _currentRadius -= SHRINK_RATE * Time.deltaTime;

            _playerZone.SetZone(_currentRadius, PLAYER_ZONE_THICKNESS);
            HandleZoneColor();
        }
    }

    void StartFishing()
    {
        if (_hasAttempted) return;

        _isShrinking = true;
        StartCoroutine(nameof(ShowAttemptPossible));

        FishingStartedFeedback?.PlayFeedbacks();
        OnFishingStart?.Invoke(this, EventArgs.Empty);

        HandleZoneColor();

    }
    void AttemptToCatchFish()
    {
        if (_hasAttempted || !_isShrinking) return;

        _hasAttempted = true;
        _isShrinking = false;

        StopCoroutine(nameof(ShowAttemptPossible));
        AttemptMadeFeedback?.PlayFeedbacks();


        bool isAttemptPossible = IsAttemptPossible();
        bool isFishOnLastDifficulty = _difficultyList.Count == 1;

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
                _difficultyList.Remove(_currentDifficulty);

                _currentDifficulty = GetNextDifficultyInList();

                RestartFishing(_currentDifficulty);

                Invoke(nameof(StartFishing), 1.5f);
            }
            if (!isPossible)
            {
                // failed
                AttemptFailedFeedback?.PlayFeedbacks();
                HandleZoneColor();

                _hasAttempted = true;

                FishTracker.Instance.SetRandomFishActive();
                _currentDifficulty = GetNextDifficultyInList();

                // RestartFishing(currentDifficulty);

                print("FAIL");
            }
        }
    }
    void RestartFishing(Difficulty difficulty)
    {
        if (!_hasAttempted) return;

        _isShrinking = false;
        _hasAttempted = false;

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
            _targetZone.SetColor(_startingColor);
            return;
        }
        else if (!_isShrinking && !IsAttemptPossible())
        {
            _targetZone.SetColor(_outsideZoneColor);
            return;
        }
        _targetZone.SetColor(_startingColor);
    }

    void SetDifficultyListFromFish(FishSO fish)
    {
        _difficultyList.Clear();

        foreach (Difficulty difficulty in fish.difficultyList)
        {
            _difficultyList.Add(difficulty);
        }

        _currentDifficulty = GetNextDifficultyInList();
        SetDifficulty(_currentDifficulty);
        _targetZone.SetZone(TARGET_ZONE_RADIUS, TARGET_ZONE_THICKNESS);
    }
    void SetDifficulty(Difficulty difficulty)
    {
        _currentDifficulty = difficulty;

        float startRadius = GetStartRadiusForDifficulty(difficulty);
        _currentRadius = startRadius;

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
        Difficulty difficulty = _difficultyList[0];
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
    bool IsNotStarted() => !_isShrinking && !_hasAttempted;
}
