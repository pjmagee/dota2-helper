using System;
using System.Collections.Generic;
using LibVLCSharp.Shared;

namespace Dota2Helper.Core;

public class AudioPlayer : IDisposable
{
    private static readonly LibVLC LibVlc = new();
    private readonly MediaPlayer _player = new(LibVlc);
    private readonly Queue<string> _queue = new();

    public void QueueReminder(string path)
    {
        _queue.Enqueue(path);
    }
    
    public int Volume 
    {
        set => _player.Volume = value;
        get => _player.Volume;
    }
    
    public bool HasAudioToPlay => _queue.Count > 0;
    
    public void PlayReminder()
    {
        if (!_queue.TryDequeue(out var audioPath)) return;
        using var reminderAudio = new Media(LibVlc, audioPath);
        _player.Play(reminderAudio);
    }

    public void Dispose()
    {
        _player.Dispose();
    }
}