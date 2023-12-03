using System;
using System.IO;
using LibVLCSharp.Shared;

namespace Dota2Helper.Core;

public class DotaTimer : IDisposable
{
    private static readonly LibVLC LibVlc = new();
    private readonly MediaPlayer _player = new(LibVlc);
    
    public bool IsManualReset { get; protected set; }

    protected DotaTimer(string label, TimeSpan fromGameStart, TimeSpan interval, TimeSpan reminderTime)
    {
        Label = label;
        FromGameStart = fromGameStart;
        Interval = interval;
        ReminderTime = reminderTime;
    }

    public double Volume
    {
        set => _player.Volume = (int) value;
        get => _player.Volume;
    }
    
    public TimeSpan ReminderTime { get; }
    public bool IsReminderActive => TimeRemaining - ReminderTime <= TimeSpan.Zero;

    protected string SoundToPlay { get; init; }

    public string Label { get; }
    public TimeSpan FromGameStart { get; }
    public TimeSpan Interval { get; }
    public TimeSpan TimeRemaining { get; private set; }

    public Avalonia.Media.IBrush TimerColor
    {
        get
        {
            if (IsPendingManualReset)
                return Avalonia.Media.Brushes.DarkOrange;
            
            return IsReminderActive ? Avalonia.Media.Brushes.OrangeRed : Avalonia.Media.Brushes.AntiqueWhite;
        }
    }

    /// <summary>
    /// The last time this timer was updated.
    /// </summary>
    private TimeSpan LastUpdateTime { get; set; }
    
    /// <summary>
    /// If this is set to true and TimeRemaining reaches 0, play a sound to indicate that the timer has reached 0.
    /// The sound to play should be defined by the SoundToPlay property.
    /// The sound should play 5 seconds prior to the TimeRemaining reaching 0.
    /// This allows the player time to react to the sound and prepare for the timer to reach 0.
    /// </summary>
    public bool IsSoundEnabled { get; set; }

    public bool IsTriggered => TimeRemaining <= TimeSpan.Zero && LastUpdateTime != default;


    public void Reset()
    {
        LastUpdateTime = default;
    }

    public bool IsPendingManualReset => LastUpdateTime != default && TimeRemaining <= TimeSpan.Zero && IsManualReset;
    
    public void Update(TimeSpan gameTime)
    {
        if (IsTriggered)
        {
            TimeRemaining = TimeSpan.Zero;
            if (IsManualReset) return;
            LastUpdateTime = default;
        }

        if (TimeRemaining <= TimeSpan.Zero && LastUpdateTime == default)
        {
            if (FromGameStart > gameTime)
            {
                TimeRemaining = FromGameStart;
            }
            else if (FromGameStart <= gameTime)
            {
                TimeRemaining = Interval;
            }
            
            IsSoundPlayed = false;

            if (LastUpdateTime == default)
            {
                LastUpdateTime = gameTime;
            }
        }
        else
        {
            TimeRemaining -= gameTime - LastUpdateTime;
            LastUpdateTime = gameTime;
        }
        
        if (IsSoundEnabled && IsReminderActive && !IsSoundPlayed)
        {
            using (var reminderAudio = new Media(LibVlc, SoundToPlay, FromType.FromPath))
            {
                _player.Play(reminderAudio);
            }

            IsSoundPlayed = true;
        }
    }

    public bool IsSoundPlayed { get; set; }

    public void Dispose()
    {
        _player.Dispose();
    }
}