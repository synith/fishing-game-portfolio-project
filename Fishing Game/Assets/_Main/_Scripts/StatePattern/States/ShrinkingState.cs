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