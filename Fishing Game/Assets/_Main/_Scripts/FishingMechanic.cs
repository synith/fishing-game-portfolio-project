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

    List<Difficulty> difficultyList;
    Difficulty currentDifficulty;

    Color startingColor;
    float currentRadius;
    bool isShrinking;
    bool hasAttempted;

    FishingRodAnimation fishingRodAnimation;
    FishSO activeFish;

    #endregion
        

    #region Event Subscription

    void OnEnable()
    {
        FishingControls.Instance.OnButtonPressed += FishingControls_OnButtonPressed;

        FishTracker.Instance.OnActiveFishChanged += FishTracker_OnActiveFishChanged;
    }

    void OnDisable()
    {
        FishingControls.Instance.OnButtonPressed -= FishingControls_OnButtonPressed;

        FishTracker.Instance.OnActiveFishChanged -= FishTracker_OnActiveFishChanged;
    }

    void FishingControls_OnButtonPressed(object sender, EventArgs e) => ButtonPressed();
    void FishTracker_OnActiveFishChanged(FishSO fish) => activeFish = fish;

    #endregion


    void Awake()
    {
        difficultyList = new List<Difficulty>();
        Time.timeScale = 1f;
    }
    void Start()
    {
        startingColor = _targetZone.GetColor();

        ReadyState readyState = new(this);
        fishingStatePattern = new FishingStatePattern(readyState);
        fishingStatePattern.Enter();


        fishingRodAnimation = FindObjectOfType<FishingRodAnimation>();
        
    }
    void Update()
    {
        fishingStatePattern.Tick();
    }




    public void ButtonPressed() => fishingStatePattern.HandleButtonPress();

    public void Cast()
    {
        if (isShrinking) return;
        StartCoroutine(nameof(ShrinkAfterCast));
        isShrinking = true;
    }
    public void HandleShrinking()
    {
        currentRadius -= SHRINK_RATE * Time.deltaTime;

        _playerZone.SetZone(currentRadius, PLAYER_ZONE_THICKNESS);
        
        HandleZoneColor();
    }
    public FishSO GetActiveFish() => activeFish;
    public void StartFishing()
    {
        StartCoroutine(nameof(WaitUntilAttemptPossible));

        FishingStartedFeedback?.PlayFeedbacks();
        OnFishingStart?.Invoke(this, EventArgs.Empty);

        HandleZoneColor();
    }
    public void RemoveCurrentDifficulty()
    {
        difficultyList.Remove(currentDifficulty);
    }
    public void HandleFishCaught()
    {
        HandleZoneColor();
        FishCaughtFeedback?.PlayFeedbacks();

        FishSO fish = GetActiveFish();

        FishTracker.Instance.RecordFishCaught(fish);

        isShrinking = false;
    }
    public void HandleFailed()
    {
        isShrinking = false;
        hasAttempted = false;

        AttemptFailedFeedback?.PlayFeedbacks();
        HandleZoneColor();
    }
    public void SetRandomFishActive() => FishTracker.Instance.SetRandomFishActive();
    public void PlayStruggleAnimation()
    {
        fishingRodAnimation.PlayAnimation(FishingRodAnimation.AnimationType.Struggle);
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
    public void SetDifficultyListFromFish(FishSO fish)
    {
        difficultyList.Clear();

        foreach (Difficulty difficulty in fish.difficultyList)
        {
            difficultyList.Add(difficulty);
        }

        SetTargetZone();
    }
    public void SetTargetZone() => _targetZone.SetZone(TARGET_ZONE_RADIUS, TARGET_ZONE_THICKNESS);
    public void SetDifficulty(Difficulty difficulty)
    {
        currentDifficulty = difficulty;

        float startRadius = GetStartRadiusForDifficulty(difficulty);
        currentRadius = startRadius;

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
        Difficulty difficulty = difficultyList[0];
        return difficulty;
    }
    public bool IsFishOnLastDifficulty() => difficultyList.Count == 1;
    public bool IsAttemptPossible() => !IsPlayerTooFarInside() && !IsPlayerTooFarOutside();
    public bool IsPlayerTooFarInside() => _playerZone.GetOutsideRingRadius() < _targetZone.GetInsideRingRadius();

    IEnumerator ShrinkAfterCast()
    {
        PlayCastAnimation();
        yield return new WaitForSeconds(CAST_ANIMATION_COOLDOWN);

        Difficulty nextDifficulty = GetNextDifficulty();
        ShrinkingState shrinkingState = new(this, nextDifficulty);

        fishingStatePattern.SetState(shrinkingState);
    }
    void PlayCastAnimation() => fishingRodAnimation.PlayAnimation(FishingRodAnimation.AnimationType.Cast);
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
        else if (!isShrinking && !IsAttemptPossible())
        {
            _targetZone.SetColor(_outsideZoneColor);
            return;
        }
        _targetZone.SetColor(startingColor);
    }
    bool IsPlayerTooFarOutside() => _playerZone.GetInsideRingRadius() > _targetZone.GetOutsideRingRadius();
    bool IsNotStarted() => !isShrinking && !hasAttempted;
}
