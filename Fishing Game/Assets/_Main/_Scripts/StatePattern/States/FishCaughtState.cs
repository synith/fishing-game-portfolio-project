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