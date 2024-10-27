using System.Runtime.Versioning;
using System.Speech.Synthesis;

namespace Dota2Helper.Core.Audio;

[SupportedOSPlatform("windows")]
public class TextToSpeechWindows : ITextToSpeech
{
    readonly SpeechSynthesizer _synthesizer = new();

    public void Speak(string text)
    {
        _synthesizer.SpeakAsync(text);
    }

    public int Volume
    {
        set => _synthesizer.Volume = value;
        get => _synthesizer.Volume;
    }

    public void Dispose()
    {
        _synthesizer.Dispose();
    }
}