using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text.Json;
using Dota2Helper.Features.Gsi;
using Dota2Helper.Features.Http.Serialisation;
using Dota2Helper.Features.TimeProvider;
using Dota2Helper.ViewModels;

namespace Dota2Helper.Features.Http;

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
            _settingsViewModel.IsListening = false;

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
                    _settingsViewModel.IsListening = true;

                    var context = _listener.GetContext();
                    var gameState = JsonSerializer.Deserialize<GameState>(context.Request.InputStream)!;
                    _realProvider.Time = gameState.GameTime();
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.Close();

                    _settingsViewModel.LatestUpdateTime = DateTime.Now;
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