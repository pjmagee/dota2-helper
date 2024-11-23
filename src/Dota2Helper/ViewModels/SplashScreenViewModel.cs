using System.Threading;
using CommunityToolkit.Mvvm.Input;

namespace Dota2Helper.ViewModels;

public class SplashScreenViewModel : ViewModelBase
{
    public IRelayCommand ContinueCommand { get; }

    readonly CancellationTokenSource _cts = new();

    public SplashScreenViewModel()
    {
        ContinueCommand = new RelayCommand(CloseSplash);
    }

    void CloseSplash() => _cts.Cancel();
    public CancellationToken CancellationToken => _cts.Token;
}