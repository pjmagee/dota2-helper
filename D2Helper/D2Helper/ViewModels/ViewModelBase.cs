using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using D2Helper.Features.Gsi;

namespace D2Helper.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    
}


public class SplashScreenViewModel : ViewModelBase
{
    readonly CancellationTokenSource _cts = new CancellationTokenSource();

    public string StartupMessage
    {
        get => _startupMessage;
        set => this.SetProperty(ref _startupMessage, value);
    }

    string _startupMessage = "Starting application...";

    public void Cancel()
    {
        StartupMessage = "Cancelling...";
        _cts.Cancel();
    }



    public CancellationToken CancellationToken => _cts.Token;
}
