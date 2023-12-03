using System.Text.Json.Serialization;

namespace Dota2Helper.Core;

public class Map
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "start";

    [JsonPropertyName("matchid")]
    public string MatchId { get; set; } = "0";

    [JsonPropertyName("clock_time")]
    public int ClockTime { get; set; } = 460;

    [JsonPropertyName("game_time")]
    public int GameTime { get; set; } = 230;

    [JsonPropertyName("daytime")]
    public bool Daytime { get; set; } = false;

    [JsonPropertyName("nightstalker_night")]
    public bool NightStalkerNight { get; set; } = false;

    [JsonPropertyName("radiant_score")]
    public int RadiantScore { get; set; } = 0;

    [JsonPropertyName("dire_score")]
    public int DireScore { get; set; } = 3;

    [JsonPropertyName("game_state")]
    public string GameState { get; set; } = "DOTA_GAMERULES_STATE_GAME_IN_PROGRESS";
}