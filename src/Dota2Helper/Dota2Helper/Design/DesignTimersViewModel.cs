using Dota2Helper.Features.TimeProvider;
using Dota2Helper.ViewModels;

namespace Dota2Helper.Design;

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