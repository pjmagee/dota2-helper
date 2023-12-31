﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Speech.Synthesis;
using System.Threading.Tasks;
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

    public void QueueReminder(DotaTimer timer)
    {
        _queue.Enqueue(timer.IsTts
            ? new AudioQueueItem { Value = timer.Speech, IsTts = false }
            : new AudioQueueItem { Value = timer.SoundToPlay, IsTts = true });
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

        if (item.IsTts)
        {
            using (var reminderAudio = new Media(LibVlc, item.Value))
            {
                _player.Play(reminderAudio);
            }    
        }
        else
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