using D2Helper.Services;

namespace D2Helper.ViewModels;

public class DesignSettingsViewModel : SettingsViewModel
{
    public DesignSettingsViewModel() : base(new TimerService(), new DynamicProvider(new RealGameTimeProvider(), new DemoGameTimeProvider()))
    {
        SelectedTimer = Timers[0];
        SelectedTimerMode = TimerModes[^1];
        TimerVolume = 50;
    }
}