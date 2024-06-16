using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Dota2Helper.Core.Configuration;
using Dota2Helper.Core.Gsi;
using Microsoft.Extensions.Options;

namespace Dota2Helper.Core.Listeners;

public class DotaListener : IDotaListener
{
    private readonly HttpListener _listener = new();

    public DotaListener(IOptions<Settings> settings)
    {
        _listener.Prefixes.Add(settings.Value.Address.AbsoluteUri);
    }

    public async Task<GameState?> GetStateAsync()
    {
        if (!_listener.IsListening)
        {
            _listener.Start();
        }
        
        try
        {
            HttpListenerContext context = await _listener.GetContextAsync();
            GameState? state = await JsonSerializer.DeserializeAsync<GameState>(context.Request.InputStream);
            
            context.Response.StatusCode = 200;
            context.Response.Close();

            return state;
        }
        catch (Exception)
        {
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