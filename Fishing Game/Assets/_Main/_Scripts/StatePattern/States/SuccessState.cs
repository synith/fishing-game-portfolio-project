using UnityEngine;

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