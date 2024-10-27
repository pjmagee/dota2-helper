using System;
using System.IO;
using System.Net;
using System.Reactive.Disposables;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Dota2Helper.Core.Configuration;
using Dota2Helper.Core.Gsi;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dota2Helper.Core.Listeners;

public class DotaListener(ILogger<DotaListener> logger, IOptions<Settings> settings, GsiConfigService gsiConfig) : IDotaListener
{
    HttpListener? _listener;

    readonly SemaphoreSlim _semaphore = new(1, 1);

    readonly static JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    static GameState? _state;

    public async Task<GameState?> GetStateAsync(CancellationToken ct = default)
    {
        try
        {
            await _semaphore.WaitAsync(ct);

            if (_listener == null)
            {
                _listener = new HttpListener();
                _listener.Prefixes.Add(gsiConfig.GetUri().ToString());
                _listener.Start();
            }

            try
            {
                var resp = await _listener.GetContextAsync().WaitAsync(ct);
                _state = JsonSerializer.Deserialize<GameState>(resp.Request.InputStream, Options);

                /*
                 * timeout: Game expects an HTTP 2XX response code from its HTTP POST request
                 * and game will not attempt submitting the next HTTP POST request while a previous request is still in flight.
                 * This response is sent to the game to unblock it and is important to get consistent updates.
                 */
                resp.Response.StatusCode = (int)HttpStatusCode.OK;
                resp.Response.Close();

                logger.LogDebug("{@Message}", _state);
            }
            catch (ObjectDisposedException e)
            {
                logger.LogWarning(e, "Listener disposed");
                _listener = null;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error processing Dota2 GSI data");
            }

            return _state;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error processing Dota2 GSI data");
            return null;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Dispose()
    {
        logger.LogInformation("Disposing DotaListener");

        try
        {
            _listener?.Stop();
            _listener?.Close();
        }
        finally
        {
            _listener = null;
        }
    }
}