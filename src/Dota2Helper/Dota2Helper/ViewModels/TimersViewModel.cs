using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using Dota2Helper.Features.TimeProvider;
using Dota2Helper.Views;
using Microsoft.Extensions.DependencyInjection;
using TimeProvider = System.TimeProvider;

namespace Dota2Helper.ViewModels;

public class TimersViewModel : ViewModelBase, IDisposable, IAsyncDisposable
{
    readonly ITimeProvider _timeProvider;
    readonly ITimer? _timer = null;
    TimeSpan _time;

    public SettingsViewModel SettingsViewModel { get; }

    public PixelPoint TopLeft { get; set; } = new(10, 10);

    readonly SemaphoreSlim _semaphore = new(1, 1);

    public TimersViewModel(SettingsViewModel settingsViewModel, ITimeProvider timeProvider)
    {
        SettingsViewModel = settingsViewModel;
        _timeProvider = timeProvider;

        OpenSettingsCommand = new RelayCommand(() =>
            {
                var settingsWindow = App.ServiceProvider.GetRequiredService<SettingsWindow>();
                settingsWindow.Show();
            }
        );

        CloseAppCommand = new RelayCommand(() =>
            {
                if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    desktop.Shutdown();
                }
            }
        );

        _timer = TimeProvider.System.CreateTimer(
            callback: UpdateTimers,
            state: null,
            dueTime: TimeSpan.Zero,
            period: TimeSpan.FromSeconds(0.500)
        );
    }

    async void UpdateTimers(object? state)
    {
        if (await _semaphore.WaitAsync(0))
        {
            try
            {
                Time = _timeProvider.Time;

                foreach (var timer in SettingsViewModel.SelectedProfileViewModel.Timers)
                {
                    timer.Update(Time);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }

    public IRelayCommand OpenSettingsCommand { get; }

    public TimeSpan Time
    {
        get => _time;
        set => SetProperty(ref _time, value);
    }

    public IRelayCommand CloseAppCommand { get; }

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