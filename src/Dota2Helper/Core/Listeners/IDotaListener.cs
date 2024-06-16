using System;
using System.Threading.Tasks;
using Dota2Helper.Core.Gsi;

namespace Dota2Helper.Core.Listeners;

public interface IDotaListener : IDisposable
{
    Task<GameState?> GetStateAsync();
}