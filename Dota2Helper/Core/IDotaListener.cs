using System;
using System.Threading.Tasks;

namespace Dota2Helper.Core;

public interface IDotaListener : IDisposable
{
    Task<GameState?> GetStateAsync();
}