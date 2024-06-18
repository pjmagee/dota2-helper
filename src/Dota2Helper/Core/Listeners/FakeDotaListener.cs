using System;
using System.Threading;
using System.Threading.Tasks;
using Dota2Helper.Core.Gsi;
using Microsoft.Extensions.Logging;

namespace Dota2Helper.Core.Listeners;

public class FakeDotaListener : IDotaListener, IAsyncDisposable
{
    private readonly ILogger<FakeDotaListener> _logger = null!;
    private readonly ITimer? _timer = null!;
    
    private TimeSpan _gameTime = new(hours: 0, minutes: 0, seconds: 0);

    public FakeDotaListener(ILogger<FakeDotaListener> logger) : this(TimeProvider.System)
    {
        _logger = logger;
    }

    private FakeDotaListener(TimeProvider provider)
    {
        _timer = provider.CreateTimer(Update, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    private void Update(object? state)
    {
        _gameTime = _gameTime.Add(TimeSpan.FromSeconds(4 ));
    }

    public Task<GameState?> GetStateAsync(CancellationToken cancellationToken)
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
        _timer?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_timer != null)
            {
                await _timer.DisposeAsync();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to dispose timer");
        }
    }
}