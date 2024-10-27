using Dota2Helper.Core.Timers;

namespace Dota2Helper.Core.Audio;

public class AudioQueueItem
{
    public required string Value { get; init; }

    public required string Label { get; init; }
    
    public AudioQueueItemType AudioQueueItemType { get; init; }
    

    public static AudioQueueItem FromTimer(DotaTimer timer) => new()
    {
        Value = timer.IsTts ? timer.Speech : timer.AudioFile, 
        AudioQueueItemType = timer.IsTts ? AudioQueueItemType.TextToSpeech : AudioQueueItemType.Effect,
        Label = timer.Label
    };
}