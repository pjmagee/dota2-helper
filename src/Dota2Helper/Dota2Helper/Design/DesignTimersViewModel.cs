using Dota2Helper.Features.TimeProvider;
using Dota2Helper.ViewModels;

namespace Dota2Helper.Design;

public class DesignTimersViewModel : TimersViewModel
{
    DesignTimersViewModel(GameTimeProvider timeProvider) : base(new DesignSettingsViewModel(), timeProvider)
    {

    }

    DesignTimersViewModel(DesignSettingsService settingsService) : this(new GameTimeProvider(settingsService, new RealProvider(), new DemoProvider()))
    {

    }

    public DesignTimersViewModel() : this(new DesignSettingsService())
    {

    }
}