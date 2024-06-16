namespace Dota2Helper.Core.Gsi;

public class GameStateHolder
{
    private GameState? _state;

    private readonly object _lock = new();
    
    public GameState? State
    {
        get
        {
            lock (_lock)
            {
                return _state;
            }
        }
        set
        {
            lock (_lock)
            {
                _state = value;
            }
        }
    }
}