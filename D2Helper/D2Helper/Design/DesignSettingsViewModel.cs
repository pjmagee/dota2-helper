using D2Helper.Features.Audio;
using D2Helper.Features.Settings;
using D2Helper.Features.TimeProvider;
using D2Helper.Features.Timers;
using D2Helper.ViewModels;

namespace D2Helper.Design;

public class DesignSettingsViewModel : SettingsViewModel
{
    public DesignSettingsViewModel() : this(new DesignSettingsService())
    {

    }

    DesignSettingsViewModel(SettingsService settingsService) : base(
        new TimerService(settingsService),
        new AudioService(),
        new StrategyTimeProvider(new GameTimeProvider(), new DemoTimeProvider()), settingsService)
    {
        SelectedTimerViewModel = Timers[0];
        SelectedTimerMode = TimerModes[^1];
        Volume = 50;
    }
}