using System.Threading;
using CommunityToolkit.Mvvm.Input;
using Dota2Helper.Features.Gsi;

namespace Dota2Helper.ViewModels;


public class SplashScreenViewModel : ViewModelBase
{
    readonly GsiConfigService? _gsiConfigService;
    public IRelayCommand ContinueCommand { get; }
    public IRelayCommand WindowOpenedCommand { get; }

    readonly CancellationTokenSource _cts = new();

    public string Version { get; }

    public SplashScreenViewModel()
    {
        ContinueCommand = new RelayCommand(CloseSplash);
        WindowOpenedCommand = new RelayCommand(WindowOpened);
        Version = $"Version: {GitVersionInformation.MajorMinorPatch}";
        StatusText = "Starting...";
    }

    public SplashScreenViewModel(GsiConfigService gsiConfigService) : this()
    {
        _gsiConfigService = gsiConfigService;
    }

    string _statusText = "Starting...";

    public string StatusText
    {
        get => _statusText;
        set => SetProperty(ref _statusText, value);
    }

    void CloseSplash() => _cts.Cancel();

    public CancellationToken CancellationToken => _cts.Token;


    public void WindowOpened()
    {
        if (_gsiConfigService != null)
        {
            var text = _gsiConfigService.TryInstall() ? "GSI config installed successfully!" : "GSI config is already installed.";
            StatusText = $"{text} Click anywhere to continue.";
        }
    }
}