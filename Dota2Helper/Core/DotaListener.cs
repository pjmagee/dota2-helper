using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dota2Helper.Core;

public class DotaListener : IDotaListener
{
    private readonly HttpListener _listener = new(){  };

    public DotaListener()
    {
        _listener.Prefixes.Add("http://localhost:4001/");
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
        catch (Exception e)
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