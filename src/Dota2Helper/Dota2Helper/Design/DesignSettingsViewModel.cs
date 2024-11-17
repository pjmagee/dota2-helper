using System;
using Dota2Helper.Features.Audio;
using Dota2Helper.Features.Gsi;
using Dota2Helper.Features.Settings;
using Dota2Helper.ViewModels;

namespace Dota2Helper.Design;

public class DesignSettingsViewModel : SettingsViewModel
{
    DesignSettingsViewModel(SettingsService settingsService) : base(new DesignProfileService(), new AudioService(settingsService), new ViewModelFactory(new FakeTimerAudioService()), settingsService, new GsiConfigService())
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