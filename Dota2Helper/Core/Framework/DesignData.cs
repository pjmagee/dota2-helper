using Avalonia;
using Dota2Helper.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Dota2Helper.Core.Framework;

public static class DesignData
{
    public static MainWindowViewModel MainWindowViewModel { get; } = ((App)Application.Current!).Host!.Services.GetRequiredService<MainWindowViewModel>();
    
    public static TimersViewModel TimersViewModel { get; } = ((App)Application.Current!).Host!.Services.GetRequiredService<TimersViewModel>();
    
    public static SettingsViewModel SettingsViewModel { get; } = ((App)Application.Current!).Host!.Services.GetRequiredService<SettingsViewModel>();
}