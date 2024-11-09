using D2Helper.Features.TimeProvider;
using D2Helper.Features.Timers;
using D2Helper.ViewModels;

namespace D2Helper.Design;

public class DesignTimersViewModel : TimersViewModel
{

    public DesignTimersViewModel(TimerService timerService, TimeProvider timeProvider) : base(timerService, timeProvider)
    {

    }

    private DesignTimersViewModel(DesignSettingsService settingsService) : this(
        new TimerService(settingsService),
        new TimeProvider(settingsService, new RealProvider(), new DemoProvider()))
    {

    }

    public DesignTimersViewModel() : this(new DesignSettingsService())
    {

    }
}