using System;

namespace Dota2Helper.Core.Gsi;

public class GameStateHolder
{
    
    private static readonly object Lock = new();
    
    private GameState? _state;

    public GameState? State
    {
        get
        {
            lock (Lock)
            {
                return _state;
            }
        }
        set
        {
            lock (Lock)
            {
                _state = value;
            }
        }
    }
}