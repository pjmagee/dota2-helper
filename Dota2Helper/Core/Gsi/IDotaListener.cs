using System;
using System.Threading.Tasks;

namespace Dota2Helper.Core.Gsi;

public interface IDotaListener : IDisposable
{
    Task<GameState?> GetStateAsync();
}