using System.Collections.ObjectModel;
using Dota2Helper.Features.Audio;
using Dota2Helper.Features.Settings;

namespace Dota2Helper.ViewModels;

public class ProfileViewModel : ViewModelBase
{
    string? _name;

    public string? Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public ObservableCollection<DotaTimerViewModel> Timers { get; } = new();

    public ProfileViewModel(Profile profile, ViewModelFactory factory)
    {
        Name = profile.Name;

        foreach (var timer in profile.Timers)
        {
            Timers.Add(factory.Create(timer));
        }
    }
}