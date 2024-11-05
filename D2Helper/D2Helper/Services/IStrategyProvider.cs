using D2Helper.Models;

namespace D2Helper.Services;

public interface IStrategyProvider
{
    public GameStateStrategy Strategy { get; set; }
}