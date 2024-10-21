using System;
using ReactiveUI;

namespace Dota2Helper.Core.Timers;

public class DotaTimer : ReactiveObject
{
    private TimeSpan _manualResetTime;

    protected internal DotaTimer(
        string label, 
        TimeSpan first, 
        TimeSpan interval, 
        TimeSpan reminder,
        string audioFile, 
        bool isManualReset,
        string speech,
        bool isTts,
        bool isEnabled)
    {
        Label = label;
        First = first;
        Interval = interval;
        Reminder = reminder;
        AudioFile = audioFile;
        IsManualReset = isManualReset;
        Speech = speech;
        IsTts = isTts;
        IsEnabled = isEnabled;
        IsActive = true;
    }

    public bool IsManualReset { get; protected init; }
    
    public string Speech { get; }
    
    private bool _isTts;
    public bool IsTts 
    {
        get => _isTts;
        set => this.RaiseAndSetIfChanged(ref _isTts, value);
    }
    
    private TimeSpan _reminder;
    public TimeSpan Reminder 
    {
        get => _reminder;
        set => this.RaiseAndSetIfChanged(ref _reminder, value);
    }

    public int ReminderInSeconds
    {
        get => (int)Reminder.TotalSeconds;
        set => Reminder = TimeSpan.FromSeconds(value);
    }

    private bool _isEnabled;
    public bool IsEnabled 
    {
        get => _isEnabled;
        set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
    }
    
    public bool IsReminderActive => TimeRemaining - Reminder <= TimeSpan.Zero;
    
    public string AudioFile { get; }
    public string Label { get; }
    public TimeSpan First { get; }
    public TimeSpan Interval { get; }
    
    
    private TimeSpan _timeRemaining;
    
    public TimeSpan TimeRemaining
    {
        get => _timeRemaining;
        private set
        {
            this.RaisePropertyChanging();
            _timeRemaining = value;
            this.RaisePropertyChanged();
            this.RaisePropertyChanged(nameof(IsReminderActive));
        }
    }

    private bool _isActive;

    public bool IsActive
    {
        get => _isActive;
        private set => this.RaiseAndSetIfChanged(ref _isActive, value);
    }
    

    private bool _isSoundEnabled;
    public bool IsSoundEnabled
    {
        get => _isSoundEnabled;
        set => this.RaiseAndSetIfChanged(ref _isSoundEnabled, value);
    }
    
    public string EnableDisableSoundTooltip => IsSoundEnabled ? "Disable sound" : "Enable sound";
    
    public bool IsTriggered => TimeRemaining <= TimeSpan.Zero;
    
    public bool IsPendingManualReset => !IsActive && IsManualReset;
    
    private bool _isSoundPlayed;

    private bool IsSoundPlayed
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

    private TimeSpan GetObjectiveTime(TimeSpan gameTime)
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
        {
            IsSoundPlayed = false;
        }

        if (IsTriggered && IsActive && IsManualReset)
        {
            IsActive = false;
        }

        if (IsSoundEnabled && IsReminderActive && !IsSoundPlayed)
        {
            OnReminder?.Invoke(this, EventArgs.Empty);
            IsSoundPlayed = true;
        }
    }

    private TimeSpan CalculateTimeRemaining(TimeSpan gameTime)
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