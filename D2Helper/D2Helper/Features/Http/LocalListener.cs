using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
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
        _configWatcher.Changed += GameStateFolderChanged;
    }

    void GameStateFolderChanged(object sender, FileSystemEventArgs args)
    {
        if (args.FullPath == _configurationService.ConfigFilePath)
        {
            TryCreateOrRenewListener();
        }
    }

    void TryCreateOrRenewListener()
    {
        if (_listener is not null && _listener.IsListening)
        {
            _listener.Stop();
            _listener.Close();

            CreateAndStartListener();
        }
        else if (_listener is null)
        {
            CreateAndStartListener();
        }
    }

    void CreateAndStartListener()
    {
        if (_configurationService.IsIntegrationInstalled())
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(_configurationService.GetUri().ToString());
            _listener.Start();
        }
    }

    protected override void OnDoWork(DoWorkEventArgs eventArgs)
    {
        TryCreateOrRenewListener();

        while (!CancellationPending)
        {
            try
            {
                if (_listener is not null && _listener.IsListening)
                {
                    var context = _listener.GetContext();
                    var gameState = JsonSerializer.Deserialize<GameState>(context.Request.InputStream)!;
                    _realProvider.Time = gameState.GameTime();
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.Close();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
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