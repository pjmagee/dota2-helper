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
    private static readonly LibVLC LibVlc = new();
    private readonly MediaPlayer _player = new(LibVlc);
    private readonly Queue<AudioQueueItem> _queue = new();
    private readonly SpeechSynthesizer _synthesizer = new();

    public void QueueReminder(DotaTimer timer) => _queue.Enqueue(AudioQueueItem.FromTimer(timer));

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

        if (item.AudioQueueItemType == AudioQueueItemType.Audio)
        {
            using (var reminderAudio = new Media(LibVlc, item.Value))
            {
                _player.Play(reminderAudio);
            }    
        }
        
        if (item.AudioQueueItemType == AudioQueueItemType.Tts)
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