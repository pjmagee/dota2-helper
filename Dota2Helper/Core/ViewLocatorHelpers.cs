using Avalonia.Controls;
using Dota2Helper.Core;
using Dota2Helper.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Dota2Helper;

public static class ViewLocatorHelpers
{
    public static IServiceCollection AddView<TViewModel, TView>(this IServiceCollection services) where TView : Control, new() where TViewModel : ViewModelBase
    {
        services.AddSingleton(new ViewLocator.ViewLocationDescriptor(typeof(TViewModel), () => new TView()));
        return services;
    }
}