using System;
using System.Threading;
using System.Threading.Tasks;
using Dota2Helper.Core.Gsi;
using Microsoft.Extensions.Logging;

namespace Dota2Helper.Core.Listeners;

public class FakeDotaListener : IDotaListener, IAsyncDisposable
{
    readonly ILogger<FakeDotaListener> _logger = null!;
    readonly ITimer? _timer = null!;

    TimeSpan _gameTime = new(hours: 0, minutes: 0, seconds: 0);

    public FakeDotaListener(ILogger<FakeDotaListener> logger) : this(TimeProvider.System)
    {
        _logger = logger;
    }

    FakeDotaListener(TimeProvider provider)
    {
        _timer = provider.CreateTimer(Update, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    void Update(object? state)
    {
        _gameTime = _gameTime.Add(TimeSpan.FromSeconds(4 ));
    }

    public Task<GameState?> GetStateAsync(CancellationToken ct)
    {
        var fakeState = new GameState()
        {
            Map = new Map()
            {
                GameTime = (int) _gameTime.TotalSeconds,
                ClockTime = (int) _gameTime.TotalSeconds,
                MatchId = "FAKE_MATCH_ID",
            }
        };

        return Task.FromResult<GameState?>(fakeState);
    }

    public void Dispose()
    {
        _logger.LogInformation("Disposing FakeDotaListener");
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