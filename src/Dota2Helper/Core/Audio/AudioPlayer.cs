using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Speech.Synthesis;
using Dota2Helper.Core.Timers;
using LibVLCSharp.Shared;

namespace Dota2Helper.Core.Audio;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class AudioPlayer : IDisposable
{
    readonly static LibVLC LibVlc = new();
    readonly MediaPlayer _player = new(LibVlc);
    readonly Queue<AudioQueueItem> _queue = new();
    readonly SpeechSynthesizer _synthesizer = new();

    public void QueueReminder(DotaTimer timer)
    {
        // Prevent duplicate reminders
        foreach (var item in _queue)
        {
            if (item.Label == timer.Label) return;
        }

        _queue.Enqueue(AudioQueueItem.FromTimer(timer));
    }

    public int Volume 
    {
        set
        {
            _player.Volume = value;
            _synthesizer.Volume = value;   
        }
        get => _player.Volume;
    }
    
    public bool HasReminderQueued => _queue.Count > 0;
    
    public void PlayReminder()
    {
        if (!_queue.TryDequeue(out var item)) return;

        if (item.AudioQueueItemType == AudioQueueItemType.Effect)
        {
            using (var reminderAudio = new Media(LibVlc, item.Value))
            {
                _player.Play(reminderAudio);
            }    
        }
        
        if (item.AudioQueueItemType == AudioQueueItemType.TextToSpeech)
        {
            _synthesizer.SpeakAsync(item.Value);
        }
    }

    public void Dispose()
    {
        _synthesizer.Dispose();
        _player.Dispose();
    }
}