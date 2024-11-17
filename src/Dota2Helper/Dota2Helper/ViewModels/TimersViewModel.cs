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
    readonly ITimer? _timer;
    TimeSpan _time;

    public SettingsViewModel SettingsViewModel { get; }

    readonly SemaphoreSlim _semaphore = new(1, 1);

    public TimersViewModel(SettingsViewModel settingsViewModel, ITimeProvider timeProvider)
    {
        SettingsViewModel = settingsViewModel;
        _timeProvider = timeProvider;

        OpenSettingsCommand = new RelayCommand(OpenSettings);
        CloseAppCommand = new RelayCommand(CloseApplication);

        _timer = TimeProvider.System.CreateTimer(
            callback: UpdateTimers,
            state: null,
            dueTime: TimeSpan.Zero,
            period: TimeSpan.FromSeconds(0.500)
        );
    }

    void OpenSettings()
    {
        var settingsWindow = App.ServiceProvider.GetRequiredService<SettingsWindow>();
        settingsWindow.Show();
    }

    static void CloseApplication()
    {
        if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
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