using System.Linq;

namespace Dota2Helper.Features.Audio;

public class AudioService(AudioQueue audioQueue) : IAudioService
{
    public void Play(string audioFile)
    {
        if (!string.IsNullOrWhiteSpace(audioFile) && !audioQueue.Contains(audioFile))
        {
            audioQueue.Enqueue(audioFile);
        }
    }
}