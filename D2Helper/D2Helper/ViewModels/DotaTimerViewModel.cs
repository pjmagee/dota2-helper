using System;
using CommunityToolkit.Mvvm.Input;
using D2Helper.Features.Audio;
using D2Helper.Features.Timers;
using Microsoft.Extensions.DependencyInjection;

namespace D2Helper.ViewModels;

/// <summary>
/// Dota2 Timer configuration
/// </summary>
public class DotaTimerViewModel : ViewModelBase
{
    DotaTimer _timer;
    TimerAudioService _audioService;

    bool _isEnabled;
    bool _isResetRequired;
    bool _isAlerting;
    bool _isManualReset;
    bool _isMuted;
    bool _isInterval;
    bool _isExpired;
    bool _isVisible;

    string _name;

    TimeSpan _time;
    TimeSpan _timeRemaining;
    TimeSpan? _showAfter;
    TimeSpan? _timeRemind;
    TimeSpan? _hideAfter;
    string? _audioFile;

    TimeSpan? _manualResetTime;
    public bool IsManualTimerReset => _manualResetTime == default(TimeSpan);
    public bool IsFirstManualTimer => _manualResetTime == null;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public TimeSpan Time
    {
        get => _time;
        set => SetProperty(ref _time, value);
    }

    public bool IsExpired
    {
        get => _isExpired;
        set => SetProperty(ref _isExpired, value);
    }

    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }


    public TimeSpan? RemindAt
    {
        get => _timeRemind;
        set => SetProperty(ref _timeRemind, value == TimeSpan.Zero ? null : value);
    }

    public TimeSpan? HideAfter
    {
        get => _hideAfter;
        set => SetProperty(ref _hideAfter, value == TimeSpan.Zero ? null : value);
    }

    public bool IsAlerting
    {
        get => _isAlerting;
        set => SetProperty(ref _isAlerting, value);
    }

    public bool IsVisible
    {
        get => _isVisible;
        set => SetProperty(ref _isVisible, value);
    }

    public bool IsResetRequired
    {
        get => _isResetRequired;
        set => SetProperty(ref _isResetRequired, value);
    }

    public IRelayCommand ResetCommand { get; }

    DotaTimerViewModel()
    {
        ResetCommand = new RelayCommand(ResetTimer);
    }

    void ResetTimer()
    {
        _manualResetTime = default(TimeSpan);
        IsResetRequired = false;
    }

    public DotaTimerViewModel(DotaTimer timer) : this()
    {
        _timer = timer;

        Name = timer.Name;
        Time = timer.Time;

        IsManualReset = timer.IsManualReset;
        IsInterval = timer.IsInterval;
        IsEnabled = timer.IsEnabled;

        RemindAt = timer.RemindAt;
        HideAfter = timer.HideAfter;
        ShowAfter = timer.ShowAfter;

        AudioFile = timer.AudioFile;

        _manualResetTime = null;
    }

    public TimeSpan TimeRemaining
    {
        get => _timeRemaining;
        set => SetProperty(ref _timeRemaining, value);
    }

    /// <summary>
    /// When the timer becomes active
    /// </summary>
    public TimeSpan? ShowAfter
    {
        get => _showAfter;
        set => SetProperty(ref _showAfter, value == TimeSpan.Zero ? null : value);
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

    public bool IsSoundPlayed { get; set; }

    public void Update(TimeSpan gameTime)
    {
        if (IsEnabled)
        {
            IsResetRequired = CalculateIsResetRequired(gameTime);
            TimeRemaining = CalculateTimeRemaining(gameTime);
            IsAlerting = CalculateIsAlerting(gameTime);
            IsExpired = CalculateIsExpired(gameTime);
            IsVisible = CalculateIsVisible(gameTime);

            if (CalculateIsSoundPlayable())
            {
                App.ServiceProvider.GetRequiredService<TimerAudioService>().Play(AudioFile!);
                IsSoundPlayed = true;
            }

            if (!IsAlerting)
            {
                IsSoundPlayed = false;
            }
        }
        else
        {
            IsVisible = false;
        }
    }

    TimeSpan CalculateTimeRemaining(TimeSpan gameTime)
    {
        return IsInterval ? CalculateIntervalRemaining(gameTime) : CalculateTimeUntilNextOccurrence(gameTime);
    }

    bool CalculateIsSoundPlayable()
    {
        return IsAlerting &&
               !IsMuted &&
               !IsSoundPlayed &&
               IsEnabled &&
               IsVisible &&
               !IsExpired &&
               !IsResetRequired &&
               !string.IsNullOrWhiteSpace(AudioFile);
    }

    bool CalculateIsAlerting(TimeSpan gameTime)
    {
        return TimeRemaining <= RemindAt.GetValueOrDefault(TimeSpan.FromSeconds(1));
    }

    readonly static TimeSpan OneSecond = TimeSpan.FromSeconds(1);

    bool CalculateIsResetRequired(TimeSpan gameTime)
    {
        if (IsResetRequired) return true;

        if (IsManualReset)
        {
            var resetRequired = TimeRemaining <= OneSecond;

            if (resetRequired)
            {
                if (IsFirstManualTimer)
                {
                    _manualResetTime = default(TimeSpan);
                    return false;
                }
                else
                {
                    _manualResetTime = default(TimeSpan);
                    return true;
                }
            }
        }

        return false;
    }

    bool CalculateIsExpired(TimeSpan gameTime)
    {
        if (HideAfter.HasValue)
        {
            return gameTime >= HideAfter;
        }

        return false;
    }

    bool CalculateIsVisible(TimeSpan gameTime)
    {
        if (IsEnabled is not true) return false;

        if(ShowAfter is null && HideAfter is null)
        {
            return true;
        }

        if (ShowAfter.HasValue && HideAfter.HasValue)
        {
            return gameTime > ShowAfter && gameTime < HideAfter;
        }

        if (ShowAfter.HasValue && !HideAfter.HasValue)
        {
            return gameTime > ShowAfter;
        }

        if (HideAfter.HasValue && !ShowAfter.HasValue)
        {
            return gameTime < HideAfter;
        }

        return false;
    }

    TimeSpan CalculateTimeUntilNextOccurrence(TimeSpan gameTime)
    {
        // Skip calculation if the game time is negative
        if (gameTime.TotalSeconds < 0)
        {
            return Time;
        }

        // If the timer needs to be reset by the user, reset `_manualResetTime` to null and return the original timer value
        if (IsResetRequired)
        {
            return Time; // Return the original timer value
        }

        // Initialize `_manualResetTime` to `gameTime` if it's not already set
        if (_manualResetTime == default(TimeSpan))
        {
            _manualResetTime = gameTime;
        }

        // Calculate elapsed time since `_manualResetTime`
        double targetSeconds = Time.TotalSeconds; // The timer interval in seconds (e.g., 20 seconds)
        double elapsedSeconds = gameTime.TotalSeconds - _manualResetTime.GetValueOrDefault().TotalSeconds;

        // Calculate the next occurrence based on the elapsed time
        double nextOccurrence = Math.Floor(elapsedSeconds / targetSeconds) * targetSeconds + targetSeconds;

        // If elapsedSeconds has already reached or passed the next occurrence, adjust forward
        if (elapsedSeconds >= nextOccurrence)
            nextOccurrence += targetSeconds;

        // Calculate the remaining time until the next occurrence
        double remainingSeconds = nextOccurrence - elapsedSeconds;

        return TimeSpan.FromSeconds(remainingSeconds);
    }

    TimeSpan GetObjectiveTime(TimeSpan gameTime)
    {
        if (IsInterval)
        {
            if (gameTime < TimeSpan.Zero)
            {
                return Time + gameTime.Negate();
            }

            if (gameTime > TimeSpan.Zero)
            {
                return Time;
            }
        }

        return gameTime;
    }

    TimeSpan CalculateIntervalRemaining(TimeSpan gameTime)
    {
        try
        {
            var objectiveTime = GetObjectiveTime(gameTime);

            if (IsManualReset)
            {
                if (IsResetRequired) return Time;
                if (IsManualTimerReset || IsFirstManualTimer) _manualResetTime = gameTime;
                return objectiveTime - TimeSpan.FromTicks((gameTime - _manualResetTime.GetValueOrDefault()).Ticks % objectiveTime.Ticks);
            }

            return objectiveTime - TimeSpan.FromTicks(gameTime.Ticks % objectiveTime.Ticks);
        }
        catch (Exception)
        {

        }

        return TimeSpan.Zero;
    }
}