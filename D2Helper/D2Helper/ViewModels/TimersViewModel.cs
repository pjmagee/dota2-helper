using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using D2Helper.Features.TimeProvider;
using D2Helper.Features.Timers;
using D2Helper.Views;
using Microsoft.Extensions.DependencyInjection;
using TimeProvider = System.TimeProvider;

namespace D2Helper.ViewModels;

public class TimersViewModel : ViewModelBase, IDisposable, IAsyncDisposable
{
    readonly TimerService _timerService;
    readonly ITimeProvider _timeProvider;
    readonly ITimer? _timer = null;
    TimeSpan _time;

    readonly SemaphoreSlim _semaphore = new(1, 1);

    public TimersViewModel(
        TimerService timerService,
        ITimeProvider timeProvider)
    {
        _timerService = timerService;
        _timeProvider = timeProvider;

        OpenSettingsCommand = new RelayCommand(() =>
        {
            var settings = new SettingsWindow
            {
                DataContext = App.ServiceProvider.GetRequiredService<SettingsViewModel>(),
            };

            settings.Show();
        });

        _timer = TimeProvider.System.CreateTimer(
            callback: UpdateTimers,
            state: null,
            dueTime: TimeSpan.Zero,
            period: TimeSpan.FromSeconds(1));
    }

    async void UpdateTimers(object? state)
    {
        if (await _semaphore.WaitAsync(0))
        {
            try
            {
                Time = _timeProvider.Time;

                foreach (var timer in Timers)
                {
                    if (timer.IsEnabled)
                    {
                        timer.Update(Time);
                    }
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
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