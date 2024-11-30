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
    readonly IAudioService _audioService;

    TimeSpan? ManualResetTime { get; set; }

    public DotaTimerViewModel(DotaTimer timer, IAudioService audioService)
    {
        _audioService = audioService;
        Name = timer.Name;

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
        get;
        set => SetProperty(ref field, value);
    }

    public TimeSpan Time
    {
        get;
        set => SetProperty(ref field, value);
    }

    public bool IsStopped
    {
        get;
        set => SetProperty(ref field, value);
    }

    public bool IsStarted
    {
        get;
        set => SetProperty(ref field, value);
    }

    public bool IsEnabled
    {
        get;
        set => SetProperty(ref field, value);
    }


    public TimeSpan? RemindAt
    {
        get;
        set => SetProperty(ref field, value == TimeSpan.Zero ? null : value);
    }

    public TimeSpan? StopsAfter
    {
        get;
        set => SetProperty(ref field, value == TimeSpan.Zero ? null : value);
    }

    public bool IsAlertable
    {
        get;
        set => SetProperty(ref field, value);
    }

    public bool IsVisible
    {
        get;
        set => SetProperty(ref field, value);
    }

    public bool IsResetRequired
    {
        get;
        set => SetProperty(ref field, value);
    }

    public IRelayCommand ResetCommand { get; }

    void ResetTimer()
    {
        ManualResetTime = default(TimeSpan);
        IsResetRequired = false;
    }

    public TimeSpan TimeRemaining
    {
        get;
        set => SetProperty(ref field, value);
    }

    public TimeSpan? StartsAfter
    {
        get;
        set => SetProperty(ref field, value == TimeSpan.Zero ? null : value);
    }

    public bool IsManualReset
    {
        get;
        set => SetProperty(ref field, value);
    }

    public bool IsMuted
    {
        get;
        set => SetProperty(ref field, value);
    }

    public bool IsInterval
    {
        get;
        set => SetProperty(ref field, value);
    }

    public string? AudioFile
    {
        get;
        set => SetProperty(ref field, value);
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
                _audioService.Play(AudioFile!);
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
        get;
        set => SetProperty(ref field, value);
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