using System;
using CommunityToolkit.Mvvm.Input;
using D2Helper.Features.Timers;

namespace D2Helper.ViewModels;

/// <summary>
/// Dota2 Timer configuration
/// </summary>
public class DotaTimerViewModel : ViewModelBase, IComparable<DotaTimerViewModel>
{
    DotaTimer _timer;

    // The timer is currently enabled for usage
    bool _isEnabled;

    // The timer requires manual reset ( e.g. Roshan timer)
    bool _isResetRequired;

    // The timer is currently alerting the player
    bool _isAlerting;

    // The timer requires manual reset ( e.g. Roshan timer)
    bool _isManualReset;

    // The timer is sound muted
    bool _isMuted;

    // The timer is an interval timer ( e.g. every N seconds)
    bool _isInterval;

    // The timer has gone beyond the disable after time
    bool _isExpired;

    // The order in the list
    int _sortOrder;

    string _name;
    string? _speech;

    TimeSpan _time;
    TimeSpan _timeRemaining;
    TimeSpan? _showAfter;
    TimeSpan? _timeRemind;
    TimeSpan? _hideAfter;
    string? _audioFile;

    // The timer is currently visible
    bool _isVisible;

    public int CompareTo(DotaTimerViewModel? other)
    {
        if (other == null) return 1;
        return SortOrder.CompareTo(other.SortOrder);
    }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public int SortOrder
    {
        get => _sortOrder;
        set => SetProperty(ref _sortOrder, value);
    }

    public string? Speech
    {
        get => _speech;
        set => SetProperty(ref _speech, value);
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

    // public IRelayCommand MoveUpCommand { get; }
    //
    // public IRelayCommand MoveDownCommand { get; }

    private DotaTimerViewModel()
    {
        ResetCommand = new RelayCommand(ResetTimer);
        // MoveUpCommand = new RelayCommand(MoveUp);
        // MoveDownCommand = new RelayCommand(MoveDown);
    }

    void MoveUp()
    {
        SortOrder -= 1;
    }

    void MoveDown()
    {
        SortOrder += 1;
    }

    void ResetTimer()
    {
        IsResetRequired = false;
        TimeRemaining = Time;
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
        Speech = timer.Speech;
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

    public void Update(TimeSpan gameTime)
    {
        if (IsResetRequired || IsExpired)
        {

        }
        else
        {
            TimeRemaining = IsInterval ? CalculateIntervalRemaining(gameTime) : CalculateTimeUntilNextOccurrence(gameTime, Time.Seconds);
        }

        IsResetRequired = CalculateIsResetRequired(gameTime);
        IsExpired = CalculateIsExpired(gameTime);
        IsVisible = CalculateIsVisible(gameTime);
    }

    bool CalculateIsResetRequired(TimeSpan gameTime)
    {
        if (IsManualReset)
        {
            return TimeRemaining.Subtract(TimeSpan.FromSeconds(1)) <= TimeSpan.Zero;
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

        // Calculate the remaining time until the next occurrence
        int remainingSeconds = nextOccurrence - totalSeconds;
        return TimeSpan.FromSeconds(remainingSeconds);
    }

    TimeSpan GetObjectiveTime(TimeSpan gameTime)
    {
        return gameTime > ShowAfter.GetValueOrDefault() ? Time : ShowAfter.GetValueOrDefault() + (gameTime < TimeSpan.Zero ? gameTime.Negate() : TimeSpan.Zero);
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