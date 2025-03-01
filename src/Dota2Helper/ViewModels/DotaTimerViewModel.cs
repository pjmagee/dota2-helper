using System;
using CommunityToolkit.Mvvm.Input;
using Dota2Helper.Features.Audio;
using Dota2Helper.Features.Settings;

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
        Interval = timer.Interval;
        IsManualReset = timer.IsManualReset;
        IsInterval = timer.IsInterval;
        IsEnabled = timer.IsEnabled;
        RemindAt = timer.RemindBefore;
        HideAfter = timer.Visibility.HideAfter;
        ShowAfter = timer.Visibility.ShowAfter;
        AudioFile = timer.AudioFile;
        Offset = timer.Offset;
        ManualResetTime = null;
        ResetCommand = new RelayCommand(ResetTimer);
        IsStarted = false;
        IsStopped = false;
        IsVisible = false;
        IsResetRequired = false;
    }

    public bool IsManualTimerReset => IsManualReset && ManualResetTime == default(TimeSpan);
    public bool IsFirstManualTimer => IsManualReset && ManualResetTime == null;

    string _name = null!;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    TimeSpan _interval;
    public TimeSpan Interval
    {
        get => _interval;
        set => SetProperty(ref _interval, value);
    }

    bool _isStopped;
    public bool IsStopped
    {
        get => _isStopped;
        set => SetProperty(ref _isStopped, value);
    }

    bool _isStarted;
    public bool IsStarted
    {
        get => _isStarted;
        set => SetProperty(ref _isStarted, value);
    }

    bool _isEnabled;
    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }


    TimeSpan? _remindAt;
    public TimeSpan? RemindAt
    {
        get => _remindAt;
        set => SetProperty(ref _remindAt, value == TimeSpan.Zero ? null : value);
    }

    TimeSpan? _hideAfter;
    public TimeSpan? HideAfter
    {
        get => _hideAfter;
        set => SetProperty(ref _hideAfter, value == TimeSpan.Zero ? null : value);
    }

    bool _isAlertable;
    public bool IsAlertable
    {
        get => _isAlertable;
        set => SetProperty(ref _isAlertable, value);
    }

    bool _isVisible;
    public bool IsVisible
    {
        get => _isVisible;
        set => SetProperty(ref _isVisible, value);
    }

    bool _isResetRequired;
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

    TimeSpan _timeRemaining;
    public TimeSpan TimeRemaining
    {
        get => _timeRemaining;
        set => SetProperty(ref _timeRemaining, value);
    }

    TimeSpan? _offset;
    public TimeSpan? Offset
    {
        get => _offset;
        set => SetProperty(ref _offset, value);
    }

    TimeSpan? _showAfter;
    public TimeSpan? ShowAfter
    {
        get => _showAfter;
        set => SetProperty(ref _showAfter, value == TimeSpan.Zero ? null : value);
    }

    bool _isManualReset;
    public bool IsManualReset
    {
        get => _isManualReset;
        set => SetProperty(ref _isManualReset, value);
    }

    bool _isMuted;
    public bool IsMuted
    {
        get => _isMuted;
        set => SetProperty(ref _isMuted, value);
    }

    bool _isInterval;
    public bool IsInterval
    {
        get => _isInterval;
        set => SetProperty(ref _isInterval, value);
    }

    string? _audioFile;
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
        if (HideAfter.HasValue)
        {
            return HideAfter <= gameTime;
        }

        return false;
    }

    bool CalculateIsStarted(TimeSpan gameTime)
    {
        if (ShowAfter.HasValue)
        {
            return ShowAfter <= gameTime;
        }

        return true;
    }

    bool CalculateIsVisible(TimeSpan gameTime)
    {
        return IsEnabled && IsStarted && !IsStopped;
    }

    TimeSpan CalculateTimeUntilNextOccurrence(TimeSpan gameTime)
    {
        if (gameTime.TotalSeconds < 0)
        {
            return Interval;
        }
    
        if (IsResetRequired)
        {
            return Interval;
        }
    
        if (ManualResetTime == default(TimeSpan))
        {
            ManualResetTime = gameTime;
        }
    
        double targetSeconds = Interval.TotalSeconds;
        double elapsedSeconds = gameTime.TotalSeconds - ManualResetTime.GetValueOrDefault().TotalSeconds - Offset.GetValueOrDefault().TotalSeconds;
    
        double nextOccurrence = Math.Floor(elapsedSeconds / targetSeconds) * targetSeconds + targetSeconds;
    
        if (elapsedSeconds >= nextOccurrence)
            nextOccurrence += targetSeconds;
    
        double remainingSeconds = nextOccurrence - elapsedSeconds;
    
        return TimeSpan.FromSeconds(remainingSeconds);
    }

    TimeSpan GetObjectiveTime(TimeSpan gameTime)
    {
        if (IsInterval)
        {
            if (gameTime < TimeSpan.Zero)
            {
                return Interval + gameTime.Negate();
            }

            if (gameTime > TimeSpan.Zero)
            {
                return Interval;
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
                if (IsResetRequired) return Interval;
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