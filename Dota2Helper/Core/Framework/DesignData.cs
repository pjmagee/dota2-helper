using Avalonia;
using Dota2Helper.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Dota2Helper.Core.Framework;

public static class DesignData
{
    public static MainViewModel MainViewModel { get; } = ((App)Application.Current!).Host!.Services.GetRequiredService<MainViewModel>();
}