using System.Threading;
using CommunityToolkit.Mvvm.Input;

namespace D2Helper.ViewModels;

public class SplashScreenViewModel : ViewModelBase
{
    readonly CancellationTokenSource _cts = new();

    public SplashScreenViewModel()
    {
        ContinueCommand = new RelayCommand(() =>
        {
            _cts.Cancel();
        });
    }

    public CancellationToken CancellationToken => _cts.Token;
    public IRelayCommand ContinueCommand { get; }
}