using System;
using System.Text.Json.Serialization;

namespace Dota2Helper.Core.Gsi;

public class GameState
{
    [JsonIgnore]
    // ReSharper disable once InconsistentNaming
    private const string DOTA_GAMERULES_STATE_PRE_GAME = nameof(DOTA_GAMERULES_STATE_PRE_GAME);
    
    [JsonIgnore]
    // ReSharper disable once InconsistentNaming
    private const string DOTA_GAMERULES_STATE_GAME_IN_PROGRESS = nameof(DOTA_GAMERULES_STATE_GAME_IN_PROGRESS);
    
    [JsonPropertyName("map")]
    public Map? Map { get; set; }

    [JsonIgnore]
    public TimeSpan? GameTime
    {
        get
        {
            if (Map == null) return null;
            
            return Map.GameState switch
            {
                DOTA_GAMERULES_STATE_PRE_GAME => TimeSpan.FromSeconds(Map.ClockTime),
                DOTA_GAMERULES_STATE_GAME_IN_PROGRESS => TimeSpan.FromSeconds(Map.ClockTime),
                _ => null
            };
        }
    }
}