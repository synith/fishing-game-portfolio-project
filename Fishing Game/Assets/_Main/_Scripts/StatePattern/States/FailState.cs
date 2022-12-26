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