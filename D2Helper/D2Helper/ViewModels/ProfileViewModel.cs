using System.Collections.ObjectModel;
using D2Helper.Features.Settings;

namespace D2Helper.ViewModels;

public class ProfileViewModel : ViewModelBase
{
    string _name;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public ObservableCollection<DotaTimerViewModel> Timers { get; } = new();

    public ProfileViewModel(Profile profile)
    {
        Name = profile.Name;

        foreach (var timer in profile.Timers)
        {
            Timers.Add(new DotaTimerViewModel(timer));
        }
    }
}