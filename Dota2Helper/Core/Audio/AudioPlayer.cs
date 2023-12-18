using System;
using System.Collections.Generic;
using System.Speech.Synthesis;
using Dota2Helper.Core.Timers;
using LibVLCSharp.Shared;

namespace Dota2Helper.Core.Audio;

public class AudioQueueItem
{
    public string Value { get; set; }
    public bool IsFile { get; set; }
}

public class AudioPlayer : IDisposable
{
    private static readonly LibVLC LibVlc = new();
    private readonly MediaPlayer _player = new(LibVlc);
    private readonly Queue<AudioQueueItem> _queue = new();
    private readonly SpeechSynthesizer _synthesizer = new();

    public void QueueReminder(DotaTimer timer)
    {
        _queue.Enqueue(timer.IsTts
            ? new AudioQueueItem { Value = timer.Speech, IsFile = false }
            : new AudioQueueItem { Value = timer.SoundToPlay, IsFile = true });
    }
    
    public int Volume 
    {
        set
        {
            _player.Volume = value;

            if (OperatingSystem.IsWindows())
            {
                _synthesizer.Volume = value;    
            }
        }
        get => _player.Volume;
    }
    
    public bool HasReminderQueued => _queue.Count > 0;
    
    public void PlayReminder()
    {
        if (!_queue.TryDequeue(out var item)) return;

        if (item.IsFile)
        {
            using var reminderAudio = new Media(LibVlc, item.Value);
            _player.Play(reminderAudio);    
        }
        else
        {
            if (OperatingSystem.IsWindows())
            {
                _synthesizer.Volume = _player.Volume;
                _synthesizer.Speak(item.Value);
            }
        }
    }

    public void Dispose()
    {
        _player.Dispose();
    }
}