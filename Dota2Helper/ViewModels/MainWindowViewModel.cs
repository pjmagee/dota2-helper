using Avalonia;
using Avalonia.Styling;
using ReactiveUI;

namespace Dota2Helper.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _contentViewModel;
    
    public ViewModelBase ContentViewModel
    {
        get => _contentViewModel;
        private set
        {
            this.RaiseAndSetIfChanged(ref _contentViewModel, value);
            ViewName = ContentViewModel is TimersViewModel ? "Settings" : "Timers";
        }
    }

    public void ToggleTheme()
    {
        var toggle = Application.Current!.RequestedThemeVariant switch
        {
            { Key: nameof(ThemeVariant.Light) }  => ThemeVariant.Dark,
            { Key: nameof(ThemeVariant.Dark) } => ThemeVariant.Light,
            _ => null,
        };
        
        Application.Current!.RequestedThemeVariant = toggle;
        ThemeName = (toggle == ThemeVariant.Dark ? ThemeVariant.Light : ThemeVariant.Dark).Key.ToString();
    }

    private string? _themeName;
    public string? ThemeName
    {
        get => _themeName;
        set => this.RaiseAndSetIfChanged(ref _themeName, value);
    }
    
    private string? _viewName;
    public string? ViewName
    {
        get => _viewName;
        private set => this.RaiseAndSetIfChanged(ref _viewName, value);
    }
    
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
        
        ThemeName = Application.Current!.ActualThemeVariant.Key.ToString();
    }
    
    public TimersViewModel TimersViewModel { get; set; }
    public SettingsViewModel SettingsViewModel { get; set; }
}