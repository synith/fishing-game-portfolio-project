using UnityEngine;

public interface IFishingContext
{
    void SetState(IFishingState newState);
}

public interface IFishingState
{
    void Enter(IFishingContext context);
    void Tick(IFishingContext context);
    void Exit(IFishingContext context);
    void HandleButtonPress(IFishingContext context);
}

public class FishingStatePattern : IFishingContext
{
    IFishingState currentState;

    public FishingStatePattern(IFishingState currentState)
    {
        this.currentState = currentState;
    }

    public void Enter() => currentState.Enter(this);
    public void Tick() => currentState.Tick(this);
    public void Exit() => currentState.Exit(this);
    public void HandleButtonPress() => currentState.HandleButtonPress(this);

    void IFishingContext.SetState(IFishingState newState)
    {
        currentState.Exit(this);
        currentState = newState;
        currentState.Enter(this);
    }

    public void SetState(IFishingState newState)
    {
        IFishingContext context = this;
        context.SetState(newState);
    }
}
public class ReadyState : IFishingState
{
    FishingMechanic fishingMechanic;

    public ReadyState(FishingMechanic fishingMechanic)
    {
        this.fishingMechanic = fishingMechanic;
    }
    public void HandleButtonPress(IFishingContext context)
    {
        fishingMechanic.Cast();
    }
    public void Enter(IFishingContext context)
    {
        fishingMechanic.SetRandomFishActive();

        FishSO fish = fishingMechanic.GetActiveFish();
        fishingMechanic.SetDifficultyListFromFish(fish);

        fishingMechanic.ShowFishingReady();
    }
    public void Tick(IFishingContext context) { }
    public void Exit(IFishingContext context) { }

}
public class ShrinkingState : IFishingState
{
    FishingMechanic fishingMechanic;
    FishingMechanic.Difficulty difficulty;

    public ShrinkingState(FishingMechanic fishingMechanic, FishingMechanic.Difficulty difficulty)
    {
        this.fishingMechanic = fishingMechanic;
        this.difficulty = difficulty;
    }


    public void HandleButtonPress(IFishingContext context)
    {
        fishingMechanic.HandleAttemptMade();

        IFishingState nextState;

        if (fishingMechanic.IsAttemptPossible())
        {
            nextState = new SuccessState(fishingMechanic);
        }
        else
        {
            nextState = new FailState(fishingMechanic);
        }

        context.SetState(nextState);
    }


    public void Tick(IFishingContext context)
    {
        fishingMechanic.HandleShrinking();

        if (fishingMechanic.IsPlayerTooFarInside())
        {
            context.SetState(new FailState(fishingMechanic));
        }
    }

    public void Enter(IFishingContext context)
    {
        fishingMechanic.SetDifficulty(difficulty);


        fishingMechanic.StartFishing();
    }
    public void Exit(IFishingContext context) { }
}
public class SuccessState : IFishingState
{
    FishingMechanic fishingMechanic;

    float timer;
    float timerMax;
    IFishingState nextState;

    public SuccessState(FishingMechanic fishingMechanic)
    {
        this.fishingMechanic = fishingMechanic;
        timer = 0;
        timerMax = 2f;
    }
    public void HandleButtonPress(IFishingContext context) { }

    public void Enter(IFishingContext context)
    {
        fishingMechanic.PlayStruggleAnimation();

        IFishingState nextState;
        if (fishingMechanic.IsFishOnLastDifficulty())
        {
            timerMax = 0.5f;
            nextState = new FishCaughtState(fishingMechanic);
        }
        else
        {
            fishingMechanic.RemoveCurrentDifficulty();
            nextState = new ShrinkingState(fishingMechanic, fishingMechanic.GetNextDifficulty());
        }
        this.nextState = nextState;
    }


    public void Exit(IFishingContext context)
    {
        timer = 0;
    }

    public void Tick(IFishingContext context)
    {
        timer += Time.deltaTime;

        if (timer > timerMax)
        {
            context.SetState(nextState);
        }
    }
}
public class FailState : IFishingState
{
    FishingMechanic fishingMechanic;

    public FailState(FishingMechanic fishingMechanic)
    {
        this.fishingMechanic = fishingMechanic;
    }
    public void HandleButtonPress(IFishingContext context)
    {
        context.SetState(new ReadyState(fishingMechanic));
    }

    public void Enter(IFishingContext context)
    {
        fishingMechanic.HandleFailed();
    }

    public void Exit(IFishingContext context) { }

    public void Tick(IFishingContext context) { }
}
public class FishCaughtState : IFishingState
{
    FishingMechanic fishingMechanic;

    public FishCaughtState(FishingMechanic fishingMechanic)
    {
        this.fishingMechanic = fishingMechanic;
    }
    public void HandleButtonPress(IFishingContext context)
    {
        context.SetState(new ReadyState(fishingMechanic));
    }

    public void Enter(IFishingContext context)
    {
        fishingMechanic.HandleFishCaught();
    }

    public void Exit(IFishingContext context) { }

    public void Tick(IFishingContext context) { }
}
public class GameWonState : IFishingState
{
    public void HandleButtonPress(IFishingContext context) { }

    public void Enter(IFishingContext context)
    {
        // Play Game Over
    }

    public void Exit(IFishingContext context) { }

    public void Tick(IFishingContext context) { }
}
