using System;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using Dota2Helper.Features.Audio;
using Dota2Helper.Features.Settings;
using Dota2Helper.Features.Timers;
using Microsoft.Extensions.DependencyInjection;

namespace Dota2Helper.ViewModels;

/// <summary>
/// Dota2 Timer configuration
/// </summary>
public class DotaTimerViewModel : ViewModelBase
{
    DotaTimer _timer;
    readonly IAudioService _audioService;

    string _name;

    bool _isEnabled;
    bool _isResetRequired;
    bool _isAlertable;
    bool _isManualReset;
    bool _isMuted;
    bool _isInterval;
    bool _isStopped;
    bool _isVisible;

    TimeSpan _gameTime;

    TimeSpan _time;
    TimeSpan _timeRemaining;
    TimeSpan? _startsAfter;
    TimeSpan? _timeRemind;
    TimeSpan? _stopsAfter;
    string? _audioFile;

    TimeSpan? ManualResetTime { get; set; }
    bool _isStarted;

    public DotaTimerViewModel(DotaTimer timer, IAudioService audioService)
    {
        _audioService = audioService;
        _timer = timer;
        _name = timer.Name;

        Name = timer.Name;
        Time = timer.Time;

        IsManualReset = timer.IsManualReset;
        IsInterval = timer.IsInterval;
        IsEnabled = timer.IsEnabled;
        RemindAt = timer.RemindAt;
        StopsAfter = timer.StopAfter;
        StartsAfter = timer.StartAfter;
        AudioFile = timer.AudioFile;

        ManualResetTime = null;
        ResetCommand = new RelayCommand(ResetTimer);

        IsStarted = false;
        IsStopped = false;
        IsVisible = false;
        IsResetRequired = false;
    }

    public bool IsManualTimerReset => IsManualReset && ManualResetTime == default(TimeSpan);
    public bool IsFirstManualTimer => IsManualReset && ManualResetTime == null;

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

    public bool IsStopped
    {
        get => _isStopped;
        set => SetProperty(ref _isStopped, value);
    }

    public bool IsStarted
    {
        get => _isStarted;
        set => SetProperty(ref _isStarted, value);
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

    public TimeSpan? StopsAfter
    {
        get => _stopsAfter;
        set => SetProperty(ref _stopsAfter, value == TimeSpan.Zero ? null : value);
    }

    public bool IsAlertable
    {
        get => _isAlertable;
        set => SetProperty(ref _isAlertable, value);
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

    void ResetTimer()
    {
        ManualResetTime = default(TimeSpan);
        IsResetRequired = false;
    }

    public TimeSpan TimeRemaining
    {
        get => _timeRemaining;
        set => SetProperty(ref _timeRemaining, value);
    }

    public TimeSpan? StartsAfter
    {
        get => _startsAfter;
        set => SetProperty(ref _startsAfter, value == TimeSpan.Zero ? null : value);
    }

    public bool IsManualReset
    {
        get => _isManualReset;
        set => SetProperty(ref _isManualReset, value);
    }

    public bool IsMuted
    {
        get => _isMuted;
        set => SetProperty(ref _isMuted, value);
    }

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
        // GameTime = gameTime;

        if (IsEnabled)
        {
            IsStarted = CalculateIsStarted(gameTime);
            IsStopped = CalculateIsStopped(gameTime);

            IsVisible = CalculateIsVisible(gameTime);

            TimeRemaining = CalculateTimeRemaining(gameTime);

            IsResetRequired = CalculateIsResetRequired(gameTime);
            IsAlertable = CalculateIsAlertable(gameTime);

            if (CalculateIsSoundPlayable())
            {
                App.ServiceProvider.GetRequiredService<TimerAudioService>().Play(AudioFile!);
                IsSoundPlayed = true;
            }

            if (!IsAlertable)
            {
                IsSoundPlayed = false;
            }
        }
        else
        {
            IsVisible = false;
        }


    }

    public TimeSpan GameTime
    {
        get => _gameTime;
        set => SetProperty(ref _gameTime, value);
    }

    TimeSpan CalculateTimeRemaining(TimeSpan gameTime)
    {
        return IsInterval ? CalculateIntervalRemaining(gameTime) : CalculateTimeUntilNextOccurrence(gameTime);
    }

    bool CalculateIsSoundPlayable()
    {
        return IsAlertable &&
               !IsMuted &&
               !IsSoundPlayed &&
               IsEnabled &&
               IsVisible &&
               IsStarted &&
               !IsStopped &&
               !IsResetRequired &&
               !string.IsNullOrWhiteSpace(AudioFile);
    }

    bool CalculateIsAlertable(TimeSpan gameTime)
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
                    ManualResetTime = gameTime;
                    return false;
                }

                ManualResetTime = default(TimeSpan);
                return true;
            }
        }

        return false;
    }

    bool CalculateIsStopped(TimeSpan gameTime)
    {
        if (StopsAfter.HasValue)
        {
            return StopsAfter <= gameTime;
        }

        return false;
    }

    bool CalculateIsStarted(TimeSpan gameTime)
    {
        if (StartsAfter.HasValue)
        {
            return StartsAfter <= gameTime;
        }

        return true;
    }

    bool CalculateIsVisible(TimeSpan gameTime)
    {
        return IsEnabled && IsStarted && !IsStopped;
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
        if (ManualResetTime == default(TimeSpan))
        {
            ManualResetTime = gameTime;
        }

        // Calculate elapsed time since `_manualResetTime`
        double targetSeconds = Time.TotalSeconds; // The timer interval in seconds (e.g., 20 seconds)
        double elapsedSeconds = gameTime.TotalSeconds - ManualResetTime.GetValueOrDefault().TotalSeconds;

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
                if (IsManualTimerReset) ManualResetTime = gameTime;
                return objectiveTime - TimeSpan.FromTicks((gameTime - ManualResetTime.GetValueOrDefault()).Ticks % objectiveTime.Ticks);
            }

            return objectiveTime - TimeSpan.FromTicks(gameTime.Ticks % objectiveTime.Ticks);
        }
        catch (Exception)
        {

        }

        return TimeSpan.Zero;
    }
}