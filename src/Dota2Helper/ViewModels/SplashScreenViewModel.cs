using System;
using System.Threading;
using CommunityToolkit.Mvvm.Input;

namespace Dota2Helper.ViewModels;

public class SplashScreenViewModel : ViewModelBase
{
    public IRelayCommand ContinueCommand { get; }

    readonly CancellationTokenSource _cts = new();

    public string Version { get; }

    public SplashScreenViewModel()
    {
        ContinueCommand = new RelayCommand(CloseSplash);
        Version = $"Version: {GitVersionInformation.MajorMinorPatch}";
    }

    void CloseSplash() => _cts.Cancel();
    public CancellationToken CancellationToken => _cts.Token;

}