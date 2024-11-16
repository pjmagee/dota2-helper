using D2Helper.Features.Audio;
using D2Helper.Features.Gsi;
using D2Helper.Features.TimeProvider;
using D2Helper.Features.Timers;
using D2Helper.ViewModels;

namespace D2Helper.Design;

public class DesignTimersViewModel : TimersViewModel
{

    public DesignTimersViewModel(SettingsViewModel settingsViewModel, GameTimeProvider timeProvider) : base(settingsViewModel, timeProvider)
    {

    }

    private DesignTimersViewModel(DesignSettingsService settingsService) : this(
        new DesignSettingsViewModel(),
        new GameTimeProvider(settingsService, new RealProvider(), new DemoProvider()))
    {

    }

    public DesignTimersViewModel() : this(new DesignSettingsService())
    {

    }
}