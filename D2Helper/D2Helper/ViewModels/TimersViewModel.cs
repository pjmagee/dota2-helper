using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using D2Helper.Models;
using D2Helper.Services;
using D2Helper.Views;
using Microsoft.Extensions.DependencyInjection;

namespace D2Helper.ViewModels;

public class TimersViewModel : ViewModelBase, IDisposable, IAsyncDisposable
{
    readonly TimerService _timerService;
    readonly IStrategyProvider _strategyProvider;
    readonly IGameTimeProvider _gameTimeProvider;
    readonly ITimer? _timer = null;
    TimeSpan _time;


    public TimersViewModel(
        TimerService timerService,
        IStrategyProvider strategyProvider,
        IGameTimeProvider gameTimeProvider)
    {
        _timerService = timerService;
        _strategyProvider = strategyProvider;
        _gameTimeProvider = gameTimeProvider;

        OpenSettingsCommand = new RelayCommand(() =>
        {
            var settings = new SettingsWindow()
            {
                DataContext = App.ServiceProvider.GetRequiredService<SettingsViewModel>(),
            };

            settings.Show();
        });

        _timer = TimeProvider.System.CreateTimer(
            callback: UpdateTimers,
            state: null, dueTime:
            TimeSpan.Zero,
            period:TimeSpan.FromSeconds(1));
    }

    void UpdateTimers(object? state)
    {
        foreach (var timer in Timers)
        {
            if (timer.IsEnabled)
            {
                timer.Update(_gameTimeProvider.Time);
            }
        }

        Time = _gameTimeProvider.Time;
    }

    public ObservableCollection<DotaTimerViewModel> Timers => _timerService.Timers;
    public IRelayCommand OpenSettingsCommand { get; }

    public TimeSpan Time
    {
        get => _time;
        set => SetProperty(ref _time, value);
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_timer is not null)
            await _timer.DisposeAsync();
    }
}