using System;
using Avalonia.Media;

namespace Dota2Helper.Core;

public class DotaTimer
{
    private TimeSpan _manualResetTime;

    protected internal DotaTimer(string label, TimeSpan first, TimeSpan interval, TimeSpan reminder, string soundToPlay, bool isManualReset = false)
    {
        Label = label;
        First = first;
        Interval = interval;
        Reminder = reminder;
        SoundToPlay = soundToPlay;
        IsActive = true;
        IsManualReset = isManualReset;
    }

    public bool IsManualReset { get; protected init; }

    public TimeSpan Reminder { get; set; }

    public int ReminderInSeconds
    {
        get => (int)Reminder.TotalSeconds;
        set => Reminder = TimeSpan.FromSeconds(value);
    }

    private bool IsReminderActive => TimeRemaining - Reminder <= TimeSpan.Zero;
    public string SoundToPlay { get; }
    public string Label { get; }
    public TimeSpan First { get; }
    public TimeSpan Interval { get; }
    public TimeSpan TimeRemaining { get; private set; }

    public bool IsActive { get; set; }

    public IBrush TimerColor
    {
        get
        {
            if (IsPendingManualReset)
                return Brushes.DarkOrange;

            return IsReminderActive ? Brushes.OrangeRed : Brushes.AntiqueWhite;
        }
    }

    public bool IsSoundEnabled { get; set; }
    public bool IsTriggered => TimeRemaining <= TimeSpan.Zero;

    public bool IsPendingManualReset => !IsActive && TimeRemaining <= TimeSpan.Zero && IsManualReset;

    private bool IsSoundPlayed { get; set; }

    public event EventHandler<EventArgs>? OnReminder;

    public void Reset()
    {
        IsActive = true;
        _manualResetTime = default;
    }

    public TimeSpan GetObjectiveTime(TimeSpan gameTime)
    {
        return gameTime > First ? Interval : First + (gameTime < TimeSpan.Zero ? gameTime.Negate() : TimeSpan.Zero);
    }

    public void Update(TimeSpan gameTime)
    {
        if (!IsActive)
        {
            TimeRemaining = TimeSpan.Zero;
            return;
        }
        
        TimeRemaining = CalculateTimeRemaining(gameTime);
        
        if (TimeRemaining - Reminder > TimeSpan.Zero)
            IsSoundPlayed = false;

        if (IsTriggered && IsActive && IsManualReset)
            IsActive = false;

        if (IsSoundEnabled && IsReminderActive && !IsSoundPlayed)
        {
            OnReminder?.Invoke(this, EventArgs.Empty);
            IsSoundPlayed = true;
        }
    }

    private TimeSpan CalculateTimeRemaining(TimeSpan gameTime)
    {
        var interval = GetObjectiveTime(gameTime);
        
        if (!IsManualReset) return interval - TimeSpan.FromTicks(gameTime.Ticks % interval.Ticks);
        
        if (_manualResetTime == default) 
            _manualResetTime = gameTime;

        return interval - TimeSpan.FromTicks((gameTime.Ticks - _manualResetTime.Ticks) % interval.Ticks);
    }
}