using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text.Json;

namespace D2Helper.Services;

public class LongLivedHttpListener : BackgroundWorker, IDisposable
{
    readonly RealGameTimeProvider _gameTimeProvider;
    readonly Dota2ConfigurationService _configurationService;

    readonly HttpListener _listener;
    // a static instance of the HTTP listener for the entire lifetime of the application

    public LongLivedHttpListener(RealGameTimeProvider gameTimeProvider, Dota2ConfigurationService configurationService)
    {
        _gameTimeProvider = gameTimeProvider;
        _configurationService = configurationService;

        _listener = new HttpListener();
        _listener.Prefixes.Add(configurationService.HttpListenerPrefix);
        _listener.Start();

        RunWorkerAsync();
    }

    protected override void OnDoWork(DoWorkEventArgs e)
    {
        while (!CancellationPending)
        {
            var context = _listener.GetContext();
            var request = context.Request;
            var response = context.Response;

            if (request.HttpMethod == "POST")
            {
                using var reader = new StreamReader(request.InputStream);
                var json = reader.ReadToEnd();
                var gameState = JsonSerializer.Deserialize<GameState>(json);
                // TODO:  //  gameState.Time;
                _gameTimeProvider.Time = DateTime.Now.TimeOfDay;
            }

            response.StatusCode = 200;
            response.Close();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            try
            {
                _listener.Stop();
                _listener.Close();
            }
            catch
            {
                // ignored
            }
        }
    }
}