using System;

namespace Dota2Helper.Core.Audio;

public class TextToSpeechMac : ITextToSpeech
{
    public void Speak(string text)
    {
        throw new PlatformNotSupportedException("macOS is not supported.");
    }

    public int Volume { get; set; }

    public void Dispose()
    {
    }
}