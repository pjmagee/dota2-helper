namespace Dota2Helper.Core.Audio;

public class StubTextToSpeech : ITextToSpeech
{
    public void Speak(string text)
    {
    }

    public int Volume { get; set; }

    public void Dispose()
    {
    }
}