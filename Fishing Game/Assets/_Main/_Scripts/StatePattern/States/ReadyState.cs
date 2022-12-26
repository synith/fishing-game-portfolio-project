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