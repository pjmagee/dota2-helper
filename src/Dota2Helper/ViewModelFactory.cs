using Dota2Helper.Features.Audio;
using Dota2Helper.Features.Settings;
using Dota2Helper.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Dota2Helper;

public class ViewModelFactory([FromKeyedServices(nameof(TimerAudioService))] IAudioService audioService)
{
    public DotaTimerViewModel Create(DotaTimer timer) => new(timer, audioService);
    public ProfileViewModel Create(Profile profile) => new(profile, this);
}