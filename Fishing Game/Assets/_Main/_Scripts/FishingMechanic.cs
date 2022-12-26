using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingMechanic : MonoBehaviour
{
    #region Variables

    FishingStatePattern fishingStatePattern;

    public enum Difficulty { Easy, Medium, Hard };

    public static event Action<bool, bool> OnFishAttempt;
    public static event EventHandler OnFishingStart;
    public static event EventHandler OnFishingReady;
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
    [SerializeField] MMFeedbacks FishingReadyFeedback;

    const float SHRINK_RATE = 6;

    const float EASY_START_RADIUS = 6f;
    const float MEDIUM_START_RADIUS = 4.5f;
    const float HARD_START_RADIUS = 3f;

    const float TARGET_ZONE_RADIUS = 1f;
    const float TARGET_ZONE_THICKNESS = 0.7f;
    const float PLAYER_ZONE_THICKNESS = 0.2f;
    const float MIN_RADIUS = 0.1f;

    const float CAST_ANIMATION_COOLDOWN = 1.5f;

    List<Difficulty> _difficultyList;
    Difficulty _currentDifficulty;

    Color _startingColor;
    float _currentRadius;
    bool _isShrinking;
    bool _hasAttempted;

    FishingRodAnimation _fishingRodAnimation;
    FishSO _activeFish;

    #endregion

    

    #region Event Subscription

    void OnEnable()
    {
        FishingControls.Instance.OnFishAttemptPressed += FishingControls_OnFishAttempt;
        FishingControls.Instance.OnFishingRodCastPressed += FishingControls_OnCast;
        FishingControls.Instance.OnFishingRodRecastPressed += FishingControls_OnRecast;

        FishTracker.Instance.OnActiveFishChanged += FishTracker_OnActiveFishChanged;
    }
    void OnDisable()
    {
        FishingControls.Instance.OnFishAttemptPressed -= FishingControls_OnFishAttempt;
        FishingControls.Instance.OnFishingRodCastPressed -= FishingControls_OnCast;
        FishingControls.Instance.OnFishingRodRecastPressed -= FishingControls_OnRecast;

        FishTracker.Instance.OnActiveFishChanged -= FishTracker_OnActiveFishChanged;
    }
    void FishingControls_OnFishAttempt(object sender, EventArgs e) => OnButtonPress();
    void FishingControls_OnCast(object sender, EventArgs e) => OnButtonPress();


    public void Cast()
    {
        if (_isShrinking) return;
        StartCoroutine(nameof(ShrinkAfterCast));
        _isShrinking = true;
    }

    IEnumerator ShrinkAfterCast()
    {
        PlayCastAnimation();
        yield return new WaitForSeconds(CAST_ANIMATION_COOLDOWN);

        Difficulty nextDifficulty = GetNextDifficulty();
        ShrinkingState shrinkingState = new(this, nextDifficulty);

        fishingStatePattern.SetState(shrinkingState);
    }

    void PlayCastAnimation() => _fishingRodAnimation.PlayAnimation(FishingRodAnimation.AnimationType.Cast);

    void FishingControls_OnRecast(object sender, EventArgs e) => OnButtonPress();

    void FishTracker_OnActiveFishChanged(FishSO fish) => _activeFish = fish;

    #endregion



    void Awake()
    {
        _difficultyList = new List<Difficulty>();
        Time.timeScale = 1f;
    }
    void Start()
    {
        _startingColor = _targetZone.GetColor();

        ReadyState readyState = new(this);
        fishingStatePattern = new FishingStatePattern(readyState);
        fishingStatePattern.Enter();


        _fishingRodAnimation = FindObjectOfType<FishingRodAnimation>();
        
    }
    void Update()
    {
        fishingStatePattern.Tick();
    }

    void OnButtonPress() => fishingStatePattern.HandleButtonPress();

    public void HandleShrinking()
    {
        _currentRadius -= SHRINK_RATE * Time.deltaTime;

        _playerZone.SetZone(_currentRadius, PLAYER_ZONE_THICKNESS);
        
        HandleZoneColor();
    }
    public FishSO GetActiveFish() => _activeFish;

    public void StartFishing()
    {
        StartCoroutine(nameof(WaitUntilAttemptPossible));

        FishingStartedFeedback?.PlayFeedbacks();
        OnFishingStart?.Invoke(this, EventArgs.Empty);

        HandleZoneColor();
    }

    public void RemoveCurrentDifficulty()
    {
        _difficultyList.Remove(_currentDifficulty);
    }
    public void HandleFishCaught()
    {
        HandleZoneColor();
        FishCaughtFeedback?.PlayFeedbacks();

        FishSO fish = GetActiveFish();

        FishTracker.Instance.RecordFishCaught(fish);

        _isShrinking = false;
    }
    public void HandleFailed()
    {
        _isShrinking = false;
        _hasAttempted = false;

        AttemptFailedFeedback?.PlayFeedbacks();
        HandleZoneColor();
    }

    public void SetRandomFishActive() => FishTracker.Instance.SetRandomFishActive();
    public void PlayStruggleAnimation()
    {
        _fishingRodAnimation.PlayAnimation(FishingRodAnimation.AnimationType.Struggle);
    }

    public void HandleAttemptMade()
    {   
        StopCoroutine(nameof(WaitUntilAttemptPossible));
        AttemptMadeFeedback?.PlayFeedbacks();

        bool isAttemptPossible = IsAttemptPossible();
        bool isFishOnLastDifficulty = IsFishOnLastDifficulty();
        OnFishAttempt?.Invoke(isAttemptPossible, isFishOnLastDifficulty);        
    }
    public void ShowFishingReady()
    {
        FishingReadyFeedback?.PlayFeedbacks();
        OnFishingReady?.Invoke(this, EventArgs.Empty);
    }

    IEnumerator WaitUntilAttemptPossible()
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
        else if (!_isShrinking && !IsAttemptPossible())
        {
            _targetZone.SetColor(_outsideZoneColor);
            return;
        }
        _targetZone.SetColor(_startingColor);
    }

    public void SetDifficultyListFromFish(FishSO fish)
    {
        _difficultyList.Clear();

        foreach (Difficulty difficulty in fish.difficultyList)
        {
            _difficultyList.Add(difficulty);
        }

        SetTargetZone();
    }

    public void SetTargetZone() => _targetZone.SetZone(TARGET_ZONE_RADIUS, TARGET_ZONE_THICKNESS);

    public void SetDifficulty(Difficulty difficulty)
    {
        _currentDifficulty = difficulty;

        float startRadius = GetStartRadiusForDifficulty(difficulty);
        _currentRadius = startRadius;

        _playerZone.SetZone(startRadius, PLAYER_ZONE_THICKNESS);

        OnDifficultyChanged?.Invoke(this, difficulty);

        float GetStartRadiusForDifficulty(Difficulty difficulty) => difficulty switch
        {
            Difficulty.Easy => EASY_START_RADIUS,
            Difficulty.Medium => MEDIUM_START_RADIUS,
            Difficulty.Hard => HARD_START_RADIUS,
            _ => EASY_START_RADIUS,
        };
    }

    public Difficulty GetNextDifficulty()
    {
        Difficulty difficulty = _difficultyList[0];
        return difficulty;
    }

    public bool IsFishOnLastDifficulty() => _difficultyList.Count == 1;
    public bool IsAttemptPossible() => !IsPlayerTooFarInside() && !IsPlayerTooFarOutside();
    public bool IsPlayerTooFarInside() => _playerZone.GetOutsideRingRadius() < _targetZone.GetInsideRingRadius();
    bool IsPlayerTooFarOutside() => _playerZone.GetInsideRingRadius() > _targetZone.GetOutsideRingRadius();
    bool IsNotStarted() => !_isShrinking && !_hasAttempted;
}
