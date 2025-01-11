using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dota2Helper.Features.Gsi;
using Dota2Helper.Features.Http.Serialisation;
using Dota2Helper.Features.TimeProvider;
using Dota2Helper.ViewModels;
using Microsoft.Extensions.Hosting;

namespace Dota2Helper.Features.Http;

public class LocalListener : BackgroundService
{
    readonly RealGameTimeProvider _realGameTimeProvider;
    readonly GsiConfigWatcher _configWatcher;
    readonly GsiConfigService _configurationService;
    readonly SettingsViewModel _settingsViewModel;

    HttpListener? _listener;

    public LocalListener(
        RealGameTimeProvider realGameTimeProvider,
        GsiConfigWatcher configWatcher,
        GsiConfigService configurationService,
        SettingsViewModel settingsViewModel)
    {
        _realGameTimeProvider = realGameTimeProvider;
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

    public void ManualStart()
    {
        TryCreateOrRenewListener();

        if (_listener is not null)
        {
            _settingsViewModel.IsListening = _listener.IsListening;
        }
    }

    public void ManualStop()
    {
        if (_listener is not null)
        {
            _listener.Stop();
            _listener.Close();
            _listener = null;
            _settingsViewModel.IsListening = false;
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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        TryCreateOrRenewListener();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_listener is not null && _listener.IsListening)
                {
                    _settingsViewModel.IsListening = true;

                    var context = await _listener.GetContextAsync();
                    var gameState = JsonSerializer.Deserialize<GameState>(context.Request.InputStream)!;
                    _realGameTimeProvider.Time = gameState.GameTime();
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.Close();

                    _settingsViewModel.LatestUpdateTime = DateTime.Now;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            await Task.Delay(100, stoppingToken);
        }
    }

    public override void Dispose()
    {
        if (_listener is not null)
        {
            try
            {
                _listener?.Stop();
                _listener?.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        base.Dispose();
    }
}