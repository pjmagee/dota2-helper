using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text.Json;
using D2Helper.Features.Gsi;
using D2Helper.Features.Http.Serialisation;
using D2Helper.Features.TimeProvider;
using D2Helper.ViewModels;

namespace D2Helper.Features.Http;

public class LocalListener : BackgroundWorker
{
    readonly RealProvider _realProvider;
    readonly GsiConfigWatcher _configWatcher;
    readonly GsiConfigService _configurationService;
    readonly SettingsViewModel _settingsViewModel;

    HttpListener? _listener;

    public LocalListener(
        RealProvider realProvider,
        GsiConfigWatcher configWatcher,
        GsiConfigService configurationService,
        SettingsViewModel settingsViewModel)
    {
        _realProvider = realProvider;
        _configurationService = configurationService;
        _settingsViewModel = settingsViewModel;
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

        while (!CancellationPending)
        {
            try
            {
                _settingsViewModel.IsListening = _listener?.IsListening ?? false;

                if (_listener is not null && _listener.IsListening)
                {
                    var context = _listener.GetContext();
                    var request = context.Request;
                    var response = context.Response;

                    if (request.HttpMethod == "POST")
                    {
                        using var reader = new StreamReader(request.InputStream);
                        var json = reader.ReadToEnd();
                        var gameState = JsonSerializer.Deserialize<GameState>(json)!;
                        _realProvider.Time = gameState.GameTime();
                    }

                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Close();
                }
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