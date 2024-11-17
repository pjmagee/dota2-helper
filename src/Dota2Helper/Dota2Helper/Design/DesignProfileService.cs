using Dota2Helper.Features.Audio;
using Dota2Helper.Features.Timers;

namespace Dota2Helper.Design;

public class DesignProfileService : ProfileService
{
    public DesignProfileService() : base(new DesignSettingsService(), new ViewModelFactory(new FakeTimerAudioService()))
    {

    }
}