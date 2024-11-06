using D2Helper.Services;
using D2Helper.ViewModels;

namespace D2Helper.Design;

public class DesignSettingsViewModel : SettingsViewModel
{

    public DesignSettingsViewModel() : this(new DesignSettingsService())
    {

    }

    DesignSettingsViewModel(SettingsService settingsService) : base(new TimerService(settingsService), new DynamicProvider(new RealGameTimeProvider(), new DemoGameTimeProvider()), settingsService)
    {
        SelectedTimerViewModel = Timers[0];
        SelectedTimerMode = TimerModes[^1];
        Volume = 50;
    }
}