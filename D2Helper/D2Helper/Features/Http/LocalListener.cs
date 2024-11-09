using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text.Json;
using D2Helper.Features.Gsi;
using D2Helper.Features.Http.Serialisation;
using D2Helper.Features.TimeProvider;

namespace D2Helper.Features.Http;

public class LocalListener : BackgroundWorker
{
    readonly GameTimeProvider _gameTimeProvider;
    readonly GsiConfigWatcher _configWatcher;
    readonly GsiConfigService _configurationService;

    HttpListener? _listener;

    public LocalListener(
        GameTimeProvider gameTimeProvider,
        GsiConfigWatcher configWatcher,
        GsiConfigService configurationService)
    {
        _gameTimeProvider = gameTimeProvider;
        _configurationService = configurationService;
        _configWatcher = configWatcher;
        _configWatcher.Changed += OnFileSystemWatcherOnChanged;

        RunWorkerAsync();
    }

    void OnFileSystemWatcherOnChanged(object sender, FileSystemEventArgs args)
    {
        if (args.FullPath == _configurationService.ConfigFilePath)
        {
            if (_listener is not null && _listener.IsListening)
            {
                _listener.Stop();
                _listener.Close();

                _listener = new HttpListener();
                _listener.Prefixes.Add(_configurationService.GetUri().ToString());
                _listener.Start();
            }
        }
    }

    protected override void OnDoWork(DoWorkEventArgs e)
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add(_configurationService.GetUri().ToString());
        _listener.Start();

        while (!CancellationPending && _listener is not null)
        {
            try
            {
                var context = _listener.GetContext();
                var request = context.Request;
                var response = context.Response;

                if (request.HttpMethod == "POST")
                {
                    using var reader = new StreamReader(request.InputStream);
                    var json = reader.ReadToEnd();
                    var gameState = JsonSerializer.Deserialize<GameState>(json)!;
                    _gameTimeProvider.Time = gameState.GameTime();
                }

                response.StatusCode = (int)HttpStatusCode.OK;
                response.Close();
            }
            catch
            {
                // ignored
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            try
            {
                _listener?.Stop();
                _listener?.Close();
            }
            catch
            {
                // ignored
            }
        }
    }
}