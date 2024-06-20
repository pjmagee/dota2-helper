using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dota2Helper.Core.Configuration;
using Dota2Helper.Core.Gsi;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dota2Helper.Core.Listeners;

public class DotaListener : IDotaListener
{
    private readonly ILogger<DotaListener> _logger;
    private readonly HttpListener _listener = new();
    private static JsonSerializerOptions _options = new() { WriteIndented = true };
    private readonly ManualResetEventSlim _resetEvent = new(false);
    
    private static GameState? _state;

    public DotaListener(ILogger<DotaListener> logger, IOptions<Settings> settings)
    {
        _logger = logger;
        _listener.Prefixes.Add(settings.Value.Address.AbsoluteUri);
    }

    public async Task<GameState?> GetStateAsync(CancellationToken cancellationToken)
    {  
        try
        {
            if (!_listener.IsListening)
            {
                _listener.Start();
            }
            
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(10)); // Set your timeout duration here
            
            var listenerContextTask = Task.Factory.FromAsync(
                _listener.BeginGetContext,
                _listener.EndGetContext,
                this, TaskCreationOptions.None);
            
            var delayTask = Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            var completedTask = await Task.WhenAny(listenerContextTask, delayTask);

            if (completedTask == delayTask)
            {
                _logger.LogError("The operation has timed out");
                return null;
            }
            
            HttpListenerContext? context = await listenerContextTask;
            
            try
            {
                using (var reader = new StreamReader(context.Request.InputStream))
                {
                    var json = reader.ReadToEnd();
                
                    _logger.LogDebug("{@Request}", context.Request);
                    _logger.LogDebug("{Body}", json);
                
                    _state = JsonSerializer.Deserialize<GameState>(json, _options);
                    context.Response.StatusCode = 200;
                    context.Response.Close();    
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Dota2 GSI data");

                try
                {
                    if (context != null)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.Close();
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            _logger.LogDebug("{@Message}", _state);
            
            return _state;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error processing Dota2 GSI data");
            return null;
        }
    }


    public void Dispose()
    {
        try
        {
            _listener.Close();
        }
        catch (Exception)
        {
            // ignored
        }
    }
}