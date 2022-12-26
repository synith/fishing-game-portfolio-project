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



public interface IFishingState
{
    void Enter(IFishingContext context);
    void Tick(IFishingContext context);
    void Exit(IFishingContext context);
    void HandleButtonPress(IFishingContext context);
}



public interface IFishingContext
{
    void SetState(IFishingState newState);
}