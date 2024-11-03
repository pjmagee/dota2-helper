namespace D2Helper.Services;

public class FakeGameStateService : GameStateService
{
    public FakeGameStateService()
    {
        RunWorkerAsync();
    }
}