using System;

namespace Dota2Helper.Core.Audio;

public interface ITextToSpeech : IDisposable
{
    void Speak(string text);
    int Volume { get; set; }
}