using System;

namespace D2Helper.Features.Http.Serialisation;

public static class GameStateExtensions
{
    public static TimeSpan GameTime(this GameState state)
    {
        if (state.Map == null) return TimeSpan.Zero;

        return state.Map.GameState switch
        {
            DotaGameStates.DOTA_GAMERULES_STATE_PRE_GAME => TimeSpan.FromSeconds(state.Map.ClockTime),
            DotaGameStates.DOTA_GAMERULES_STATE_GAME_IN_PROGRESS => TimeSpan.FromSeconds(state.Map.ClockTime),
            _ => TimeSpan.Zero
        };
    }
}