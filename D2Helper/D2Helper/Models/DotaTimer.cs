using System;
using CommunityToolkit.Mvvm.Input;
using D2Helper.ViewModels;

namespace D2Helper.Models;

/// <summary>
/// Dota2 Timer configuration
/// </summary>
public class DotaTimer : ViewModelBase
{
    bool _isEnabled;
    bool _isResetRequired;
    bool _isAlerting;
    bool _isManualReset;
    bool _isMuted;
    bool _isInterval;

    string _name;
    string? _speech;

    TimeSpan _time;
    TimeSpan _timeRemaining;
    TimeSpan? _timeStarts;
    TimeSpan? _timeRemind;
    TimeSpan? _timeDisabled;
    string? _audioFile;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string? Speech
    {
        get => _speech;
        set => SetProperty(ref _speech, value);
    }

    public TimeSpan Every
    {
        get => _time;
        set => SetProperty(ref _time, value);
    }

    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }


    public TimeSpan? RemindAt
    {
        get => _timeRemind;
        set
        {
            if (SetProperty(ref _timeRemind, value)) OnPropertyChanged(nameof(ReminderInSeconds));
        }
    }

    public TimeSpan? DisableAfter
    {
        get => _timeDisabled;
        set
        {
            if (SetProperty(ref _timeDisabled, value)) OnPropertyChanged(nameof(DisableAfterMinutes));
        }
    }

    public bool IsAlerting
    {
        get => _isAlerting;
        set => SetProperty(ref _isAlerting, value);
    }

    public bool IsResetRequired
    {
        get => _isResetRequired;
        set => SetProperty(ref _isResetRequired, value);
    }

    public IRelayCommand ResetCommand { get; }

    public DotaTimer()
    {
        ResetCommand = new RelayCommand(() => { Remaining = Every; });
    }

    public TimeSpan Remaining
    {
        get => _timeRemaining;
        set => SetProperty(ref _timeRemaining, value);
    }

    /// <summary>
    /// When the timer becomes active
    /// </summary>
    public TimeSpan? Starts
    {
        get => _timeStarts;
        set
        {
            if (SetProperty(ref _timeStarts, value)) OnPropertyChanged(nameof(StartsAtAfterMinutes));
        }
    }

    public decimal? ReminderInSeconds
    {
        get => (decimal?)RemindAt?.TotalSeconds;
        set => RemindAt = value is > 0 ? TimeSpan.FromSeconds((double)value.Value) : null;
    }

    public decimal? DisableAfterMinutes
    {
        get => (decimal?)DisableAfter?.TotalMinutes;
        set => DisableAfter = value is > 0 ? TimeSpan.FromMinutes((double)value.Value) : null;
    }

    public decimal? StartsAtAfterMinutes
    {
        get => (decimal?)Starts?.TotalMinutes;
        set => Starts = value is > 0 ? TimeSpan.FromMinutes((double)value.Value) : null;
    }

    /// <summary>
    /// If the timer requires manual resetting by the user
    /// </summary>
    public bool IsManualReset
    {
        get => _isManualReset;
        set => SetProperty(ref _isManualReset, value);
    }

    /// <summary>
    /// If the sound is muted
    /// </summary>
    public bool IsMuted
    {
        get => _isMuted;
        set => SetProperty(ref _isMuted, value);
    }

    /// <summary>
    /// This means the Time is 'every N' seconds, otherwise it's 'at N' seconds
    /// </summary>
    public bool IsInterval
    {
        get => _isInterval;
        set => SetProperty(ref _isInterval, value);
    }

    public string? AudioFile
    {
        get => _audioFile;
        set => SetProperty(ref _audioFile, value);
    }

    public void Update(TimeSpan gameTime)
    {
        if (Starts.HasValue && gameTime < Starts) return;

        if (IsInterval)
        {
            Remaining = CalculateIntervalRemaining(gameTime);
        }
        else
        {
            Remaining = CalculateTimeUntilNextOccurrence(gameTime, Every.Seconds);
        }

        IsResetRequired = Remaining <= TimeSpan.Zero && IsManualReset;
    }

    TimeSpan CalculateTimeUntilNextOccurrence(TimeSpan gameTime, int targetSecond)
    {
        // Calculate the total seconds of the current game time
        int totalSeconds = (int)gameTime.TotalSeconds;

        // Calculate the next occurrence of the target second mark
        int nextOccurrence = ((totalSeconds / 60) * 60) + targetSecond;

        if (totalSeconds >= nextOccurrence)
        {
            nextOccurrence += 60; // Move to the next minute if the current time has passed the target second
        }

        // Calculate the remaining time until the next occurrence
        int remainingSeconds = nextOccurrence - totalSeconds;
        return TimeSpan.FromSeconds(remainingSeconds);
    }

    TimeSpan GetObjectiveTime(TimeSpan gameTime)
    {
        return gameTime > Starts.GetValueOrDefault() ? Every : Starts.GetValueOrDefault() + (gameTime < TimeSpan.Zero ? gameTime.Negate() : TimeSpan.Zero);
    }

    TimeSpan CalculateIntervalRemaining(TimeSpan gameTime)
    {
        try
        {
            var objectiveTime = GetObjectiveTime(gameTime);
            return objectiveTime - TimeSpan.FromTicks(gameTime.Ticks % objectiveTime.Ticks);
        }
        catch (Exception)
        {
            return TimeSpan.Zero.Add(TimeSpan.FromSeconds(1));
        }
    }
}