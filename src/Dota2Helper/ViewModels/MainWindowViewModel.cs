using Avalonia;
using Avalonia.Styling;
using ReactiveUI;

namespace Dota2Helper.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    ViewModelBase _contentViewModel;



    public ViewModelBase ContentViewModel
    {
        get => _contentViewModel;
        private set
        {
            this.RaiseAndSetIfChanged(ref _contentViewModel, value);
            ViewName = _contentViewModel is TimersViewModel ? "Settings" : "Timers";
            this.RaisePropertyChanged(nameof(IsSettingsView));
            this.RaisePropertyChanged(nameof(IsTimersView));
        }
    }

    string? _viewName;

    public string? ViewName
    {
        get => _viewName;
        private set => this.RaiseAndSetIfChanged(ref _viewName, value);
    }

    public bool IsSettingsView => ContentViewModel is SettingsViewModel;

    public bool IsTimersView => ContentViewModel is TimersViewModel;

    public void ToggleView()
    {
        ContentViewModel = ContentViewModel is SettingsViewModel ? TimersViewModel : SettingsViewModel;
    }

    public MainWindowViewModel(TimersViewModel timersViewModel, SettingsViewModel settingsViewModel)
    {
        _contentViewModel = timersViewModel;
        ContentViewModel = timersViewModel;

        TimersViewModel = timersViewModel;
        SettingsViewModel = settingsViewModel;
    }

    public TimersViewModel TimersViewModel { get; set; }
    public SettingsViewModel SettingsViewModel { get; set; }
}