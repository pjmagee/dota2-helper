using System;
using CommunityToolkit.Mvvm.Input;
using D2Helper.Features.Audio;
using D2Helper.Features.Settings;
using D2Helper.Features.TimeProvider;
using D2Helper.Features.Timers;
using Microsoft.Extensions.DependencyInjection;

namespace D2Helper.ViewModels;

/// <summary>
/// Dota2 Timer configuration
/// </summary>
public class DotaTimerViewModel : ViewModelBase
{
    DotaTimer _timer;

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
        IsResetRequired = false;
        TimeRemaining = Time;
        _manualResetTime = null;
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
            TimeRemaining = CalculateTimeRemaining(gameTime);
            IsResetRequired = CalculateIsResetRequired(gameTime);
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
        if (IsInterval)
            return CalculateIntervalRemaining(gameTime);

        return CalculateTimeUntilNextOccurrence(gameTime, Time.Seconds);
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

    bool CalculateIsAlerting(TimeSpan gameTime) => TimeRemaining <= RemindAt.GetValueOrDefault(TimeSpan.FromSeconds(1));

    readonly static TimeSpan OneSecond = TimeSpan.FromSeconds(1);

    bool CalculateIsResetRequired(TimeSpan gameTime)
    {
        if (IsResetRequired) return true;

        if (IsManualReset && TimeRemaining <= OneSecond)
        {
            _manualResetTime = null;
            return true;
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

        if (totalSeconds < 0)
        {
            nextOccurrence = ((totalSeconds / 60) * 60) - 60 + targetSecond;
        }

        // Calculate the remaining time until the next occurrence
        int remainingSeconds = nextOccurrence - totalSeconds;
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
                if (_manualResetTime is null)
                {
                    return TimeSpan.Zero;
                }

                return objectiveTime - TimeSpan.FromTicks((gameTime - _manualResetTime.GetValueOrDefault()).Ticks % objectiveTime.Ticks);
            }
            else
            {
                return objectiveTime - TimeSpan.FromTicks(gameTime.Ticks % objectiveTime.Ticks);
            }
        }
        catch (Exception)
        {

        }

        return TimeSpan.Zero;
    }
}