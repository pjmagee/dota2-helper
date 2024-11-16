using D2Helper.Features.Audio;
using D2Helper.Features.Gsi;
using D2Helper.Features.Settings;
using D2Helper.Features.Timers;
using D2Helper.ViewModels;

namespace D2Helper.Design;

public class DesignSettingsViewModel : SettingsViewModel
{
    DesignSettingsViewModel(SettingsService settingsService) : base(new ProfileService(settingsService), new AudioService(), settingsService, new GsiConfigService())
    {
        SelectedProfileViewModel = Profiles[0];
        SelectedTimerViewModel = Timers[0];
        SelectedTimerMode = TimerModes[^1];
        Volume = 50;
    }

    public DesignSettingsViewModel() : this(new DesignSettingsService())
    {

    }
}