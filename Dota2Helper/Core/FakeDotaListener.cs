using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dota2Helper.Core;

public class FakeDotaListener : IDotaListener, IAsyncDisposable
{
    private readonly TimeProvider _provider;
    private TimeProvider provider;
    private ITimer? timer;
    
    private TimeSpan _gameTime = new(hours: 0, minutes: 0, seconds: 0);

    public FakeDotaListener() : this(TimeProvider.System)
    {
        
    }

    public FakeDotaListener(TimeProvider provider)
    {
        _provider = provider;
        timer = _provider.CreateTimer(Update, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    private void Update(object? state)
    {
        _gameTime = _gameTime.Add(TimeSpan.FromSeconds(4 ));
    }

    public Task<GameState?> GetStateAsync()
    {
        var fakeState = new GameState()
        {
            Map = new Map()
            {
                GameTime = (int) _gameTime.TotalSeconds,
                ClockTime = (int) _gameTime.TotalSeconds
            }
        };


        return Task.FromResult<GameState?>(fakeState);
    }

    public void Dispose()
    {
        timer?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (timer != null) await timer.DisposeAsync();
    }
}