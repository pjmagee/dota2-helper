using System;
using ReactiveUI;

namespace Dota2Helper.Core.Timers;

public class DotaTimer : ReactiveObject
{
    TimeSpan? _expireAt;
    TimeSpan _manualResetTime;
    TimeSpan? _offset;
    TimeSpan _timeRemaining;
    TimeSpan _reminder;

    bool _isEnabled;
    bool _isActive;
    bool _isReminderActive;
    bool _isSoundEnabled;
    bool _isSoundPlayed;
    bool _isTts;
    bool _isVisible;
    bool _isExpired;

    protected internal DotaTimer(
        string label,
        TimeSpan first,
        TimeSpan interval,
        TimeSpan reminder,
        TimeSpan? offset,
        TimeSpan? expireAt,
        string audioFile,
        bool isManualReset,
        string speech,
        bool isTts,
        bool isSoundEnabled,
        bool isEnabled)
    {

        Label = label;
        First = first;
        Interval = interval;
        Reminder = reminder;
        AudioFile = audioFile;
        IsManualReset = isManualReset;
        Speech = speech;
        Offset = offset;
        ExpireAt = expireAt;
        IsEnabled = isEnabled;
        IsSoundEnabled = isSoundEnabled;
        IsTts = isTts;
        IsActive = true;
        IsExpired = false;
        IsVisible = true;
    }

    public bool IsExpired
    {
        get => _isExpired;
        set => this.RaiseAndSetIfChanged(ref _isExpired, value);
    }

    public TimeSpan? ExpireAt
    {
        get => _expireAt;
        set =>  this.RaiseAndSetIfChanged(ref _expireAt, value);
    }

    public bool IsManualReset { get; protected init; }

    public string Speech { get; }

    public bool IsTts
    {
        get => _isTts;
        set => this.RaiseAndSetIfChanged(ref _isTts, value);
    }

    public TimeSpan Reminder
    {
        get => _reminder;
        private set => this.RaiseAndSetIfChanged(ref _reminder, value);
    }

    public int ReminderInSeconds
    {
        get => (int)Reminder.TotalSeconds;
        set => Reminder = TimeSpan.FromSeconds(value);
    }

    public int OffsetInSeconds
    {
        get => (int) (Offset?.TotalSeconds ?? TimeSpan.Zero.TotalSeconds);
        set => Offset = TimeSpan.FromSeconds(value);
    }

    public int ExpireInMinutes
    {
        get => (int) (ExpireAt?.Minutes ?? TimeSpan.Zero.Minutes);
        set => ExpireAt = TimeSpan.FromMinutes(value);
    }

    public bool IsEnabled
    {
        get => _isEnabled;
        set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
    }


    public bool IsReminderActive
    {
        get => _isReminderActive;
        set => this.RaiseAndSetIfChanged(ref _isReminderActive, value);
    }

    public string AudioFile { get; }
    public string Label { get; }
    public TimeSpan First { get; }
    public TimeSpan Interval { get; }

    public TimeSpan? Offset
    {
        get => _offset;
        private set => this.RaiseAndSetIfChanged(ref _offset, value);
    }

    public TimeSpan TimeRemaining
    {
        get => _timeRemaining;
        private set => this.RaiseAndSetIfChanged(ref _timeRemaining, value);
    }

    public bool IsActive
    {
        get => _isActive;
        private set => this.RaiseAndSetIfChanged(ref _isActive, value);
    }

    public bool IsSoundEnabled
    {
        get => _isSoundEnabled;
        set => this.RaiseAndSetIfChanged(ref _isSoundEnabled, value);
    }

    public string EnableDisableSoundTooltip => IsSoundEnabled ? "Disable sound" : "Enable sound";

    bool IsTriggered => TimeRemaining <= TimeSpan.Zero;

    public bool IsPendingManualReset => !IsActive && IsManualReset;

    bool IsSoundPlayed
    {
        get => _isSoundPlayed;
        set => this.RaiseAndSetIfChanged(ref _isSoundPlayed, value);
    }

    public string EnableDisableTimerTooltip => IsEnabled ? "Disable timer" : "Enable timer";

    public string EnableDisableTtsTooltip => "TTS or Effect";

    public event EventHandler<EventArgs>? OnReminder;

    public void Reset()
    {
        IsActive = true;
        _manualResetTime = default;
    }

    TimeSpan GetObjectiveTime(TimeSpan gameTime)
    {
        var time =  gameTime > First ? Interval : First + (gameTime < TimeSpan.Zero ? gameTime.Negate() : TimeSpan.Zero);

        if (Offset.HasValue)
        {
            time = time.Add(Offset.Value);
        }

        return time;
    }

    public void Update(TimeSpan gameTime)
    {
        IsExpired = gameTime >= ExpireAt;
        IsVisible = !IsExpired && IsEnabled;

        if (IsExpired || !IsVisible || !IsEnabled)
        {
            TimeRemaining = TimeSpan.Zero;
            return;
        }

        if (!IsActive)
        {
            TimeRemaining = TimeSpan.Zero;
            return;
        }

        TimeRemaining = CalculateTimeRemaining(gameTime);

        if (TimeRemaining - Reminder > TimeSpan.Zero)
        {
            IsSoundPlayed = false;
        }

        if (IsTriggered && IsActive && IsManualReset)
        {
            IsActive = false;
        }

        if (IsSoundEnabled && CheckIsReminderActive() && !IsSoundPlayed)
        {
            OnReminder?.Invoke(this, EventArgs.Empty);
            IsSoundPlayed = true;
        }
    }

    public bool IsVisible
    {
        get => _isVisible;
        set => this.RaiseAndSetIfChanged(ref _isVisible, value);
    }

    bool CheckIsReminderActive()
    {
        return IsReminderActive = TimeRemaining - Reminder <= TimeSpan.Zero;
    }

    TimeSpan CalculateTimeRemaining(TimeSpan gameTime)
    {
        var interval = GetObjectiveTime(gameTime);

        if (!IsManualReset)
        {
            return interval - TimeSpan.FromTicks(gameTime.Ticks % interval.Ticks);
        }

        if (_manualResetTime == default)
        {
            _manualResetTime = gameTime;
        }

        return interval - TimeSpan.FromTicks((gameTime.Ticks - _manualResetTime.Ticks) % interval.Ticks);
    }
}