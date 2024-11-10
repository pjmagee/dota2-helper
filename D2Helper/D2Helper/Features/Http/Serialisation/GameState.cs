using System.Text.Json.Serialization;

namespace D2Helper.Features.Http.Serialisation;

[JsonSerializable(typeof(GameState))]
public class GameState
{
    [JsonPropertyName("map")]
    public Map? Map { get; set; }
}