using System.Collections.ObjectModel;
using Dota2Helper.Core.Timers;
using ReactiveUI;

namespace Dota2Helper.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private ObservableCollection<DotaTimer>? _timers;

    public ObservableCollection<DotaTimer>? Timers
    {
        get => _timers;
        set => this.RaiseAndSetIfChanged( ref _timers, value);
    }

    public SettingsViewModel(DotaTimers timers)
    {
        Timers = timers;
    }
}