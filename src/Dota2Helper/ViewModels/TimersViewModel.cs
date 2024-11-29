using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dota2Helper.Features.Settings;
using Dota2Helper.Features.TimeProvider;
using Dota2Helper.Features.Timers;
using Dota2Helper.Views;
using Microsoft.Extensions.DependencyInjection;
using TimeProvider = System.TimeProvider;

namespace Dota2Helper.ViewModels;

public class TimersViewModel : ViewModelBase, IDisposable, IAsyncDisposable
{
    readonly SettingsViewModel _settingsViewModel;
    readonly ITimeProvider _timeProvider;
    readonly ITimer? _timer;
    TimeSpan _time;

    ObservableCollection<DotaTimerViewModel> _timers;

    public ObservableCollection<DotaTimerViewModel> Timers
    {
        get => _timers;
        set => SetProperty(ref _timers, value);
    }

    public ObservableCollection<DotaTimerViewModel> VisibleTimers
    {
        get => _visibleTimers;
        set => SetProperty(ref _visibleTimers, value);
    }

    readonly SemaphoreSlim _semaphore = new(1, 1);
    ObservableCollection<DotaTimerViewModel> _visibleTimers = new();

    public TimersViewModel(SettingsViewModel settingsViewModel, ITimeProvider timeProvider)
    {
        _settingsViewModel = settingsViewModel;
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

        if (settingsWindow.WindowState == WindowState.Minimized)
            settingsWindow.WindowState = WindowState.Normal;
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
                Timers = _settingsViewModel.SelectedProfileViewModel.Timers;
                Time = _timeProvider.Time;

                foreach (var timer in Timers)
                {
                    timer.Update(Time);
                }

                VisibleTimers = new(Timers.Where(t => t.IsVisible));
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