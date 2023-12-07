using System;
using LibVLCSharp.Shared;

namespace Dota2Helper.Core;

public abstract class DotaTimer : IDisposable
{
    private readonly LibVLC _libVlc = new();
    private readonly MediaPlayer _player;
    
    public bool IsManualReset { get; protected init; }

    protected DotaTimer(string label, TimeSpan first, TimeSpan interval, TimeSpan reminder, string soundToPlay)
    {
        Label = label;
        First = first;
        Interval = interval;
        Reminder = reminder;
        SoundToPlay = soundToPlay;
        IsActive = true;
        _player = new(_libVlc);
    }

    public double Volume
    {
        set => _player.Volume = (int) value;
        get => _player.Volume;
    }
    
    public TimeSpan Reminder { get; set; }

    public int ReminderInSeconds
    {
        get
        {
            return (int) Reminder.TotalSeconds;
        }
        set
        {
            Reminder = TimeSpan.FromSeconds(value);
        }
    }
    
    private bool IsReminderActive => TimeRemaining - Reminder <= TimeSpan.Zero;
    protected string SoundToPlay { get; init; }
    public string Label { get; }
    public TimeSpan First { get; }
    public TimeSpan Interval { get; }
    public TimeSpan TimeRemaining { get; private set; }
    
    public bool IsActive { get; set; }

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
    /// If this is set to true and TimeRemaining reaches 0, play a sound to indicate that the timer has reached 0.
    /// The sound to play should be defined by the SoundToPlay property.
    /// The sound should play 5 seconds prior to the TimeRemaining reaching 0.
    /// This allows the player time to react to the sound and prepare for the timer to reach 0.
    /// </summary>
    public bool IsSoundEnabled { get; set; }

    public bool IsTriggered => TimeRemaining <= TimeSpan.Zero;

    private TimeSpan _manualResetTime;

    public void Reset()
    {
        IsActive = true;
        _manualResetTime = default;
    }

    public TimeSpan GetObjectiveTime(TimeSpan gameTime)
    {
        return (gameTime > First ? Interval : First + (gameTime < TimeSpan.Zero ? gameTime.Negate() : TimeSpan.Zero));
    }

    public bool IsPendingManualReset => !IsActive && TimeRemaining <= TimeSpan.Zero && IsManualReset;
    
    public void Update(TimeSpan gameTime)
    {
        TimeSpan interval = GetObjectiveTime(gameTime);
        
        if (!IsActive)
        {
            TimeRemaining = TimeSpan.Zero;
            return;
        }
        
        if (!IsManualReset)
        {
            TimeSpan timeInCurrentInterval = TimeSpan.FromTicks(gameTime.Ticks % interval.Ticks);
            TimeRemaining = interval - timeInCurrentInterval;    
        }

        if (IsManualReset)
        {
            if (_manualResetTime == default)
            {
                _manualResetTime = gameTime;
            }
            
            TimeSpan timeInCurrentInterval = TimeSpan.FromTicks((gameTime.Ticks - _manualResetTime.Ticks) % interval.Ticks);
            TimeRemaining = interval - timeInCurrentInterval;
        }
        
        if (IsSoundEnabled && IsReminderActive && !IsSoundPlayed)
        {
            using (var reminderAudio = new Media(_libVlc, SoundToPlay))
            {
                _player.Play(reminderAudio);
            }

            IsSoundPlayed = true;
        }
        
        if (IsTriggered)
            IsSoundPlayed = false;

        if (IsTriggered && IsActive && IsManualReset)
            IsActive = false;
    }

    private bool IsSoundPlayed { get; set; }

    public void Dispose()
    {
        _player.Dispose();
    }
}